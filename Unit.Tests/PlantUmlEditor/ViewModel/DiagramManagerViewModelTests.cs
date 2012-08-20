using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.ViewModel;
using Utilities.Concurrency;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class DiagramManagerViewModelTests
	{
		public DiagramManagerViewModelTests()
		{
			var previews = new List<PreviewDiagramViewModel>();
			explorer.SetupGet(p => p.PreviewDiagrams).Returns(previews);
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramCommand()
		{
			// Arrange.
			var diagramPreview = new PreviewDiagramViewModel(new Diagram { File = testDiagramFile });
			var editor = new Mock<IDiagramEditor>();

			var diagramManager = CreateManager(d => editor.Object);

			// Act.
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Assert.
			Assert.Single(diagramManager.OpenDiagrams);
			Assert.Equal(editor.Object, diagramManager.OpenDiagram);
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

			var diagramManager = CreateManager(null);
			diagramManager.OpenDiagrams.Add(editor.Object);

			// Act.
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Assert.
			Assert.Single(diagramManager.OpenDiagrams);
			Assert.Equal(editor.Object, diagramManager.OpenDiagram);
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

			var diagramManager = CreateManager(d => editor.Object);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagramManager.OpenDiagrams);
			editor.Verify(e => e.SaveAsync(), Times.Never());
			editor.Verify(e => e.Dispose());
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
			editor.Setup(e => e.SaveAsync()).Returns(Tasks.FromSuccess());

			var diagramManager = CreateManager(d => editor.Object);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closing += null, new CancelEventArgs());
			diagramManager.SaveClosingDiagramCommand.Execute(null);

			// Assert.
			Assert.Equal(editor.Object, diagramManager.ClosingDiagram);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagramManager.OpenDiagrams);
			editor.Verify(e => e.SaveAsync(), Times.Exactly(1));
			editor.Verify(e => e.Dispose());
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

			var diagramManager = CreateManager(d => editor.Object);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closing += null, new CancelEventArgs());

			// Assert.
			Assert.Equal(editor.Object, diagramManager.ClosingDiagram);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagramManager.OpenDiagrams);
			editor.Verify(e => e.SaveAsync(), Times.Never());
			editor.Verify(e => e.Dispose());
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

			var diagramManager = CreateManager(d => previewMap[d]);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview1);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview2);

			// Act.
			editor2.Raise(e => e.Closing += null, new CancelEventArgs());
			diagramManager.SaveClosingDiagramCommand.Execute(null);

			// Assert.
			Assert.Equal(editor2.Object, diagramManager.ClosingDiagram);

			// Act.
			editor1.Raise(e => e.Closed += null, EventArgs.Empty);	// Raise the event for a different editor.

			// Assert.
			Assert.Single(diagramManager.OpenDiagrams);
			Assert.Equal(editor2.Object, diagramManager.OpenDiagrams.Single());
			editor1.Verify(e => e.SaveAsync(), Times.Never());
			editor2.Verify(e => e.SaveAsync(), Times.Never());
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

			var diagramManager = CreateManager(d => editor.Object);
			diagramManager.Explorer.PreviewDiagrams.Add(diagramPreview);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

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

			var diagramManager = CreateManager(d => editor.Object);
			diagramManager.Explorer.PreviewDiagrams.Add(diagramPreview);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Saved += null, EventArgs.Empty);

			// Assert.
			Assert.Null(diagramPreview.ImagePreview);
		}

		[Fact]
		[Synchronous]
		public void Test_OpenPreviewRequested()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var diagramPreview = new PreviewDiagramViewModel(diagram);

			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(diagram);

			var diagramManager = CreateManager(d => editor.Object);

			// Act.
			explorer.Raise(p => p.OpenPreviewRequested += null, new OpenPreviewRequestedEventArgs(diagramPreview));

			// Assert.
			Assert.Single(diagramManager.OpenDiagrams);
			Assert.Equal(editor.Object, diagramManager.OpenDiagrams.Single());
			Assert.Equal(editor.Object, diagramManager.OpenDiagram);
		}

		[Fact]
		[Synchronous]
		public void Test_CloseCommand_UnsavedDiagram()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };

			var unsavedCodeEditor = new Mock<ICodeEditor>();
			unsavedCodeEditor.SetupGet(ce => ce.IsModified).Returns(true);
			var unsavedEditorCloseCommand = new Mock<ICommand>();
			var unsavedEditor = new Mock<IDiagramEditor>();
			unsavedEditor.SetupGet(e => e.Diagram).Returns(diagram);
			unsavedEditor.SetupGet(e => e.CodeEditor).Returns(unsavedCodeEditor.Object);
			unsavedEditor.SetupGet(e => e.CloseCommand).Returns(unsavedEditorCloseCommand.Object);

			var codeEditor = new Mock<ICodeEditor>();
			codeEditor.SetupGet(ce => ce.IsModified).Returns(false);
			var editorCloseCommand = new Mock<ICommand>();
			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(diagram);
			editor.SetupGet(e => e.CodeEditor).Returns(codeEditor.Object);
			editor.SetupGet(e => e.CloseCommand).Returns(editorCloseCommand.Object);

			var diagramManager = CreateManager(d => unsavedEditor.Object);
			diagramManager.OpenDiagrams.Add(unsavedEditor.Object);
			diagramManager.OpenDiagrams.Add(editor.Object);

			// Act.
			diagramManager.CloseCommand.Execute(null);

			// Assert.
			unsavedEditorCloseCommand.Verify(c => c.Execute(It.IsAny<object>()));
			editorCloseCommand.Verify(c => c.Execute(It.IsAny<object>()), Times.Never());
		}

		private DiagramManagerViewModel CreateManager(Func<PreviewDiagramViewModel, IDiagramEditor> editorFactory)
		{
			return new DiagramManagerViewModel(explorer.Object, editorFactory, settings.Object);
		}

		private readonly Mock<IDiagramExplorer> explorer = new Mock<IDiagramExplorer>();
		private readonly Mock<ISettings> settings = new Mock<ISettings>();

		private static readonly FileInfo testDiagramFile = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestDiagrams\class.puml"));
	}
}