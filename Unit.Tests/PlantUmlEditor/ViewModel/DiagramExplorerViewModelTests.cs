using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.Core.InputOutput;
using PlantUmlEditor.ViewModel;
using PlantUmlEditor.ViewModel.Notifications;
using Utilities.Concurrency;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class DiagramExplorerViewModelTests
	{
		public DiagramExplorerViewModelTests()
		{
			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromSuccess());

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

			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<CancellationToken>(), It.IsAny<IProgress<ReadDiagramsProgress>>()))
				.Returns(() => Task.FromResult<IEnumerable<Diagram>>(diagrams))
				.Callback((DirectoryInfo dir, CancellationToken ct, IProgress<ReadDiagramsProgress> prog) =>
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
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<CancellationToken>(), It.IsAny<IProgress<ReadDiagramsProgress>>()))
				.Returns(Tasks.FromException<IEnumerable<Diagram>>(new AggregateException()));

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
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<CancellationToken>(), It.IsAny<IProgress<ReadDiagramsProgress>>()))
				.Returns(Task.FromResult(Enumerable.Empty<Diagram>()));

			// Act.
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.False(isValid);
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.IsAny<DirectoryInfo>(),
				It.IsAny<CancellationToken>(),
				It.IsAny<IProgress<ReadDiagramsProgress>>()), Times.Never());
			settings.VerifySet(s => s.LastDiagramLocation = It.IsAny<DirectoryInfo>(), Times.Never());
		}

		[Fact]
		public void Test_LoadDiagramsCommand_InvalidDirectory()
		{
			// Arrange.
			explorer.DiagramLocation = new DirectoryInfo("test");

			// Act.
			explorer.LoadDiagramsCommand.Execute(null);

			// Assert.
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.IsAny<DirectoryInfo>(),
				It.IsAny<CancellationToken>(),
				It.IsAny<IProgress<ReadDiagramsProgress>>()), Times.Never());
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
				.Returns(Tasks.FromException(new InvalidOperationException()));

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
		[PropertyData("CanRequestOpenPreviewData")]
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
				yield return new object[] { false, null };
				yield return new object[] { true, new PreviewDiagramViewModel(new Diagram()) };
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

		[Theory]
		[PropertyData("CanDeleteDiagramData")]
		public void Test_CanDeleteDiagram(bool expected, PreviewDiagramViewModel preview)
		{
			// Act.
			bool actual = explorer.DeleteDiagramCommand.CanExecute(preview);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Test_DeleteDiagramCommand()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.DeleteAsync(It.IsAny<Diagram>()))
				.Returns(Tasks.FromSuccess());

			var preview = new PreviewDiagramViewModel(new Diagram { File = new FileInfo("TestFile") });
			explorer.PreviewDiagrams.Add(preview);

			// Act.
			explorer.DeleteDiagramCommand.Execute(preview);

			// Assert.
			Assert.Empty(explorer.PreviewDiagrams);
			diagramIO.Verify(dio => dio.DeleteAsync(preview.Diagram));
		}

		public static IEnumerable<object[]> CanDeleteDiagramData
		{
			get
			{
				yield return new object[] { false, null };
				yield return new object[] { true, new PreviewDiagramViewModel(new Diagram()) };
			}
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
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();
		private readonly Mock<ISettings> settings = new Mock<ISettings>();
		private readonly TaskScheduler uiScheduler = new SynchronousTaskScheduler();

		private static readonly DirectoryInfo diagramLocation = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDiagrams"));
	}
}