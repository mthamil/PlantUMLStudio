using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using Moq;
using PlantUmlEditor.Core;
using PlantUmlEditor.Core.InputOutput;
using PlantUmlEditor.Model;
using PlantUmlEditor.ViewModel;
using Utilities.Chronology;
using Utilities.Concurrency;
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

			progress.Setup(p => p.New(It.IsAny<bool>()))
				.Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
		}

		[Fact]
		[Synchronous]
		public void Test_AutoSave_Enabled_ContentIsModified()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.SetupGet(ce => ce.IsModified).Returns(true);

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

			codeEditor.SetupGet(ce => ce.IsModified).Returns(false);

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

			codeEditor.SetupGet(ce => ce.IsModified).Returns(isModified);

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
			codeEditor.SetupGet(ce => ce.IsModified).Returns(true);
			codeEditor.Raise(ce => ce.PropertyChanged += null, new PropertyChangedEventArgs("IsModified"));

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

			// Act.
			codeEditor.SetupGet(ce => ce.IsModified).Returns(false);
			codeEditor.Raise(ce => ce.PropertyChanged += null, new PropertyChangedEventArgs("IsModified"));

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

			bool closing = false;
			CancelEventHandler closingHandler = (o, e) => closing = true;
			editor.Closing += closingHandler;

			bool closed = false;
			EventHandler closeHandler = (o, e) => closed = true;
			editor.Closed += closeHandler;

			// Act.
			editor.CloseCommand.Execute(null);

			// Assert.
			Assert.True(closing);
			Assert.True(closed);
			refreshTimer.Verify(t => t.TryStop());
			autoSaveTimer.Verify(t => t.TryStop());
		}

		[Fact]
		[Synchronous]
		public void Test_CloseCommand_Cancelled()
		{
			// Arrange.
			editor = CreateEditor();

			bool closing = false;
			CancelEventHandler closingHandler = (o, e) =>
			{
				e.Cancel = true;
				closing = true;
			};
			editor.Closing += closingHandler;

			bool closed = false;
			EventHandler closeHandler = (o, e) => closed = true;
			editor.Closed += closeHandler;

			// Act.
			editor.CloseCommand.Execute(null);

			// Assert.
			Assert.True(closing);
			Assert.False(closed);
			refreshTimer.Verify(t => t.TryStop(), Times.Never());
			autoSaveTimer.Verify(t => t.TryStop(), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_SaveCommand_SaveSuccessful()
		{
			// Arrange.
			editor = CreateEditor();
			progress.Setup(p => p.New(It.IsAny<bool>())).Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
			codeEditor.SetupProperty(ce => ce.IsModified);
			codeEditor.SetupProperty(ce => ce.Content);

			diagram.File = new FileInfo("TestFile.puml");
			codeEditor.Object.Content = "Blah blah blah";
			codeEditor.Object.IsModified = true;

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromSuccess());

			compiler.Setup(c => c.CompileToFile(It.IsAny<FileInfo>()))
				.Returns(Tasks.FromSuccess());

			// Act.
			editor.SaveCommand.Execute(null);

			// Assert.
			Assert.False(codeEditor.Object.IsModified);
			Assert.Equal(codeEditor.Object.Content, diagram.Content);
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, true));
		}

		[Fact]
		[Synchronous]
		public void Test_SaveCommand_SavedAgain()
		{
			// Arrange.
			editor = CreateEditor();
			progress.Setup(p => p.New(It.IsAny<bool>())).Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
			codeEditor.SetupProperty(ce => ce.IsModified);
			codeEditor.SetupProperty(ce => ce.Content);

			diagram.File = new FileInfo("TestFile.puml");
			codeEditor.Object.Content = "Blah blah blah";
			codeEditor.Object.IsModified = true;

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromSuccess());

			compiler.Setup(c => c.CompileToFile(It.IsAny<FileInfo>()))
				.Returns(Tasks.FromSuccess());

			editor.SaveCommand.Execute(null);

			// Act.
			editor.SaveCommand.Execute(null);

			// Assert.
			Assert.True(editor.IsIdle);
			Assert.False(codeEditor.Object.IsModified);
			Assert.Equal(codeEditor.Object.Content, diagram.Content);
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, false));
		}

		[Fact]
		[Synchronous]
		public void Test_SaveCommand_SaveUnuccessful()
		{
			// Arrange.
			editor = CreateEditor();
			progress.Setup(p => p.New(It.IsAny<bool>())).Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
			codeEditor.SetupProperty(ce => ce.IsModified);
			codeEditor.SetupProperty(ce => ce.Content);

			diagram.File = new FileInfo("TestFile.puml");
			codeEditor.Object.Content = "Blah blah blah";
			codeEditor.Object.IsModified = true;

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromException(new InvalidOperationException()));

			// Act.
			editor.SaveCommand.Execute(null);

			// Assert.
			Assert.True(editor.IsIdle);
			Assert.True(codeEditor.Object.IsModified);
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, true));
		}

		[Fact]
		[Synchronous]
		public void Test_RefreshCommand_Successful()
		{
			// Arrange.
			editor = CreateEditor();
			codeEditor.SetupProperty(ce => ce.Content);

			codeEditor.Object.Content = "Diagram code goes here";
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
			codeEditor.SetupProperty(ce => ce.Content);

			codeEditor.Object.Content = "Diagram code goes here";

			compiler.Setup(c => c.CompileToImage(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns(Tasks.FromException<BitmapSource, InvalidOperationException>(new InvalidOperationException()));

			// Act.
			editor.RefreshCommand.Execute(null);

			// Assert.
			Assert.Null(editor.DiagramImage);
		}

		[Fact]
		[Synchronous]
		public void Test_RefreshCommand_Canceled()
		{
			// Arrange.
			editor = CreateEditor();
			codeEditor.SetupProperty(ce => ce.Content);

			codeEditor.Object.Content = "Diagram code goes here";

			compiler.Setup(c => c.CompileToImage(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns(Tasks.FromCanceled<BitmapSource>());

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
			var disposableSaveTimer = autoSaveTimer.As<IDisposable>();
			var disposableRefreshTimer = refreshTimer.As<IDisposable>();
			var disposableCodeEditor = codeEditor.As<IDisposable>();

			editor = CreateEditor();

			// Act.
			editor.Dispose();

			// Assert.
			refreshTimer.Verify(t => t.TryStop());
			disposableRefreshTimer.Verify(t => t.Dispose());
			autoSaveTimer.Verify(t => t.TryStop());
			disposableSaveTimer.Verify(t => t.Dispose());
			disposableCodeEditor.Verify(ce => ce.Dispose());
		}

		private DiagramEditorViewModel CreateEditor()
		{
			return new DiagramEditorViewModel(previewDiagram, codeEditor.Object, progress.Object, renderer.Object,
											  diagramIO.Object, compiler.Object, autoSaveTimer.Object, refreshTimer.Object);
		}


		private DiagramEditorViewModel editor;

		private readonly Diagram diagram = new Diagram();
		private readonly PreviewDiagramViewModel previewDiagram;

		private readonly Mock<ICodeEditor> codeEditor = new Mock<ICodeEditor>();
		private readonly Mock<IProgressRegistration> progress = new Mock<IProgressRegistration>();
		private readonly Mock<IDiagramRenderer> renderer = new Mock<IDiagramRenderer>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();
		private readonly Mock<IDiagramCompiler> compiler = new Mock<IDiagramCompiler>();
		private readonly Mock<ITimer> autoSaveTimer = new Mock<ITimer>();
		private readonly Mock<ITimer> refreshTimer = new Mock<ITimer>();
	}
}