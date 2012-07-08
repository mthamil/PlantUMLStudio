using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using Moq;
using PlantUmlEditor.Model;
using PlantUmlEditor.ViewModel;
using Utilities.Chronology;
using Utilities.Concurrency;
using Utilities.Mvvm;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class DiagramEditorViewModelTests
	{
		public DiagramEditorViewModelTests()
		{
			autoSaveTimer.SetupProperty(t => t.Interval);
			previewDiagram = new PreviewDiagramViewModel(diagram);
		}

		[Fact]
		[Synchronous]
		public void Test_AutoSave_Enabled_ContentIsModified()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.IsModified = true;

			// Act.
			editor.AutoSave = true;

			// Assert.
			Assert.True(editor.AutoSave);
			autoSaveTimer.Verify(t => t.TryStart(), Times.Once());
		}

		[Fact]
		[Synchronous]
		public void Test_AutoSave_Enabled_ContentIsNotModified()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.IsModified = false;

			// Act.
			editor.AutoSave = true;

			// Assert.
			Assert.True(editor.AutoSave);
			autoSaveTimer.Verify(t => t.TryStart(), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_AutoSave_Disabled()
		{
			// Arrange.
			editor = CreateEditor();
			editor.AutoSave = true;

			// Act.
			editor.AutoSave = false;

			// Assert.
			Assert.False(editor.AutoSave);
			autoSaveTimer.Verify(t => t.TryStop(), Times.Once());
		}

		[Fact]
		[Synchronous]
		public void Test_AutoSaveInterval()
		{
			// Arrange.
			editor = CreateEditor();

			// Act.
			editor.AutoSaveInterval = TimeSpan.FromSeconds(7);

			// Assert.
			Assert.Equal(TimeSpan.FromSeconds(7), editor.AutoSaveInterval);
			Assert.Equal(TimeSpan.FromSeconds(7), autoSaveTimer.Object.Interval);
		}

		[Theory]
		[Synchronous]
		[InlineData(true, true, true)]
		[InlineData(false, false, true)]
		[InlineData(false, true, false)]
		[InlineData(false, false, false)]
		public void Test_CanSave(bool expected, bool isIdle, bool isModified)
		{
			// Arrange.
			editor = CreateEditor();
			editor.IsIdle = isIdle;

			codeEditor.IsModified = isModified;

			// Act.
			bool actual = editor.CanSave;

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[Synchronous]
		[InlineData(true, true)]
		[InlineData(false, false)]
		public void Test_CanRefresh(bool expected, bool isIdle)
		{
			// Arrange.
			editor = CreateEditor();
			editor.IsIdle = isIdle;

			// Act.
			bool actual = editor.CanRefresh;

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[Synchronous]
		[InlineData(true, true)]
		[InlineData(false, false)]
		public void Test_CanClose(bool expected, bool isIdle)
		{
			// Arrange.
			editor = CreateEditor();
			editor.IsIdle = isIdle;

			// Act.
			bool actual = editor.CanClose;

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Fact]
		[Synchronous]
		public void Test_CodeEditor_IsModified_ChangedToTrue()
		{
			// Arrange.
			editor = CreateEditor();
			editor.AutoSave = true;

			// Act.
			codeEditor.IsModified = true;

			// Assert.
			Assert.True(editor.CanSave);
			autoSaveTimer.Verify(t => t.TryStart());
		}

		[Fact]
		[Synchronous]
		public void Test_CodeEditor_IsModified_ChangedToFalse()
		{
			// Arrange.
			editor = CreateEditor();
			editor.AutoSave = true;

			codeEditor.IsModified = true;

			// Act.
			codeEditor.IsModified = false;

			// Assert.
			Assert.False(editor.CanSave);
			autoSaveTimer.Verify(t => t.TryStop());
		}

		[Fact]
		[Synchronous]
		public void Test_CloseCommand()
		{
			// Arrange.
			editor = CreateEditor();

			bool closed = false;
			EventHandler closeHandler = (o, e) => closed = true;
			editor.Closed += closeHandler;

			// Act.
			editor.CloseCommand.Execute(null);

			// Assert.
			Assert.True(closed);
		}

		[Fact]
		[Synchronous]
		public void Test_SaveCommand_SaveSuccessful()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.Content = "Blah blah blah";

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromSuccess());

			compiler.Setup(c => c.CompileToFile(It.IsAny<FileInfo>()))
				.Returns(Tasks.FromSuccess());

			// Act.
			editor.SaveCommand.Execute(null);

			// Assert.
			Assert.True(editor.IsIdle);
			Assert.False(codeEditor.IsModified);
			Assert.Equal(codeEditor.Content, diagram.Content);
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, true));
		}

		[Fact]
		[Synchronous]
		public void Test_SaveCommand_SavedAgain()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.Content = "Blah blah blah";

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromSuccess());

			compiler.Setup(c => c.CompileToFile(It.IsAny<FileInfo>()))
				.Returns(Tasks.FromSuccess());

			editor.SaveCommand.Execute(null);

			// Act.
			editor.SaveCommand.Execute(null);

			// Assert.
			Assert.True(editor.IsIdle);
			Assert.False(codeEditor.IsModified);
			Assert.Equal(codeEditor.Content, diagram.Content);
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, false));
		}

		[Fact]
		[Synchronous]
		public void Test_SaveCommand_SaveUnuccessful()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.Content = "Blah blah blah";

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromException(new AggregateException()));

			// Act.
			editor.SaveCommand.Execute(null);

			// Assert.
			Assert.True(editor.IsIdle);
			Assert.True(codeEditor.IsModified);
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, true));
		}

		[Fact]
		[Synchronous]
		public void Test_RefreshCommand_Successful()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.Content = "Diagram code goes here";
			var result = new BitmapImage();

			compiler.Setup(c => c.CompileToImage(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns(Tasks.FromResult<BitmapSource>(result));

			// Act.
			editor.RefreshCommand.Execute(null);

			// Assert.
			Assert.Equal(result, editor.DiagramImage);
		}

		[Fact]
		[Synchronous]
		public void Test_RefreshCommand_Unsuccessful()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.Content = "Diagram code goes here";

			compiler.Setup(c => c.CompileToImage(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns(Tasks.FromException<BitmapSource, InvalidOperationException>(new InvalidOperationException()));

			// Act.
			editor.RefreshCommand.Execute(null);

			// Assert.
			Assert.Null(editor.DiagramImage);
		}

		[Fact]
		[Synchronous]
		public void Test_Dispose()
		{
			// Arrange.
			var disposableTimer = autoSaveTimer.As<IDisposable>();

			editor = CreateEditor();

			// Act.
			editor.Dispose();

			// Assert.
			disposableTimer.Verify(t => t.Dispose());
		}

		private DiagramEditorViewModel CreateEditor()
		{
			return new DiagramEditorViewModel(previewDiagram, codeEditor, progress.Object, renderer.Object,
											  diagramIO.Object, compiler.Object, autoSaveTimer.Object, refreshTimer.Object);
		}


		private DiagramEditorViewModel editor;

		private readonly Diagram diagram = new Diagram();
		private readonly PreviewDiagramViewModel previewDiagram;

		private readonly CodeEditorViewModel codeEditor = new CodeEditorViewModel(Enumerable.Empty<ViewModelBase>());
		private readonly Mock<IProgressViewModel> progress = new Mock<IProgressViewModel>();
		private readonly Mock<IDiagramRenderer> renderer = new Mock<IDiagramRenderer>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();
		private readonly Mock<IDiagramCompiler> compiler = new Mock<IDiagramCompiler>();
		private readonly Mock<ITimer> autoSaveTimer = new Mock<ITimer>();
		private readonly Mock<ITimer> refreshTimer = new Mock<ITimer>();
	}
}