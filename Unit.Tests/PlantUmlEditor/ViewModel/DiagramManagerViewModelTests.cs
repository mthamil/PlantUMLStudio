using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using Moq;
using PlantUmlEditor.Model;
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

			_diagramManager = new DiagramManagerViewModel(explorer.Object, d => editor.Object);

			// Act.
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Assert.
			Assert.Single(_diagramManager.OpenDiagrams);
			Assert.Equal(editor.Object, _diagramManager.OpenDiagram);
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

			_diagramManager = new DiagramManagerViewModel(explorer.Object, null);
			_diagramManager.OpenDiagrams.Add(editor.Object);

			// Act.
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Assert.
			Assert.Single(_diagramManager.OpenDiagrams);
			Assert.Equal(editor.Object, _diagramManager.OpenDiagram);
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

			_diagramManager = new DiagramManagerViewModel(explorer.Object, d => editor.Object);
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(_diagramManager.OpenDiagrams);
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

			_diagramManager = new DiagramManagerViewModel(explorer.Object, d => editor.Object);
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closing += null, new CancelEventArgs());
			_diagramManager.SaveClosingDiagramCommand.Execute(null);

			// Assert.
			Assert.Equal(editor.Object, _diagramManager.ClosingDiagram);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(_diagramManager.OpenDiagrams);
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

			_diagramManager = new DiagramManagerViewModel(explorer.Object, d => editor.Object);
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Closing += null, new CancelEventArgs());

			// Assert.
			Assert.Equal(editor.Object, _diagramManager.ClosingDiagram);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(_diagramManager.OpenDiagrams);
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

			_diagramManager = new DiagramManagerViewModel(explorer.Object, d => previewMap[d]);
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview1);
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview2);

			// Act.
			editor2.Raise(e => e.Closing += null, new CancelEventArgs());
			_diagramManager.SaveClosingDiagramCommand.Execute(null);

			// Assert.
			Assert.Equal(editor2.Object, _diagramManager.ClosingDiagram);

			// Act.
			editor1.Raise(e => e.Closed += null, EventArgs.Empty);	// Raise the event for a different editor.

			// Assert.
			Assert.Single(_diagramManager.OpenDiagrams);
			Assert.Equal(editor2.Object, _diagramManager.OpenDiagrams.Single());
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

			_diagramManager = new DiagramManagerViewModel(explorer.Object, d => editor.Object);
			_diagramManager.Explorer.PreviewDiagrams.Add(diagramPreview);
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview);

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

			_diagramManager = new DiagramManagerViewModel(explorer.Object, d => editor.Object);
			_diagramManager.Explorer.PreviewDiagrams.Add(diagramPreview);
			_diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			editor.Raise(e => e.Saved += null, EventArgs.Empty);

			// Assert.
			Assert.Null(diagramPreview.ImagePreview);
		}

		[Fact]
		[Synchronous]
		public void Test_NewDiagramCreated()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var diagramPreview = new PreviewDiagramViewModel(diagram);

			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(diagram);

			_diagramManager = new DiagramManagerViewModel(explorer.Object, d => editor.Object);

			// Act.
			explorer.Raise(p => p.NewDiagramCreated += null, new NewDiagramCreatedEventArgs(diagramPreview));

			// Assert.
			Assert.Single(_diagramManager.OpenDiagrams);
			Assert.Equal(editor.Object, _diagramManager.OpenDiagrams.Single());
			Assert.Equal(editor.Object, _diagramManager.OpenDiagram);
		}

		private DiagramManagerViewModel _diagramManager;

		private readonly Mock<IDiagramExplorer> explorer = new Mock<IDiagramExplorer>();

		private static readonly FileInfo testDiagramFile = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestDiagrams\class.puml"));
	}
}