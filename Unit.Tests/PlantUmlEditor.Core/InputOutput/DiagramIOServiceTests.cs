using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PlantUmlEditor.Core.InputOutput;
using Utilities.Concurrency;
using Utilities.InputOutput;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.Core.InputOutput
{
	public class DiagramIOServiceTests
	{
		public DiagramIOServiceTests()
		{
			diagramIO = new DiagramIOService(scheduler, fileSystemWatcher.Object);
		}

		[Fact(Skip="File system watcher created event needs better handling")]
		public void Test_DiagramAdded()
		{
			// Arrange.
			DiagramFileAddedEventArgs args = null;
			EventHandler<DiagramFileAddedEventArgs> addHandler = (o, e) => args = e;
			diagramIO.DiagramAdded += addHandler;

			// Act.
			fileSystemWatcher.Raise(fsw => fsw.Created += null, 
				new FileSystemEventArgs(WatcherChangeTypes.Created, currentDirectory.FullName, "class.puml"));

			// Assert.
			Assert.NotNull(args);
			Assert.Equal(Path.Combine(currentDirectory.FullName, "class.puml"), args.NewDiagram.File.FullName);
			Assert.True(!String.IsNullOrWhiteSpace(args.NewDiagram.Content));
		}

		[Fact]
		public void Test_DiagramDeleted()
		{
			// Arrange.
			DiagramFileDeletedEventArgs args = null;
			EventHandler<DiagramFileDeletedEventArgs> deleteHandler = (o, e) => args = e;
			diagramIO.DiagramDeleted += deleteHandler;

			// Act.
			fileSystemWatcher.Raise(fsw => fsw.Deleted += null,
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
			fileSystemWatcher.VerifySet(fsw => fsw.Path = currentDirectory.FullName);
			fileSystemWatcher.VerifySet(fsw => fsw.EnableRaisingEvents = true);
		}

		[Fact]
		public void Test_StopMonitoring()
		{
			// Act.
			diagramIO.StopMonitoring();

			// Assert.
			fileSystemWatcher.VerifySet(fsw => fsw.EnableRaisingEvents = false);
		}

		[Fact]
		public void Test_ReadAsync()
		{
			// Arrange.
			var file = new FileInfo(Path.Combine(currentDirectory.FullName, "class.puml"));

			// Act.
			var readTask = diagramIO.ReadAsync(file);
			readTask.Wait();
			var diagram = readTask.Result;

			// Assert.
			Assert.Equal(file.FullName, diagram.File.FullName);
			Assert.Equal(Path.Combine(currentDirectory.FullName, @"img\classes04.png"), diagram.ImageFilePath);
			Assert.True(!String.IsNullOrWhiteSpace(diagram.Content));
		}

		[Fact]
		public void Test_ReadDiagramsAsync()
		{
			// Arrange.
			var progress = new Mock<IProgress<Tuple<int, int>>>();

			// Act.
			var readTask = diagramIO.ReadDiagramsAsync(currentDirectory, progress.Object);
			readTask.Wait();
			var diagrams = readTask.Result;

			// Assert.
			Assert.Single(diagrams);
			Assert.Equal(Path.Combine(currentDirectory.FullName, "class.puml"), diagrams.Single().File.FullName);
			Assert.Equal(Path.Combine(currentDirectory.FullName, @"img\classes04.png"), diagrams.Single().ImageFilePath);
			Assert.True(!String.IsNullOrWhiteSpace(diagrams.Single().Content));

			progress.Verify(p => p.Report(Tuple.Create(1, 1)));
		}

		private readonly DiagramIOService diagramIO;

		private readonly TaskScheduler scheduler = new SynchronousTaskScheduler();
		private readonly Mock<IFileSystemWatcher> fileSystemWatcher = new Mock<IFileSystemWatcher>();

		private static readonly DirectoryInfo currentDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDiagrams"));
	}
}