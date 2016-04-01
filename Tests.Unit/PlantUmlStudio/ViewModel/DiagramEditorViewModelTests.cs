using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Moq;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.Imaging;
using PlantUmlStudio.Core.InputOutput;
using PlantUmlStudio.ViewModel;
using PlantUmlStudio.ViewModel.Notifications;
using SharpEssentials.Chronology;
using SharpEssentials.Concurrency;
using SharpEssentials.Testing;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.ViewModel
{
	public class DiagramEditorViewModelTests
	{
		public DiagramEditorViewModelTests()
		{
			autoSaveTimer.SetupProperty(t => t.Interval);

			notifications.Setup(p => p.StartProgress(It.IsAny<bool>()))
			             .Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);

			renderers.Setup(r => r[It.IsAny<ImageFormat>()])
			         .Returns(renderer.Object);
		}

		[Fact]
		public void Test_Initialization()
		{
			// Arrange.
			diagram.Content = "Test content";
			diagram.ImageFile = new FileInfo("image.svg");

			codeEditor.SetupProperty(c => c.Content);

			// Act.
			editor = CreateEditor();

			// Assert.
			Assert.Equal("Test content", editor.CodeEditor.Content);
			Assert.Equal(ImageFormat.SVG, editor.ImageFormat);
		}

		[Fact]
		public void Test_AutoSave_Enabled_ContentIsModified()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.SetupGet(ce => ce.IsModified).Returns(true);

			// Act.
			editor.AutoSave = true;

			// Assert.
			Assert.True(editor.AutoSave);
			autoSaveTimer.Verify(t => t.TryStart(It.IsAny<object>()), Times.Once());
		}

		[Fact]
		public void Test_AutoSave_Enabled_ContentIsNotModified()
		{
			// Arrange.
			editor = CreateEditor();

			codeEditor.SetupGet(ce => ce.IsModified).Returns(false);

			// Act.
			editor.AutoSave = true;

			// Assert.
			Assert.True(editor.AutoSave);
			autoSaveTimer.Verify(t => t.TryStart(It.IsAny<object>()), Times.Never());
		}

		[Fact]
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

		[Theory]
		[InlineData(true, true, true)]
		[InlineData(false, false, true)]
		[InlineData(false, true, false)]
		public void Test_AutoSaveInterval(bool expectRestart, bool isAutoSaveEnabled, bool isAutoSaveTimerStarted)
		{
			// Arrange.
			editor = CreateEditor();
			editor.AutoSave = isAutoSaveEnabled;
			autoSaveTimer.SetupGet(t => t.Started).Returns(isAutoSaveTimerStarted);

			// Act.
			editor.AutoSaveInterval = TimeSpan.FromSeconds(7);

			// Assert.
			Assert.Equal(TimeSpan.FromSeconds(7), editor.AutoSaveInterval);
			Assert.Equal(TimeSpan.FromSeconds(7), autoSaveTimer.Object.Interval);
			autoSaveTimer.Verify(t => t.Restart(It.IsAny<object>()), expectRestart ? Times.Exactly(1) : Times.Never());
		}

		[Theory]
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
			autoSaveTimer.Verify(t => t.TryStart(It.IsAny<object>()));
		}

		[Fact]
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
		public async Task Test_SaveCommand_SaveSuccessful()
		{
			// Arrange.
			editor = CreateEditor();
			notifications.Setup(p => p.StartProgress(It.IsAny<bool>())).Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
			codeEditor.SetupProperty(ce => ce.IsModified);
			codeEditor.SetupProperty(ce => ce.Content);

			diagram.File = new FileInfo("TestFile.puml");
			diagram.ImageFile = new FileInfo("image.png");
			codeEditor.Object.Content = "Blah blah blah";
			codeEditor.Object.IsModified = true;

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
			         .Returns(Task.CompletedTask);

			compiler.Setup(c => c.CompileToFileAsync(It.IsAny<FileInfo>(), It.IsAny<ImageFormat>(), It.IsAny<Encoding>()))
			        .Returns(Task.CompletedTask);

			editor.ImageFormat = ImageFormat.SVG;

			// Act.
			await editor.SaveCommand.ExecuteAsync(null);

			// Assert.
			Assert.False(codeEditor.Object.IsModified);
			Assert.Equal(codeEditor.Object.Content, diagram.Content);
			Assert.Equal(ImageFormat.SVG, diagram.ImageFormat);	// Make sure image format was updated.
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, true));
		}

		[Fact]
		public async Task Test_SaveCommand_SaveUnuccessful()
		{
			// Arrange.
			editor = CreateEditor();
			notifications.Setup(p => p.StartProgress(It.IsAny<bool>())).Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
			codeEditor.SetupProperty(ce => ce.IsModified);
			codeEditor.SetupProperty(ce => ce.Content);

			diagram.File = new FileInfo("TestFile.puml");
			codeEditor.Object.Content = "Blah blah blah";
			codeEditor.Object.IsModified = true;

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
					 .Returns(Task.FromException(new InvalidOperationException("Save didn't work!")));

			// Act.
            await Assert.ThrowsAsync<InvalidOperationException>(() => editor.SaveAsync());

			// Assert.
			Assert.True(editor.IsIdle);
			Assert.True(codeEditor.Object.IsModified);
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, true));
		}

		[Fact]
		public async Task Test_Save_Then_SavedAgain()
		{
			// Arrange.
			editor = CreateEditor();
			notifications.Setup(p => p.StartProgress(It.IsAny<bool>())).Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
			codeEditor.SetupProperty(ce => ce.IsModified);
			codeEditor.SetupProperty(ce => ce.Content);

			diagram.File = new FileInfo("TestFile.puml");
			codeEditor.Object.Content = "Blah blah blah";
			codeEditor.Object.IsModified = true;

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
			         .Returns(Task.CompletedTask);

			compiler.Setup(c => c.CompileToFileAsync(It.IsAny<FileInfo>(), It.IsAny<ImageFormat>(), It.IsAny<Encoding>()))
			        .Returns(Task.CompletedTask);

			await editor.SaveAsync();

			// Act.
			await editor.SaveAsync();

			// Assert.
			Assert.True(editor.IsIdle);
			Assert.False(codeEditor.Object.IsModified);
			Assert.Equal(codeEditor.Object.Content, diagram.Content);
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, false));
		}

		[Fact]
		public async Task Test_Save_Updates_ImageFile()
		{
			// Arrange.
			editor = CreateEditor();
			notifications.Setup(p => p.StartProgress(It.IsAny<bool>())).Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
			codeEditor.SetupProperty(ce => ce.IsModified);
			codeEditor.SetupProperty(ce => ce.Content);

			diagram.File = new FileInfo("TestFile.puml");
			diagram.ImageFile = new FileInfo("image.png");
			codeEditor.Object.Content = @"
				@startuml image2.svg

				title Class Diagram";
			codeEditor.Object.IsModified = true;

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
					 .Returns(Task.CompletedTask);

			compiler.Setup(c => c.CompileToFileAsync(It.IsAny<FileInfo>(), It.IsAny<ImageFormat>(), It.IsAny<Encoding>()))
					.Returns(Task.CompletedTask);

			// Act.
			await editor.SaveAsync();

			// Assert.
			Assert.Equal("image2.svg", diagram.ImageFile.Name);
		}

		[Fact]
		public async Task Test_RefreshCommand_Successful()
		{
			// Arrange.
			editor = CreateEditor();
			codeEditor.SetupProperty(ce => ce.Content);

			codeEditor.Object.Content = "Diagram code goes here";
			var result = new BitmapImage();

			compiler.Setup(c => c.CompileToImageAsync(It.IsAny<string>(), It.IsAny<ImageFormat>(), It.IsAny<Encoding>(), It.IsAny<CancellationToken>()))
			        .Returns(Task.FromResult(new DiagramResult(result)));

			// Act.
			await editor.RefreshCommand.ExecuteAsync(null);

			// Assert.
			Assert.Equal(result, editor.DiagramImage);
		}

		[Fact]
		public async Task Test_Refresh_Unsuccessful()
		{
			// Arrange.
			editor = CreateEditor();
			codeEditor.SetupProperty(ce => ce.Content);

			codeEditor.Object.Content = "Diagram code goes here";

			compiler.Setup(c => c.CompileToImageAsync(It.IsAny<string>(), It.IsAny<ImageFormat>(), It.IsAny<Encoding>(), It.IsAny<CancellationToken>()))
					.ThrowsAsync(new PlantUmlException());

			// Act.
			await editor.RefreshAsync();

			// Assert.
			Assert.Null(editor.DiagramImage);
		}

		[Fact]
		public async Task Test_Refresh_Canceled()
		{
			// Arrange.
			editor = CreateEditor();
			codeEditor.SetupProperty(ce => ce.Content);

			codeEditor.Object.Content = "Diagram code goes here";

			compiler.Setup(c => c.CompileToImageAsync(It.IsAny<string>(), It.IsAny<ImageFormat>(), It.IsAny<Encoding>(), It.IsAny<CancellationToken>()))
			        .Returns(Task.FromCanceled<DiagramResult>(new CancellationToken(true)));

			// Act.
			await editor.RefreshAsync();

			// Assert.
			Assert.Null(editor.DiagramImage);
		}

		[Fact]
		public void Test_ImageFormat_Change_TriggersRefresh()
		{
			// Arrange.
			diagram.File = new FileInfo("TestFile.puml");
			diagram.ImageFile = new FileInfo("image.png");

			editor = CreateEditor();

			// Act.
			editor.ImageFormat = ImageFormat.SVG;

			// Assert.
			compiler.Verify(c => c.CompileToImageAsync(It.IsAny<string>(), ImageFormat.SVG, It.IsAny<Encoding>(), It.IsAny<CancellationToken>()));
		}

		[Fact]
		[Synchronous]
		public async Task Test_ImageFormat_Change_CancelsOtherRefreshTasks()
		{
			// Arrange.
			diagram.File = new FileInfo("TestFile.puml");
			diagram.ImageFile = new FileInfo("image.png");

			bool killSwitch = false;
			var tasks = new List<Task>(2);
			compiler.Setup(c => c.CompileToImageAsync(It.IsAny<string>(), It.IsAny<ImageFormat>(), It.IsAny<Encoding>(), It.IsAny<CancellationToken>()))
			        .Returns((string content, ImageFormat format, Encoding encoding, CancellationToken token) =>
			        {
						var task = Task.Run(() => 
						{ 
							while (!killSwitch) 
							{ 
								token.ThrowIfCancellationRequested();
								Thread.Sleep(100); 
							}
							return new DiagramResult(Enumerable.Empty<DiagramError>());
						}, token);
						tasks.Add(task);
				        return task;
			        });

			editor = CreateEditor();
			editor.RefreshAsync();

			// Act.
			editor.ImageFormat = ImageFormat.SVG;

			// Assert.
			Assert.Equal(2, tasks.Count);
			await Assert.ThrowsAnyAsync<OperationCanceledException>(() => tasks[0]);
			Assert.Equal(TaskStatus.Canceled, tasks[0].Status);

			killSwitch = true;
			await tasks[1];
			Assert.Equal(TaskStatus.RanToCompletion, tasks[1].Status);
		}

		[Fact]
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
			return new DiagramEditorViewModel(diagram, codeEditor.Object, notifications.Object, renderers.Object,
											  diagramIO.Object, compiler.Object, autoSaveTimer.Object, refreshTimer.Object, scheduler);
		}


		private DiagramEditorViewModel editor;

		private readonly Diagram diagram = new Diagram();

		private readonly Mock<ICodeEditor> codeEditor = new Mock<ICodeEditor>();
		private readonly Mock<INotifications> notifications = new Mock<INotifications>();
		private readonly Mock<IReadOnlyDictionary<ImageFormat, IDiagramRenderer>> renderers = new Mock<IReadOnlyDictionary<ImageFormat, IDiagramRenderer>>();
		private readonly Mock<IDiagramRenderer> renderer = new Mock<IDiagramRenderer>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();
		private readonly Mock<IDiagramCompiler> compiler = new Mock<IDiagramCompiler>();
		private readonly Mock<ITimer> autoSaveTimer = new Mock<ITimer>();
		private readonly Mock<ITimer> refreshTimer = new Mock<ITimer>();
		private readonly TaskScheduler scheduler = new SynchronousTaskScheduler();
	}
}