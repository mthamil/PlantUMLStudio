using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PlantUmlStudio.Configuration;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.InputOutput;
using PlantUmlStudio.ViewModel;
using PlantUmlStudio.ViewModel.Notifications;
using SharpEssentials.Concurrency;
using SharpEssentials.InputOutput;
using SharpEssentials.Testing;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.ViewModel
{
	public class DiagramExplorerViewModelTests
	{
		public DiagramExplorerViewModelTests()
		{
			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
			         .Returns(Task.CompletedTask);

			settings.SetupProperty(s => s.LastDiagramLocation);

			notifications.Setup(p => p.StartProgress(It.IsAny<bool>()))
			             .Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);

			explorer = new DiagramExplorerViewModel(notifications.Object, diagramIO.Object,
				d => d == null ? null : new PreviewDiagramViewModel(d), settings.Object, uiScheduler)
			{
				FileExtension = ".puml"
			};
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_SuccessfulLoad()
		{
			// Arrange.
			var diagrams = new List<Diagram>
			{
				new Diagram { Content = "Diagram 1"},
				new Diagram { Content = "Diagram 2" },
				null
			};

			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<ReadDiagramsProgress>>(), It.IsAny<CancellationToken>()))
			         .Returns(() => Task.FromResult<IEnumerable<Diagram>>(diagrams))
			         .Callback((DirectoryInfo dir, IProgress<ReadDiagramsProgress> prog, CancellationToken ct) =>
			         {
				         foreach (var diagram in diagrams)
					         prog.Report(new ReadDiagramsProgress(1, 1, diagram));
			         });

			// Act.
			explorer.DiagramLocation = diagramLocation;
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Equal(2, explorer.PreviewDiagrams.Count);
			AssertThat.SequenceEqual(new[] { "Diagram 1", "Diagram 2" }, explorer.PreviewDiagrams.Select(d => d.Diagram.Content));

			Assert.Equal(diagramLocation.FullName, settings.Object.LastDiagramLocation.FullName);
			diagramIO.Verify(dio => dio.StartMonitoring(diagramLocation));
			Assert.False(explorer.IsLoadingDiagrams);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_UnsuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<ReadDiagramsProgress>>(), It.IsAny<CancellationToken>()))
			         .Returns(Task.FromException<IEnumerable<Diagram>>(new AggregateException()));

			// Act.
			explorer.DiagramLocation = diagramLocation;
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Empty(explorer.PreviewDiagrams);
			Assert.Equal(diagramLocation.FullName, settings.Object.LastDiagramLocation.FullName);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_False()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<ReadDiagramsProgress>>(), It.IsAny<CancellationToken>()))
			         .Returns(Task.FromResult(Enumerable.Empty<Diagram>()));

			// Act.
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.False(isValid);
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.IsAny<DirectoryInfo>(),
				It.IsAny<IProgress<ReadDiagramsProgress>>(), 
				It.IsAny<CancellationToken>()), Times.Never());
			settings.VerifySet(s => s.LastDiagramLocation = It.IsAny<DirectoryInfo>(), Times.Never());
		}

		[Fact]
		public async Task Test_LoadDiagramsCommand_InvalidDirectory()
		{
			// Act.
			await explorer.LoadDiagramsCommand.ExecuteAsync(new DirectoryInfo("test"));

			// Assert.
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.IsAny<DirectoryInfo>(),
				It.IsAny<IProgress<ReadDiagramsProgress>>(), 
				It.IsAny<CancellationToken>()), Times.Never());
			Assert.False(explorer.IsLoadingDiagrams);
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand()
		{
			// Arrange.
			string newDiagramFilePath = Path.Combine(diagramLocation.FullName, "new-diagram.puml");

			explorer.DiagramLocation = diagramLocation;
			explorer.NewDiagramTemplate = "New Diagram";

			OpenPreviewRequestedEventArgs newDiagramArgs = null;
			explorer.OpenPreviewRequested += (o, e) => newDiagramArgs = e;

			diagramIO.Setup(dio => dio.ReadAsync(It.IsAny<FileInfo>()))
			         .Returns((FileInfo file) => Task.FromResult(new Diagram { File = file }));

			// Act.
			explorer.AddNewDiagramCommand.Execute(new Uri(newDiagramFilePath));

			// Assert.
			Assert.Single(explorer.PreviewDiagrams);
			Assert.Equal(newDiagramFilePath, explorer.PreviewDiagrams.Single().Diagram.File.FullName);

			Assert.NotNull(newDiagramArgs);
			Assert.Equal(explorer.PreviewDiagrams.Single(), newDiagramArgs.RequestedPreview);
			Assert.Equal(explorer.PreviewDiagrams.Single(), explorer.CurrentPreviewDiagram);

			diagramIO.Verify(dio => dio.SaveAsync(
				It.Is<Diagram>(d => d.File.FullName == newDiagramFilePath),
				false));
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand_FileNameWithoutExtension()
		{
			// Arrange.
			explorer.DiagramLocation = diagramLocation;
			explorer.NewDiagramTemplate = "New Diagram";

			// Act.
			explorer.AddNewDiagramCommand.Execute(new Uri(diagramLocation.FullName + @"\test-diagram"));

			// Assert.
			diagramIO.Verify(io => io.SaveAsync(
				It.Is<Diagram>(d => d.File.FullName == diagramLocation.FullName + @"\test-diagram.puml"), 
				It.IsAny<bool>()));
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand_Unsuccessful()
		{
			// Arrange.
			string newDiagramFilePath = Path.Combine(diagramLocation.FullName, "new-diagram.puml");

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
			         .Returns(Task.FromException(new InvalidOperationException()));

			explorer.DiagramLocation = diagramLocation;
			explorer.NewDiagramTemplate = "New Diagram";

			OpenPreviewRequestedEventArgs newDiagramArgs = null;
			explorer.OpenPreviewRequested += (o, e) => newDiagramArgs = e;

			// Act.
			explorer.AddNewDiagramCommand.Execute(new Uri(newDiagramFilePath));

			// Assert.
			Assert.Null(newDiagramArgs);

			diagramIO.Verify(dio => dio.SaveAsync(
				It.Is<Diagram>(d => d.Content == "New Diagram" && d.File.FullName == newDiagramFilePath),
				false));
		}

		[Theory]
		[MemberData(nameof(CanRequestOpenPreviewData))]
		public void Test_CanRequestOpenPreview(bool expected, PreviewDiagramViewModel preview)
		{
			// Act.
			bool actual = explorer.RequestOpenPreviewCommand.CanExecute(preview);

			// Assert.
			Assert.Equal(expected, actual);
		}

		public static IEnumerable<object[]> CanRequestOpenPreviewData
		{
			get
			{
				return new TheoryData<bool, PreviewDiagramViewModel>
				{
					{ false, null },
					{ true, new PreviewDiagramViewModel(new Diagram()) }
				};
			}
		}

		[Fact]
		public void Test_RequestOpenPreviewCommand()
		{
			// Arrange.
			var preview = new PreviewDiagramViewModel(new Diagram());

			OpenPreviewRequestedEventArgs args = null;
			EventHandler<OpenPreviewRequestedEventArgs> handler = (o, e) => args = e;
			explorer.OpenPreviewRequested += handler;

			// Act.
			explorer.RequestOpenPreviewCommand.Execute(preview);

			// Assert.
			Assert.NotNull(args);
			Assert.Equal(preview, args.RequestedPreview);
		}

		[Fact]
		public void Test_OpenDiagram()
		{
			// Arrange.
			string filePath = Path.Combine(diagramLocation.FullName, "TestFile.puml");
			var diagramUri = new Uri(filePath, UriKind.Absolute);

			diagramIO.Setup(io => io.ReadAsync(It.Is<FileInfo>(f => f.FullName == filePath)))
			         .Returns((FileInfo file) => Task.FromResult(new Diagram { File = file }));

			// Act/Assert.
			var args = AssertThat.RaisesWithEventArgs<IDiagramExplorer, OpenPreviewRequestedEventArgs>(
				explorer, e => e.OpenPreviewRequested += null,
				() => explorer.OpenDiagramCommand.Execute(diagramUri));

			Assert.Equal(filePath, args.RequestedPreview.Diagram.File.FullName);
		}

		[Fact]
		public async Task Test_OpenDiagram_When_PreviewAlreadyExists()
		{
			// Arrange.
			string filePath = Path.Combine(diagramLocation.FullName, "TestFile.puml");

			var existingDiagram = new Diagram { File = new FileInfo(filePath) };
			var existingPreview = new PreviewDiagramViewModel(existingDiagram);
			explorer.PreviewDiagrams.Add(existingPreview);

			var diagramUri = new Uri(filePath, UriKind.Absolute);

			diagramIO.Setup(io => io.ReadAsync(It.Is<FileInfo>(f => f.FullName == filePath)))
			         .Returns((FileInfo file) => Task.FromResult(new Diagram { File = file }));

			OpenPreviewRequestedEventArgs openArgs = null;
			explorer.OpenPreviewRequested += (o, e) => openArgs = e;

			// Act.
			var openedDiagram = await explorer.OpenDiagramAsync(diagramUri);

			// Assert.
			Assert.Same(existingDiagram, openedDiagram);
			Assert.Same(existingPreview.Diagram, explorer.PreviewDiagrams.Single().Diagram);
			Assert.NotNull(openArgs);
			Assert.Same(existingPreview, openArgs.RequestedPreview);
		}

		[Fact]
		public async Task Test_OpenDiagramFilesAsync()
		{
			// Arrange.
			var diagrams = new[] { new FileInfo("test1.puml"), new FileInfo("test2.puml") };

			diagramIO.Setup(io => io.ReadAsync(It.IsAny<FileInfo>()))
					 .Returns((FileInfo file) => Task.FromResult(new Diagram { File = file }));

			var args = new List<OpenPreviewRequestedEventArgs>();
			explorer.OpenPreviewRequested += (o, e) => args.Add(e);

			// Act.
			await explorer.OpenDiagramFilesAsync(diagrams);

			// Assert.
			foreach (var diagram in diagrams)
				diagramIO.Verify(io => io.ReadAsync(It.Is<FileInfo>(f => f.FullName == diagram.FullName)));

			AssertThat.SequenceEqual(diagrams, args.Select(arg => arg.RequestedPreview.Diagram.File), FileSystemInfoPathEqualityComparer.Instance);
		}

		[Theory]
		[MemberData(nameof(CanDeleteDiagramData))]
		public void Test_CanDeleteDiagram(bool expected, PreviewDiagramViewModel preview)
		{
			// Act.
			bool actual = explorer.DeleteDiagramCommand.CanExecute(preview);

			// Assert.
			Assert.Equal(expected, actual);
		}

		public static IEnumerable<object[]> CanDeleteDiagramData
		{
			get
			{
				return new TheoryData<bool, PreviewDiagramViewModel>
				{
					{ false, null },
					{ true, new PreviewDiagramViewModel(new Diagram()) }
				};
			}
		}

		[Fact]
		public async Task Test_DeleteDiagramCommand()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.DeleteAsync(It.IsAny<Diagram>()))
			         .Returns(Task.CompletedTask);

			var preview = new PreviewDiagramViewModel(new Diagram { File = new FileInfo("TestFile") });
			explorer.PreviewDiagrams.Add(preview);

			// Act.
			await explorer.DeleteDiagramCommand.ExecuteAsync(preview);

			// Assert.
			Assert.Empty(explorer.PreviewDiagrams);
			diagramIO.Verify(dio => dio.DeleteAsync(preview.Diagram));
		}

		[Fact]
		public void Test_DiagramFileDeleted()
		{
			// Arrange.
			var preview = new PreviewDiagramViewModel(new Diagram { File = new FileInfo("test.puml")});
			explorer.PreviewDiagrams.Add(preview);

			DiagramDeletedEventArgs deleteArgs = null;
			EventHandler<DiagramDeletedEventArgs> deleteHandler = (o, e) => deleteArgs = e;
			explorer.DiagramDeleted += deleteHandler;

			// Act.
			diagramIO.Raise(dio => dio.DiagramFileDeleted += null, new DiagramFileDeletedEventArgs(new FileInfo("test.puml")));

			// Assert.
			Assert.Empty(explorer.PreviewDiagrams);
			Assert.NotNull(deleteArgs);
			Assert.Equal("test.puml", deleteArgs.DeletedDiagram.File.Name);
		}

		[Fact]
		public void Test_DiagramFileAdded()
		{
			// Arrange.
			var newFile = new FileInfo("test.puml");

			explorer.DiagramLocation = newFile.Directory;

			diagramIO.Setup(dio => dio.ReadAsync(It.IsAny<FileInfo>()))
			         .Returns(Task.FromResult(new Diagram
			         {
				         File = newFile,
				         Content = "New Diagram"
			         }));

			// Act.
			diagramIO.Raise(dio => dio.DiagramFileAdded += null, new DiagramFileAddedEventArgs(newFile));

			// Assert.
			Assert.Single(explorer.PreviewDiagrams);
			Assert.Equal(newFile.FullName, explorer.PreviewDiagrams.Single().Diagram.File.FullName);
		}

		[Fact]
		public void Test_DiagramFileAdded_PreviewAlreadyExists()
		{
			// Arrange.
			var existingFile = new FileInfo("test.puml");
			var existingPreview = new PreviewDiagramViewModel(new Diagram { File = existingFile });

			explorer.DiagramLocation = existingFile.Directory;
			explorer.PreviewDiagrams.Add(existingPreview);

			diagramIO.Setup(dio => dio.ReadAsync(It.IsAny<FileInfo>()))
					 .Returns(Task.FromResult(new Diagram
					 {
						 File = existingFile,
						 Content = "New Diagram"
					 }));

			// Act.
			diagramIO.Raise(dio => dio.DiagramFileAdded += null, new DiagramFileAddedEventArgs(existingFile));

			// Assert.
			Assert.Single(explorer.PreviewDiagrams);
			Assert.Same(existingPreview, explorer.PreviewDiagrams.Single());
		}

		[Fact]
		public void Test_DiagramFileAdded_NullDiagram()
		{
			// Arrange.
			var newFile = new FileInfo("test.puml");

			explorer.DiagramLocation = newFile.Directory;

			diagramIO.Setup(dio => dio.ReadAsync(It.IsAny<FileInfo>()))
			         .Returns(Task.FromResult<Diagram>(null));

			// Act.
			diagramIO.Raise(dio => dio.DiagramFileAdded += null, new DiagramFileAddedEventArgs(newFile));

			// Assert.
			Assert.Empty(explorer.PreviewDiagrams);
		}

		private readonly DiagramExplorerViewModel explorer;

		private readonly Mock<INotifications> notifications = new Mock<INotifications>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService> { DefaultValue = DefaultValue.Empty };
		private readonly Mock<ISettings> settings = new Mock<ISettings>();
		private readonly TaskScheduler uiScheduler = new SynchronousTaskScheduler();

		private static readonly DirectoryInfo diagramLocation = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDiagrams"));
	}
}