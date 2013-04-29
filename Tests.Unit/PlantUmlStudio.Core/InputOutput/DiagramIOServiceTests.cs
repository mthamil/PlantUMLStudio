using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.InputOutput;
using Utilities;
using Utilities.Concurrency;
using Utilities.InputOutput;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.Core.InputOutput
{
	public class DiagramIOServiceTests
	{
		public DiagramIOServiceTests()
		{
			diagramIO = new DiagramIOService(scheduler, monitor.Object)
			{
				FileFilter = "*.puml"
			};
		}

		[Fact]
		public void Test_DiagramAdded()
		{
			// Arrange.
			DiagramFileAddedEventArgs args = null;
			EventHandler<DiagramFileAddedEventArgs> addHandler = (o, e) => args = e;
			diagramIO.DiagramFileAdded += addHandler;

			// Act.
			monitor.Raise(m => m.Created += null, 
				new FileSystemEventArgs(WatcherChangeTypes.Created, currentDirectory.FullName, "class.puml"));

			// Assert.
			Assert.NotNull(args);
			Assert.Equal(Path.Combine(currentDirectory.FullName, "class.puml"), args.NewDiagramFile.FullName);
		}

		[Fact]
		public void Test_DiagramDeleted()
		{
			// Arrange.
			DiagramFileDeletedEventArgs args = null;
			EventHandler<DiagramFileDeletedEventArgs> deleteHandler = (o, e) => args = e;
			diagramIO.DiagramFileDeleted += deleteHandler;

			// Act.
			monitor.Raise(m => m.Deleted += null,
				new FileSystemEventArgs(WatcherChangeTypes.Deleted, currentDirectory.FullName, "class.puml"));

			// Assert.
			Assert.NotNull(args);
			Assert.Equal(Path.Combine(currentDirectory.FullName, "class.puml"), args.DeletedDiagramFile.FullName);
		}

		[Fact]
		public void Test_StartMonitoring()
		{
			// Act.
			diagramIO.StartMonitoring(currentDirectory);

			// Assert.
			monitor.Verify(m => m.StartMonitoring(currentDirectory));
		}

		[Fact]
		public void Test_StopMonitoring()
		{
			// Act.
			diagramIO.StopMonitoring();

			// Assert.
			monitor.Verify(m => m.StopMonitoring());
		}

		[Fact]
		public async Task Test_ReadAsync()
		{
			// Arrange.
			var file = new FileInfo(Path.Combine(currentDirectory.FullName, "class.puml"));

			// Act.
			var diagram = await diagramIO.ReadAsync(file);

			// Assert.
			Assert.Equal(file.FullName, diagram.File.FullName);
			Assert.Equal(Path.Combine(currentDirectory.FullName, @"img\classes04.png"), diagram.ImageFile.FullName);
			Assert.True(!String.IsNullOrWhiteSpace(diagram.Content));
		}

		[Fact]
		public void Test_ReadAsync_InvalidDiagram()
		{
			// Arrange.
			var file = new FileInfo(Path.Combine(currentDirectory.FullName, "invalid.puml"));

			// Act/Assert.
			AssertThat.Throws<InvalidDiagramFileException>(diagramIO.ReadAsync(file));
		}

		[Fact]
		public async Task Test_ReadDiagramsAsync()
		{
			// Arrange.
			var progressData = new List<ReadDiagramsProgress>();

			var progress = new Mock<IProgress<ReadDiagramsProgress>>();
			progress.Setup(p => p.Report(It.IsAny<ReadDiagramsProgress>()))
			        .Callback((ReadDiagramsProgress p) => progressData.Add(p));

			// Act.
			var diagrams = await diagramIO.ReadDiagramsAsync(currentDirectory, CancellationToken.None, progress.Object);

			// Assert.
			Assert.Single(diagrams);

			var diagram = diagrams.Single();
			Assert.Equal(Path.Combine(currentDirectory.FullName, "class.puml"), diagram.File.FullName);
			Assert.Equal(Path.Combine(currentDirectory.FullName, @"img\classes04.png"), diagram.ImageFile.FullName);
			Assert.True(!String.IsNullOrWhiteSpace(diagram.Content));

			Assert.Equal(2, progressData.Count);

			Assert.Equal(Path.Combine(currentDirectory.FullName, "class.puml"), progressData.First().Diagram.Value.File.FullName);
			Assert.Equal(1, progressData.First().ProcessedDiagramCount);
			Assert.Equal(2, progressData.First().TotalDiagramCount);

			Assert.Equal(Option<Diagram>.None(), progressData.Last().Diagram);
			Assert.Equal(2, progressData.Last().ProcessedDiagramCount);
			Assert.Equal(2, progressData.Last().TotalDiagramCount);
		}

		[Fact]
		public async Task Test_SaveAsync()
		{
			// Arrange.
			using (var temp = new TemporaryFile())
			{
				var diagram = new Diagram
				{
					File = temp.File,
					Content = "test test test"
				};

				// Act.
				await diagramIO.SaveAsync(diagram, false);

				// Assert.
				Assert.Equal("test test test", File.ReadAllText(temp.File.FullName));
				Assert.False(File.Exists(temp.File.FullName + ".bak"));
			}
		}

		[Fact]
		public async Task Test_SaveAsync_WithBackup()
		{
			// Arrange.
			using (var temp = new TemporaryFile())
			{
				File.WriteAllText(temp.File.FullName, "original");

				var diagram = new Diagram
				{
					File = temp.File,
					Content = "test test test"
				};

				// Act.
				await diagramIO.SaveAsync(diagram, true);
				var backup = new FileInfo(temp.File.FullName + ".bak");

				// Assert.
				Assert.Equal("test test test", File.ReadAllText(temp.File.FullName));
				Assert.True(backup.Exists);
				Assert.Equal("original", File.ReadAllText(backup.FullName));

				backup.Delete();
			}
		}

		[Fact]
		public async Task Test_SaveAsync_When_ContentShortened()
		{
			// Arrange.
			using (var temp = new TemporaryFile())
			{
				var diagram = new Diagram
				{
					File = temp.File,
					Content = "test\ntest\ntest"
				};

				File.WriteAllText(diagram.File.FullName, diagram.Content);

				diagram.Content = "test\ntest";

				// Act.
				await diagramIO.SaveAsync(diagram, false);

				// Assert.
				Assert.Equal("test\ntest", File.ReadAllText(temp.File.FullName));
			}
		}

		[Fact]
		public async Task Test_DeleteAsync()
		{
			// Arrange.
			using (var tempDiagram = new TemporaryFile().Touch())
			using (var tempImage = new TemporaryFile().Touch())
			{
				var diagram = new Diagram
				{
					File = tempDiagram.File,
					ImageFile = tempImage.File
				};

				// Act.
				await diagramIO.DeleteAsync(diagram);
				tempDiagram.File.Refresh();
				tempImage.File.Refresh();

				// Assert.
				Assert.False(tempDiagram.File.Exists);
				Assert.False(tempImage.File.Exists);
			}
		}

		private readonly DiagramIOService diagramIO;

		private readonly TaskScheduler scheduler = new SynchronousTaskScheduler();
		private readonly Mock<IDirectoryMonitor> monitor = new Mock<IDirectoryMonitor>();

		private static readonly DirectoryInfo currentDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDiagrams"));
	}
}