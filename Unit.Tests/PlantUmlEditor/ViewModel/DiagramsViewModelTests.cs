using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Moq;
using PlantUmlEditor.Model;
using PlantUmlEditor.ViewModel;
using Utilities.Concurrency;
using Utilities.Mvvm;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class DiagramsViewModelTests
	{
		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_SuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult<IEnumerable<Diagram>>(new List<Diagram>
				{
					new Diagram { Content = "Diagram 1"},
					new Diagram { Content = "Diagram 2" }
				}));

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, null, d => new PreviewDiagramViewModel(d))
			{
				DiagramLocation = testDiagramFile.Directory
			};

			// Act.
			bool isValid = diagrams.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Equal(2, diagrams.PreviewDiagrams.Count);
			AssertThat.SequenceEqual(new [] { "Diagram 1", "Diagram 2" }, diagrams.PreviewDiagrams.Select(d => d.Diagram.Content));
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_UnsuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromException<IEnumerable<Diagram>, AggregateException>(new AggregateException()));

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, null, d => new PreviewDiagramViewModel(d))
			{
				DiagramLocation = testDiagramFile.Directory
			};

			// Act.
			bool isValid = diagrams.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Empty(diagrams.PreviewDiagrams);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_False()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult(Enumerable.Empty<Diagram>()));

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, null, null);

			// Act.
			bool isValid = diagrams.IsDiagramLocationValid;

			// Assert.
			Assert.False(isValid);
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.IsAny<DirectoryInfo>(), 
				It.IsAny<IProgress<Tuple<int?, string>>>()), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramCommand()
		{
			// Arrange.
			var diagramPreview = new PreviewDiagramViewModel(new Diagram { File = testDiagramFile });
			var editor = new Mock<IDiagramEditor>();

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, null);

			// Act.
			diagrams.OpenDiagramCommand.Execute(diagramPreview);

			// Assert.
			Assert.Single(diagrams.OpenDiagrams);
			Assert.Equal(editor.Object, diagrams.OpenDiagram);
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramCommand_DiagramAlreadyOpenedOnce()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var diagramPreview = new PreviewDiagramViewModel(diagram);
			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(diagram);

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, null, null);
			diagrams.OpenDiagrams.Add(editor.Object);

			// Act.
			diagrams.OpenDiagramCommand.Execute(diagramPreview);

			// Assert.
			Assert.Single(diagrams.OpenDiagrams);
			Assert.Equal(editor.Object, diagrams.OpenDiagram);
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditor_Closed()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var diagramPreview = new PreviewDiagramViewModel(diagram);

			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(diagram);

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, null);
			diagrams.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagrams.OpenDiagrams);
			editor.Verify(e => e.Save(), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditor_Closing_Saved()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var diagramPreview = new PreviewDiagramViewModel(diagram);

			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(diagram);
			editor.Setup(e => e.Save()).Returns(Tasks.FromSuccess());

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, null);
			diagrams.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closing += null, new CancelEventArgs());
			diagrams.SaveClosingDiagramCommand.Execute(null);

			// Assert.
			Assert.Equal(editor.Object, diagrams.ClosingDiagram);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagrams.OpenDiagrams);
			editor.Verify(e => e.Save(), Times.Exactly(1));
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditor_Closing_NotSaved()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var diagramPreview = new PreviewDiagramViewModel(diagram);

			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(diagram);

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, null);
			diagrams.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closing += null, new CancelEventArgs());

			// Assert.
			Assert.Equal(editor.Object, diagrams.ClosingDiagram);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagrams.OpenDiagrams);
			editor.Verify(e => e.Save(), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditor_DifferentEditorClosing()
		{
			// Arrange.
			var previewMap = new Dictionary<PreviewDiagramViewModel, IDiagramEditor>();

			var diagram1 = new Diagram { File = testDiagramFile };
			var diagramPreview1 = new PreviewDiagramViewModel(diagram1);

			var editor1 = new Mock<IDiagramEditor>();
			editor1.SetupGet(e => e.Diagram).Returns(diagram1);
			previewMap[diagramPreview1] = editor1.Object;

			var diagram2 = new Diagram { File = new FileInfo(testDiagramFile.FullName + "2") };
			var diagramPreview2 = new PreviewDiagramViewModel(diagram2);

			var editor2 = new Mock<IDiagramEditor>();
			editor2.SetupGet(e => e.Diagram).Returns(diagram2);
			previewMap[diagramPreview2] = editor2.Object;

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => previewMap[d], null);
			diagrams.OpenDiagramCommand.Execute(diagramPreview1);
			diagrams.OpenDiagramCommand.Execute(diagramPreview2);

			// Act.
			editor2.Raise(e => e.Closing += null, new CancelEventArgs());
			diagrams.SaveClosingDiagramCommand.Execute(null);

			// Assert.
			Assert.Equal(editor2.Object, diagrams.ClosingDiagram);

			// Act.
			editor1.Raise(e => e.Closed += null, EventArgs.Empty);	// Raise the event for a different editor.

			// Assert.
			Assert.Single(diagrams.OpenDiagrams);
			Assert.Equal(editor2.Object, diagrams.OpenDiagrams.Single());
			editor1.Verify(e => e.Save(), Times.Never());
			editor2.Verify(e => e.Save(), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditor_Saved()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var image = new BitmapImage();

			var diagramPreview = new PreviewDiagramViewModel(diagram);

			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(diagram);
			editor.SetupGet(e => e.DiagramImage).Returns(image);

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, null);
			diagrams.PreviewDiagrams.Add(diagramPreview);
			diagrams.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Saved += null, EventArgs.Empty);

			// Assert.
			Assert.Equal(image, diagramPreview.ImagePreview);
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditor_Saved_NoMatchingPreview()
		{
			// Arrange.
			var image = new BitmapImage();

			var diagramPreview = new PreviewDiagramViewModel(new Diagram { File = new FileInfo(testDiagramFile.FullName + "2") });

			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(new Diagram { File = testDiagramFile });
			editor.SetupGet(e => e.DiagramImage).Returns(image);

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, null);
			diagrams.PreviewDiagrams.Add(diagramPreview);
			diagrams.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Saved += null, EventArgs.Empty);

			// Assert.
			Assert.Null(diagramPreview.ImagePreview);
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand()
		{
			// Arrange.
			var editor = new Mock<IDiagramEditor>();

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromResult<object>(null));

			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult<IEnumerable<Diagram>>(new List<Diagram> { new Diagram { File = testDiagramFile, Content = "New Diagram" } }));

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, d => new PreviewDiagramViewModel(d))
			{
				DiagramLocation = testDiagramFile.Directory,
				NewDiagramTemplate = "New Diagram"
			};

			// Act.
			diagrams.AddNewDiagramCommand.Execute(new Uri(testDiagramFile.FullName));

			// Assert.
			Assert.Single(diagrams.PreviewDiagrams);
			Assert.Equal(testDiagramFile.FullName, diagrams.PreviewDiagrams.Single().Diagram.File.FullName);
			Assert.Equal("New Diagram", diagrams.PreviewDiagrams.Single().Diagram.Content);

			Assert.Single(diagrams.OpenDiagrams);
			Assert.Equal(diagrams.OpenDiagrams.Single(), diagrams.OpenDiagram);
			Assert.Equal(diagrams.PreviewDiagrams.Single(), diagrams.CurrentPreviewDiagram);

			diagramIO.Verify(dio => dio.SaveAsync(
				It.Is<Diagram>(d => d.Content == "New Diagram" && d.File.FullName == testDiagramFile.FullName), 
				false));

			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.Is<DirectoryInfo>(d => d.FullName == testDiagramFile.Directory.FullName), 
				It.IsAny<IProgress<Tuple<int?, string>>>()), Times.AtLeastOnce());
		}

		private DiagramsViewModel diagrams;

		private readonly Mock<IProgressViewModel> progress = new Mock<IProgressViewModel>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();

		private static readonly FileInfo testDiagramFile = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestDiagrams\class.puml"));
	}
}