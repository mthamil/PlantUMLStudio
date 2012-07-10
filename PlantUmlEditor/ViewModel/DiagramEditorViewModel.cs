using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.Document;
using PlantUmlEditor.Model;
using Utilities.Chronology;
using Utilities.Concurrency;
using Utilities.Controls.Behaviors.AvalonEdit;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;
using Utilities.Reflection;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram editor.
	/// </summary>
	public class DiagramEditorViewModel : ViewModelBase, IUndoProvider, IDiagramEditor
	{
		/// <summary>
		/// Initializes a new diagram editor.
		/// </summary>
		/// <param name="previewDiagram">A preview of the diagram being edited</param>
		/// <param name="codeEditor">The code editor</param>
		/// <param name="progressViewModel">Reports progress of editor tasks</param>
		/// <param name="diagramRenderer">Converts existing diagram output files to images</param>
		/// <param name="diagramIO">Saves diagrams</param>
		/// <param name="compiler">Compiles diagrams</param>
		/// <param name="autoSaveTimer">Determines how soon after a change a diagram will be autosaved</param>
		/// <param name="refreshTimer">Determines how long after the last code modification was made to automatically refresh a diagram's image</param>
		public DiagramEditorViewModel(PreviewDiagramViewModel previewDiagram, CodeEditorViewModel codeEditor, IProgressViewModel progressViewModel,
			IDiagramRenderer diagramRenderer, IDiagramIOService diagramIO, IDiagramCompiler compiler, 
			ITimer autoSaveTimer, ITimer refreshTimer)
		{
			_diagram = Property.New(this, p => p.Diagram, OnPropertyChanged);
			Diagram = previewDiagram.Diagram;
			Progress = progressViewModel;

			_diagramRenderer = diagramRenderer;
			_diagramIO = diagramIO;
			_compiler = compiler;
			_autoSaveTimer = autoSaveTimer;
			_refreshTimer = refreshTimer;

			CodeEditor = codeEditor;
			CodeEditor.Content = Diagram.Content;
			CodeEditor.PropertyChanged += codeEditor_PropertyChanged;	// Subscribe after setting the content the first time.

			_diagramImage = Property.New(this, p => p.DiagramImage, OnPropertyChanged);
			DiagramImage = previewDiagram.ImagePreview;

			_isIdle = Property.New(this, p => p.IsIdle, OnPropertyChanged)
				.AlsoChanges(p => p.CanSave)
				.AlsoChanges(p => p.CanRefresh)
				.AlsoChanges(p => p.CanClose);
			IsIdle = true;

			_autoSave = Property.New(this, p => p.AutoSave, OnPropertyChanged);
			_autoSaveInterval = Property.New(this, p => p.AutoSaveInterval, OnPropertyChanged);

			_saveCommand = new BoundRelayCommand<DiagramEditorViewModel>(_ => Save(), p => p.CanSave, this);
			_refreshCommand = new BoundRelayCommand<DiagramEditorViewModel>(_ => Refresh(), p => p.CanRefresh, this);
			_closeCommand = new BoundRelayCommand<DiagramEditorViewModel>(_ => Close(), p => p.CanClose, this);

			// The document has been opened first time. So, any changes
			// made to the document will require creating a backup.
			_firstSaveAfterOpen = true;

			_autoSaveTimer.Elapsed += autoSaveTimerElapsed;
			_refreshTimer.Elapsed += refreshTimer_Elapsed;

			ImageCommands = new List<NamedOperationViewModel>
			{
				new NamedOperationViewModel("Copy to Clipboard", 
					new RelayCommand(() => Clipboard.SetImage(DiagramImage as BitmapSource))),	// Copy image.
				new NamedOperationViewModel("Open in Explorer", 
					new RelayCommand(() => Process.Start("explorer.exe","/select," + Diagram.ImageFilePath).Dispose())), // Open in explorer.
				new NamedOperationViewModel("Copy Image Path", 
					new RelayCommand(() => Clipboard.SetText(Diagram.ImageFilePath)))	// Copy image path.
			};
		}

		/// <summary>
		/// Whether an editor is currently busy with some task.
		/// </summary>
		public bool IsIdle
		{
			get { return _isIdle.Value; }
			set { _isIdle.Value = value; }
		}

		/// <summary>
		/// Whether to automatically save a diagram's changes and regenerate its image.
		/// </summary>
		public bool AutoSave
		{
			get { return _autoSave.Value; }
			set 
			{
				if (_autoSave.TrySetValue(value))
				{
					if (_autoSave.Value)
					{
						if (CodeEditor.IsModified)
							_autoSaveTimer.TryStart();
					}
					else
					{
						_autoSaveTimer.TryStop();
					}
				}
			}
		}

		/// <summary>
		/// The auto-save internval.
		/// </summary>
		public TimeSpan AutoSaveInterval
		{
			get { return _autoSaveInterval.Value; }
			set
			{
				if (_autoSaveInterval.TrySetValue(value))
					_autoSaveTimer.Interval = value;
			}
		}

		/// <summary>
		/// The underlying diagram.
		/// </summary>
		public Diagram Diagram
		{
			get { return _diagram.Value; }
			private set { _diagram.Value = value; }
		}

		/// <summary>
		/// The rendered diagram image.
		/// </summary>
		public ImageSource DiagramImage
		{
			get { return _diagramImage.Value; }
			set { _diagramImage.Value = value; }
		}

		/// <summary>
		/// Whether an editor's content can currently be saved.
		/// </summary>
		public bool CanSave
		{
			get { return CodeEditor.IsModified && IsIdle; }
		}

		/// <summary>
		/// Command that saves a diagram's changes.
		/// </summary>
		public ICommand SaveCommand
		{
			get { return _saveCommand; }
		}

		private void Save()
		{
			_autoSaveTimer.TryStop();

			if (_saveExecuting)
				return;

			_saveExecuting = true;
			IsIdle = false;
			foreach (var cts in _refreshCancellations)
				cts.Value.Cancel();

			// PlantUML seems to have a problem detecting encoding if the
			// first line is not an empty line.
			if (!Char.IsWhiteSpace(CodeEditor.Content, 0))
				CodeEditor.Content = Environment.NewLine + CodeEditor.Content;

			Diagram.Content = CodeEditor.Content;

			// Create a backup if this is the first time the diagram being modified
			// after opening.
			bool makeBackup = false;
			if (_firstSaveAfterOpen)
			{
				makeBackup = true;
				_firstSaveAfterOpen = false;
			}

			Progress.HasDiscreteProgress = false;
			IProgress<Tuple<int?, string>> progress = new Progress<Tuple<int?, string>>(p =>
			{
				Progress.PercentComplete = p.Item1;
				Progress.Message = p.Item2;
			});

			progress.Report(Tuple.Create((int?)100, "Saving and generating diagram..."));
			var saveTask = _diagramIO.SaveAsync(Diagram, makeBackup)
				.Then(() => _compiler.CompileToFile(Diagram.File));

			saveTask.ContinueWith(t =>
			{
				_saveExecuting = false;
				//DiagramViewModel.DiagramImage = null;	// Clear the old image first.
				DiagramImage = _diagramRenderer.Render(Diagram);
				CodeEditor.IsModified = false;
				IsIdle = true;
				_refreshTimer.TryStop();
				progress.Report(Tuple.Create((int?)null, "Saved."));

				OnSaved();
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);

			saveTask.ContinueWith(t =>
			{
				_saveExecuting = false;
				IsIdle = true;
				_refreshTimer.TryStop();
				if (t.Exception != null)
					progress.Report(Tuple.Create((int?)null, t.Exception.InnerException.Message));

			}, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, _uiScheduler);
		}

		void autoSaveTimerElapsed(object sender, EventArgs e)
		{
			// We must begin the Save operation on the UI thread in order to update the UI 
			// with pre-save state.
			Task.Factory.StartNew(Save, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
		}

		/// <summary>
		/// Whether a diagram's image can currently be refreshed.
		/// </summary>
		public bool CanRefresh
		{
			get { return IsIdle; }
		}

		/// <summary>
		/// Refreshes a diagram's image without saving.
		/// </summary>
		public ICommand RefreshCommand
		{
			get { return _refreshCommand; }
		}

		private void Refresh()
		{
			if (_saveExecuting)
				return;

			var tcs = new CancellationTokenSource();
			Task refreshTask = null;
			refreshTask = _compiler.CompileToImage(CodeEditor.Content, tcs.Token)
				 .ContinueWith(t =>
				 {
					 if (t.IsFaulted && t.Exception != null)
					 	Progress.Message = t.Exception.InnerException.Message;
					 else if (!t.IsCanceled)
					 	DiagramImage = t.Result;

				 	_refreshCancellations.Remove(refreshTask);
				 }, CancellationToken.None, TaskContinuationOptions.None, _uiScheduler);

			_refreshCancellations[refreshTask] = tcs;
		}

		void refreshTimer_Elapsed(object sender, EventArgs e)
		{
			Refresh();
			_refreshTimer.TryStop();
		}

		/// <summary>
		/// Whether an editor can currently be closed.
		/// </summary>
		public bool CanClose
		{
			get { return IsIdle; }
		}

		/// <summary>
		/// Command that closes a diagram editor.
		/// </summary>
		public ICommand CloseCommand
		{
			get { return _closeCommand; }
		}

		private void Close()
		{
			OnClosed();
		}

		/// <see cref="IDiagramEditor.Closed"/>
		public event EventHandler Closed;

		private void OnClosed()
		{
			var localEvent = Closed;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		/// <see cref="IDiagramEditor.Saved"/>
		public event EventHandler Saved;

		private void OnSaved()
		{
			var localEvent = Saved;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		/// <summary>
		/// The code editor.
		/// </summary>
		public CodeEditorViewModel CodeEditor { get; private set; }

		void codeEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == modifiedPropertyName)
			{
				if (AutoSave)
				{
					if (CodeEditor.IsModified)
						_autoSaveTimer.TryStart();
					else
						_autoSaveTimer.TryStop();
				}

				OnPropertyChanged(canSavePropertyName);	// boooo
			}
			else if (e.PropertyName == contentPropertyName)
			{
				// After a code change, reset the refresh timer.
				_refreshTimer.TryStop();
				_refreshTimer.TryStart();
			}
		}
		private static readonly string modifiedPropertyName = Reflect.PropertyOf<CodeEditorViewModel, bool>(p => p.IsModified).Name;
		private static readonly string contentPropertyName = Reflect.PropertyOf<CodeEditorViewModel, string>(p => p.Content).Name;
		private static readonly string canSavePropertyName = Reflect.PropertyOf<DiagramEditorViewModel, bool>(p => p.CanSave).Name;


		/// <summary>
		/// Commands available to operate on the diagram image.
		/// </summary>
		public IEnumerable<NamedOperationViewModel> ImageCommands { get; private set; }
 
		/// <summary>
		/// Contains current task progress information.
		/// </summary>
		public IProgressViewModel Progress { get; private set; }

		#region Implementation of IUndoProvider

		/// <see cref="IUndoProvider.UndoStack"/>
		public UndoStack UndoStack
		{
			get { return CodeEditor.UndoStack; }
		}

		#endregion

		/// <see cref="ViewModelBase.Dispose(bool)"/>
		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_autoSaveTimer.Elapsed -= autoSaveTimerElapsed;
					var disposableTimer = _autoSaveTimer as IDisposable;
					if (disposableTimer != null)
						disposableTimer.Dispose();
				}
				_disposed = true;
			}
		}
		private bool _disposed;

		private bool _firstSaveAfterOpen;
		private bool _saveExecuting;

		private readonly ICommand _saveCommand;
		private readonly ICommand _refreshCommand;
		private readonly ICommand _closeCommand;

		private readonly IDictionary<Task, CancellationTokenSource> _refreshCancellations = new ConcurrentDictionary<Task, CancellationTokenSource>();

		private readonly Property<bool> _autoSave;
		private readonly Property<TimeSpan> _autoSaveInterval;
		private readonly Property<bool> _isIdle;
		private readonly Property<Diagram> _diagram;
		private readonly Property<ImageSource> _diagramImage;

		private readonly IDiagramRenderer _diagramRenderer;
		private readonly IDiagramIOService _diagramIO;
		private readonly IDiagramCompiler _compiler;
		private readonly ITimer _autoSaveTimer;
		private readonly ITimer _refreshTimer;
		private readonly TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}