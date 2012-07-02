using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.Document;
using PlantUmlEditor.Model;
using Utilities.Chronology;
using Utilities.Concurrency;
using Utilities.Controls.Behaviors;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;
using Utilities.Reflection;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram editor.
	/// </summary>
	public class DiagramEditorViewModel : ViewModelBase, IUndoProvider
	{
		/// <summary>
		/// Initializes a new diagram editor.
		/// </summary>
		/// <param name="diagramViewModel">The diagram being edited</param>
		/// <param name="codeEditor">The code editor</param>
		/// <param name="progressViewModel">Reports progress of editor tasks</param>
		/// <param name="diagramRenderer">Converts existing diagram output files to images</param>
		/// <param name="diagramIO">Saves diagrams</param>
		/// <param name="refreshTimer">Determines how soon after a change a diagram will be autosaved</param>
		public DiagramEditorViewModel(DiagramViewModel diagramViewModel, CodeEditorViewModel codeEditor, IProgressViewModel progressViewModel,
			IDiagramRenderer diagramRenderer, IDiagramIOService diagramIO, ITimer refreshTimer)
		{
			_diagramViewModel = Property.New(this, p => p.DiagramViewModel, OnPropertyChanged);
			DiagramViewModel = diagramViewModel;
			Progress = progressViewModel;

			_diagramRenderer = diagramRenderer;
			_diagramIO = diagramIO;
			_refreshTimer = refreshTimer;

			CodeEditor = codeEditor;
			CodeEditor.Content = diagramViewModel.Diagram.Content;
			CodeEditor.PropertyChanged += codeEditor_PropertyChanged;	// Subscribe after setting the content the first time.

			_isIdle = Property.New(this, p => p.IsIdle, OnPropertyChanged)
				.AlsoChanges(p => p.CanSave);
			IsIdle = true;

			_autoRefresh = Property.New(this, p => p.AutoRefresh, OnPropertyChanged);
			_refreshIntervalSeconds = Property.New(this, p => p.RefreshIntervalSeconds, OnPropertyChanged);

			_saveCommand = new BoundRelayCommand<DiagramEditorViewModel>(_ => Save(), p => p.CanSave, this);
			_closeCommand = new RelayCommand(_ => Close());

			// The document has been opened first time. So, any changes
			// made to the document will require creating a backup.
			_firstSaveAfterOpen = true;

			_refreshTimer.Elapsed += refreshTimer_Elapsed;

			ImageCommands = new List<NamedOperationViewModel>
			{
				new NamedOperationViewModel("Copy to Clipboard", 
					new RelayCommand(_ => Clipboard.SetImage(DiagramViewModel.DiagramImage as BitmapSource))),	// Copy image.
				new NamedOperationViewModel("Open in Explorer", 
					new RelayCommand(_ => Process.Start("explorer.exe","/select," + DiagramViewModel.Diagram.ImageFilePath).Dispose())), // Open in explorer.
				new NamedOperationViewModel("Copy Image Path", 
					new RelayCommand(_ => Clipboard.SetText(DiagramViewModel.Diagram.ImageFilePath)))	// Copy image path.
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
		public bool AutoRefresh
		{
			get { return _autoRefresh.Value; }
			set { _autoRefresh.Value = value; }
		}

		void refreshTimer_Elapsed(object sender, EventArgs e)
		{
			// We must begin the Save operation on the UI thread in order to update the UI 
			// with pre-save state.
			Task.Factory.StartNew(Save, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
		}

		/// <summary>
		/// The auto-refresh internval.
		/// </summary>
		public int RefreshIntervalSeconds
		{
			get { return _refreshIntervalSeconds.Value; }
			set
			{
				if (_refreshIntervalSeconds.TrySetValue(value))
					_refreshTimer.Interval = TimeSpan.FromSeconds(value);
			}
		}

		/// <summary>
		/// The underlying diagram.
		/// </summary>
		public DiagramViewModel DiagramViewModel
		{
			get { return _diagramViewModel.Value; }
			private set { _diagramViewModel.Value = value; }
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
			_refreshTimer.TryStop();

			if (_saveExecuting)
				return;

			_saveExecuting = true;
			IsIdle = false;

			// PlantUML seems to have a problem detecting encoding if the
			// first line is not an empty line.
			if (!Char.IsWhiteSpace(CodeEditor.Content, 0))
				CodeEditor.Content = Environment.NewLine + CodeEditor.Content;

			DiagramViewModel.Diagram.Content = CodeEditor.Content;

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
			var saveTask = _diagramIO.SaveAsync(DiagramViewModel.Diagram, makeBackup);

			saveTask.ContinueWith(t =>
			{
				_saveExecuting = false;
				DiagramViewModel.DiagramImage = _diagramRenderer.Render(DiagramViewModel.Diagram);
				CodeEditor.IsModified = false;
				IsIdle = true;
				progress.Report(Tuple.Create((int?)null, "Saved."));
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);

			saveTask.ContinueWith(t =>
			{
				_saveExecuting = false;
				IsIdle = true;
				if (t.Exception != null)
				{
					progress.Report(Tuple.Create((int?)null, t.Exception.InnerException.Message));
					throw t.Exception.InnerException;
				}
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, _uiScheduler);
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

		/// <summary>
		/// Event raised when a diagram editor has been closed.
		/// </summary>
		public event EventHandler Closed;

		private void OnClosed()
		{
			var localEvent = Closed;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		/// <summary>
		/// The code editor.
		/// </summary>
		public CodeEditorViewModel CodeEditor
		{
			get;
			private set;
		}

		void codeEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == modifiedPropertyName)
			{
				if (AutoRefresh)
				{
					if (CodeEditor.IsModified)
						_refreshTimer.TryStart();
					else
						_refreshTimer.TryStop();
				}

				OnPropertyChanged("CanSave");	// boooo
			}
		}
		private static readonly string modifiedPropertyName = Reflect.PropertyOf<CodeEditorViewModel, bool>(p => p.IsModified).Name;


		/// <summary>
		/// Commands available to operate on the diagram image.
		/// </summary>
		public IEnumerable<NamedOperationViewModel> ImageCommands
		{
			get;
			private set;
		}
 
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
					_refreshTimer.Elapsed -= refreshTimer_Elapsed;
					var disposableTimer = _refreshTimer as IDisposable;
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
		private readonly ICommand _closeCommand;

		private readonly Property<bool> _autoRefresh;
		private readonly Property<int> _refreshIntervalSeconds;
		private readonly Property<bool> _isIdle;
		private readonly Property<DiagramViewModel> _diagramViewModel;

		private readonly IDiagramRenderer _diagramRenderer;
		private readonly IDiagramIOService _diagramIO;
		private readonly ITimer _refreshTimer;
		private readonly TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}