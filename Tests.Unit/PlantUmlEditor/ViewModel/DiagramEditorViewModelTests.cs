using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac.Features.Indexed;
using Moq;
using PlantUmlEditor.Core;
using PlantUmlEditor.Core.Imaging;
using PlantUmlEditor.Core.InputOutput;
using PlantUmlEditor.ViewModel;
using PlantUmlEditor.ViewModel.Notifications;
using Utilities.Chronology;
using Utilities.Concurrency;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PlantUmlEditor.ViewModel
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
		public void Test_SaveCommand_SaveSuccessful()
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
			         .Returns(Tasks.FromSuccess());

			compiler.Setup(c => c.CompileToFileAsync(It.IsAny<FileInfo>(), It.IsAny<ImageFormat>()))
			        .Returns(Tasks.FromSuccess());

			editor.ImageFormat = ImageFormat.SVG;

			// Act.
			editor.SaveCommand.Execute(null);

			// Assert.
			Assert.False(codeEditor.Object.IsModified);
			Assert.Equal(codeEditor.Object.Content, diagram.Content);
			Assert.Equal(ImageFormat.SVG, diagram.ImageFormat);	// Make sure image format was updated.
			autoSaveTimer.Verify(t => t.TryStop());
			diagramIO.Verify(dio => dio.SaveAsync(diagram, true));
		}

		[Fact]
		public void Test_SaveCommand_SaveUnuccessful()
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
			         .Returns(Tasks.FromSuccess());

			compiler.Setup(c => c.CompileToFileAsync(It.IsAny<FileInfo>(), It.IsAny<ImageFormat>()))
			        .Returns(Tasks.FromSuccess());

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
					 .Returns(Tasks.FromSuccess());

			compiler.Setup(c => c.CompileToFileAsync(It.IsAny<FileInfo>(), It.IsAny<ImageFormat>()))
					.Returns(Tasks.FromSuccess());

			// Act.
			await editor.SaveAsync();

			// Assert.
			Assert.Equal("image2.svg", diagram.ImageFile.Name);
		}

		[Fact]
		public void Test_RefreshCommand_Successful()
		{
			// Arrange.
			editor = CreateEditor();
			codeEditor.SetupProperty(ce => ce.Content);

			codeEditor.Object.Content = "Diagram code goes here";
			var result = new BitmapImage();

			compiler.Setup(c => c.CompileToImageAsync(It.IsAny<string>(), It.IsAny<ImageFormat>(), It.IsAny<CancellationToken>()))
			        .Returns(Task.FromResult<ImageSource>(result));

			// Act.
			editor.RefreshCommand.Execute(null);

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

			compiler.Setup(c => c.CompileToImageAsync(It.IsAny<string>(), It.IsAny<ImageFormat>(), It.IsAny<CancellationToken>()))
					.Returns(Tasks.FromException<ImageSource>(new PlantUmlException()));

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

			compiler.Setup(c => c.CompileToImageAsync(It.IsAny<string>(), It.IsAny<ImageFormat>(), It.IsAny<CancellationToken>()))
			        .Returns(Tasks.FromCanceled<ImageSource>());

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
			compiler.Verify(c => c.CompileToImageAsync(It.IsAny<string>(), ImageFormat.SVG, It.IsAny<CancellationToken>()));
		}

		[Fact]
		[Synchronous]
		public void Test_ImageFormat_Change_CancelsOtherRefreshTasks()
		{
			// Arrange.
			diagram.File = new FileInfo("TestFile.puml");
			diagram.ImageFile = new FileInfo("image.png");

			bool killSwitch = false;
			var tasks = new List<Task>(2);
			compiler.Setup(c => c.CompileToImageAsync(It.IsAny<string>(), It.IsAny<ImageFormat>(), It.IsAny<CancellationToken>()))
			        .Returns((string content, ImageFormat format, CancellationToken token) =>
			        {
						var task = Task.Run(() => 
						{ 
							while (!killSwitch) 
							{ 
								token.ThrowIfCancellationRequested();
								Thread.Sleep(100); 
							}
							return (ImageSource)null;
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
			AssertThat.Throws<OperationCanceledException>(tasks[0]);
			Assert.Equal(TaskStatus.Canceled, tasks[0].Status);

			killSwitch = true;
			AssertThat.DoesNotThrow(tasks[1]);
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
		private readonly Mock<IIndex<ImageFormat, IDiagramRenderer>> renderers = new Mock<IIndex<ImageFormat, IDiagramRenderer>>();
		private readonly Mock<IDiagramRenderer> renderer = new Mock<IDiagramRenderer>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();
		private readonly Mock<IDiagramCompiler> compiler = new Mock<IDiagramCompiler>();
		private readonly Mock<ITimer> autoSaveTimer = new Mock<ITimer>();
		private readonly Mock<ITimer> refreshTimer = new Mock<ITimer>();
		private readonly TaskScheduler scheduler = new SynchronousTaskScheduler();
	}
}