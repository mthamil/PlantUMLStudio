using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.Core.Imaging;
using PlantUmlEditor.ViewModel;
using Utilities.Collections;
using Utilities.Concurrency;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PlantUmlEditor.ViewModel
{
	public class DiagramManagerViewModelTests
	{
		public DiagramManagerViewModelTests()
		{
			var previews = new List<PreviewDiagramViewModel>();
			explorer.SetupGet(p => p.PreviewDiagrams).Returns(previews);
		}

		[Fact]
		public void Test_OpenDiagramCommand()
		{
			// Arrange.
			var diagramPreview = new PreviewDiagramViewModel(new Diagram { File = testDiagramFile });
			var editor = Mock.Of<IDiagramEditor>();

			var diagramManager = CreateManager(d => editor);

			// Act.
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Assert.
			Assert.Single(diagramManager.OpenDiagrams);
			Assert.Equal(editor, diagramManager.OpenDiagram);
		}

		[Fact]
		public void Test_OpenDiagramCommand_DiagramAlreadyOpenedOnce()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var diagramPreview = new PreviewDiagramViewModel(diagram);
			var editor = Mock.Of<IDiagramEditor>(e => e.Diagram == diagram);

			var diagramManager = CreateManager(null);
			diagramManager.OpenDiagrams.Add(editor);

			// Act.
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Assert.
			Assert.Single(diagramManager.OpenDiagrams);
			Assert.Equal(editor, diagramManager.OpenDiagram);
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

			DiagramClosedEventArgs closedArgs = null;
			diagramManager.DiagramClosed += (o, e) => closedArgs = e;

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagramManager.OpenDiagrams);
			editor.Verify(e => e.SaveAsync(), Times.Never());
			editor.Verify(e => e.Dispose());
			Assert.NotNull(closedArgs);
			Assert.Equal(diagram, closedArgs.Diagram);
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

			// Arrange.
			DiagramClosedEventArgs closedArgs = null;
			diagramManager.DiagramClosed += (o, e) => closedArgs = e;

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagramManager.OpenDiagrams);
			editor.Verify(e => e.SaveAsync(), Times.Exactly(1));
			editor.Verify(e => e.Dispose());
			Assert.NotNull(closedArgs);
			Assert.Equal(diagram, closedArgs.Diagram);
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
			var diagramMap = new Dictionary<Diagram, IDiagramEditor>();

			var diagramManager = CreateManager(d => diagramMap[d]);

			var files = new[] { testDiagramFile, new FileInfo(testDiagramFile.FullName + "2") };
			var editors = files.Select(file => new Diagram { File = file })
							   .Select(diagram => Mock.Of<IDiagramEditor>(e => e.Diagram == diagram))
							   .Tee(editor => diagramMap[editor.Diagram] = editor)
							   .Select(editor => new { Editor = editor, Preview = new PreviewDiagramViewModel(editor.Diagram) })
							   .Tee(ep => diagramManager.OpenDiagramCommand.Execute(ep.Preview))
							   .Select(ep => ep.Editor).ToList();

			// Act.
			Mock.Get(editors.Last()).Raise(e => e.Closing += null, new CancelEventArgs());
			diagramManager.SaveClosingDiagramCommand.Execute(null);

			// Assert.
			Assert.Equal(editors.Last(), diagramManager.ClosingDiagram);

			// Act.
			Mock.Get(editors.First()).Raise(e => e.Closed += null, EventArgs.Empty);	// Raise the closed event for a different editor.

			// Assert.
			Assert.Single(diagramManager.OpenDiagrams);
			Assert.Equal(editors.Last(), diagramManager.OpenDiagrams.Single());
			foreach (var editor in editors)
				Mock.Get(editor).Verify(e => e.SaveAsync(), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditor_Saved()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var image = new BitmapImage();

			var diagramPreview = new PreviewDiagramViewModel(diagram);

			var editor = Mock.Of<IDiagramEditor>(e => e.Diagram == diagram);
			Mock.Get(editor).SetupGet(e => e.DiagramImage).Returns(image);

			var diagramManager = CreateManager(d => editor);
			diagramManager.Explorer.PreviewDiagrams.Add(diagramPreview);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			Mock.Get(editor).Raise(e => e.Saved += null, EventArgs.Empty);

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

			var editor = Mock.Of<IDiagramEditor>(e => e.Diagram == new Diagram { File = testDiagramFile });
			Mock.Get(editor).SetupGet(e => e.DiagramImage).Returns(image);

			var diagramManager = CreateManager(d => editor);
			diagramManager.Explorer.PreviewDiagrams.Add(diagramPreview);
			diagramManager.OpenDiagramCommand.Execute(diagramPreview);

			// Act.
			Mock.Get(editor).Raise(e => e.Saved += null, EventArgs.Empty);

			// Assert.
			Assert.Null(diagramPreview.ImagePreview);
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditor_Saved_UpdatesPreviewDiagram()
		{
			// Arrange.
			var image = new BitmapImage();

			var preview = new PreviewDiagramViewModel(new Diagram 
			{ 
				Content = "blah",
				File = new FileInfo("diagram.puml"),
				ImageFile = new FileInfo("image.png")
			});

			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.Diagram).Returns(new Diagram
			{
				Content = "blah-blah",
				File = new FileInfo("diagram.puml"),
				ImageFile = new FileInfo("image2.svg")
			});
			editor.SetupGet(e => e.DiagramImage).Returns(image);

			var diagramManager = CreateManager(d => editor.Object);
			diagramManager.Explorer.PreviewDiagrams.Add(preview);
			diagramManager.OpenDiagramCommand.Execute(preview);

			// Act.
			editor.Raise(e => e.Saved += null, EventArgs.Empty);

			// Assert.
			Assert.Equal(image, preview.ImagePreview);
			Assert.Equal("blah-blah", preview.Diagram.Content);
			Assert.Equal(ImageFormat.SVG, preview.Diagram.ImageFormat);
			Assert.Equal("image2.svg", preview.Diagram.ImageFile.Name);
		}

		[Fact]
		[Synchronous]
		public void Test_OpenPreviewRequested()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };
			var diagramPreview = new PreviewDiagramViewModel(diagram);

			var editor = Mock.Of<IDiagramEditor>(e => e.Diagram == diagram);

			var diagramManager = CreateManager(d => editor);

			// Act.
			explorer.Raise(p => p.OpenPreviewRequested += null, new OpenPreviewRequestedEventArgs(diagramPreview));

			// Assert.
			Assert.Single(diagramManager.OpenDiagrams);
			Assert.Equal(editor, diagramManager.OpenDiagrams.Single());
			Assert.Equal(editor, diagramManager.OpenDiagram);
		}

		[Fact]
		[Synchronous]
		public void Test_CloseCommand_UnsavedDiagram()
		{
			// Arrange.
			var diagram = new Diagram { File = testDiagramFile };

			var unsavedEditor = Mock.Of<IDiagramEditor>(e => e.Diagram == diagram &&
			                                                 e.CodeEditor.IsModified == true);

			var editor = Mock.Of<IDiagramEditor>(e => e.Diagram == diagram &&
			                                          e.CodeEditor.IsModified == false);

			var diagramManager = CreateManager(d => unsavedEditor);
			diagramManager.OpenDiagrams.Add(unsavedEditor);
			diagramManager.OpenDiagrams.Add(editor);

			// Act.
			diagramManager.CloseCommand.Execute(null);

			// Assert.
			Mock.Get(unsavedEditor).Verify(c => c.Close());
			Mock.Get(editor).Verify(c => c.Close(), Times.Never());
		}

		[Fact]
		public void Test_SaveAllCommand()
		{
			// Arrange.
			var diagramManager = CreateManager(d => null);

			var modifiedEditors = Mocks.Of<IDiagramEditor>()
			                           .Where(e => e.CanSave == true &&
			                                       e.SaveAsync() == Tasks.FromSuccess())
			                           .Take(2)
									   .Tee(e => diagramManager.OpenDiagrams.Add(e))
									   .ToList();

			var unmodifiedEditor =
				Mock.Of<IDiagramEditor>(e => e.CanSave == false &&
				                             e.SaveAsync() == Tasks.FromSuccess());
				
			diagramManager.OpenDiagrams.Add(unmodifiedEditor);
			
			// Act.
			diagramManager.SaveAllCommand.Execute(null);

			// Assert.
			modifiedEditors.ForEach(e => Mock.Get(e).Verify(ed => ed.SaveAsync()));
			Mock.Get(unmodifiedEditor).Verify(e => e.SaveAsync(), Times.Never());
		}

		[Theory]
		[InlineData(true, new []{ true, true, true })]
		[InlineData(true, new[] { true, true, false })]
		[InlineData(true, new[] { true, false, false })]
		[InlineData(false, new[] { false, false, false })]
		public void Test_CanSaveAll(bool expected, bool[] modified)
		{
			// Arrange.
			var diagramManager = CreateManager(d => null);

			modified.Select(value => Mock.Of<IDiagramEditor>(e => e.CanSave == value))
			        .AddTo(diagramManager.OpenDiagrams);

			// Act.
			bool actual = diagramManager.SaveAllCommand.CanExecute(null);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Fact]
		[Synchronous]
		public void Test_Close_RemembersOpenFiles()
		{
			// Arrange.
			var diagramManager = CreateManager(d => null);

			var diagrams = new [] { "test1.puml", "test2.puml" };
			diagrams.Select(diagramName => new Diagram { File = new FileInfo(diagramName) })
					.Select(diagram => Mock.Of<IDiagramEditor>(e => e.Diagram == diagram &&
																	e.CodeEditor == Mock.Of<ICodeEditor>()))
					.AddTo(diagramManager.OpenDiagrams);	

			settings.SetupProperty(s => s.OpenFiles);
			settings.SetupProperty(s => s.RememberOpenFiles, true);

			// Act.
			diagramManager.CloseCommand.Execute(null);

			// Assert.
			Assert.NotNull(settings.Object.OpenFiles);
			Assert.Equal(diagrams.Length, settings.Object.OpenFiles.Count());
			foreach (var diagramName in diagrams)
				Assert.Contains(diagramName, settings.Object.OpenFiles.Select(f => f.Name));
		}

		[Fact]
		[Synchronous]
		public async Task Test_RememberedFiles_Reopened()
		{
			// Arrange.
			var diagrams = new[] { "test1.puml", "test2.puml" };

			settings.SetupProperty(s => s.RememberOpenFiles, true);
			settings.SetupProperty(s => s.OpenFiles, diagrams.Select(name => new FileInfo(name)).ToList());

			explorer.Setup(e => e.OpenDiagramAsync(It.IsAny<Uri>()))
			        .Returns((Uri uri) => Task.FromResult(new Diagram { File = new FileInfo(uri.AbsolutePath) }));

			var diagramManager = CreateManager(d => null);

			// Act.
			await diagramManager.InitializeAsync();

			// Assert.
			explorer.Verify(e => e.OpenDiagramAsync(It.IsAny<Uri>()), Times.Exactly(2));
			foreach (var diagram in diagrams)
				explorer.Verify(e => e.OpenDiagramAsync(It.Is<Uri>(uri => Path.GetFileName(uri.AbsolutePath) == diagram)));
		}

		private DiagramManagerViewModel CreateManager(Func<Diagram, IDiagramEditor> editorFactory)
		{
			return new DiagramManagerViewModel(explorer.Object, editorFactory, settings.Object);
		}

		private readonly Mock<IDiagramExplorer> explorer = new Mock<IDiagramExplorer>();
		private readonly Mock<ISettings> settings = new Mock<ISettings>();

		private static readonly FileInfo testDiagramFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestDiagrams\class.puml"));
	}
}