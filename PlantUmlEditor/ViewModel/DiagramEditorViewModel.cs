using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlEditor.Model;
using Utilities.Chronology;
using Utilities.Concurrency;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;
using Utilities.Reflection;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram editor.
	/// </summary>
	public class DiagramEditorViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new diagram editor.
		/// </summary>
		/// <param name="diagramViewModel">The diagram being edited</param>
		/// <param name="progressViewModel">Reports progress of editor tasks</param>
		/// <param name="snippets">Available code snippets</param>
		/// <param name="diagramRenderer">Converts existing diagram output files to images</param>
		/// <param name="diagramCompiler">Creates diagram images</param>
		/// <param name="refreshTimer">Determines how soon after a change a diagram will be autosaved</param>
		/// <param name="taskScheduler">Executes background tasks</param>
		public DiagramEditorViewModel(DiagramViewModel diagramViewModel, IProgressViewModel progressViewModel, IEnumerable<SnippetCategoryViewModel> snippets, 
			IDiagramRenderer diagramRenderer, IDiagramCompiler diagramCompiler, ITimer refreshTimer, TaskScheduler taskScheduler)
		{
			_diagramViewModel = Property.New(this, p => p.DiagramViewModel, OnPropertyChanged);
			DiagramViewModel = diagramViewModel;
			Progress = progressViewModel;

			_snippets = Property.New(this, p => p.Snippets, OnPropertyChanged);
			Snippets = new ObservableCollection<SnippetCategoryViewModel>(snippets);

			_diagramRenderer = diagramRenderer;
			_diagramCompiler = diagramCompiler;
			_refreshTimer = refreshTimer;
			_taskScheduler = taskScheduler;

			CodeEditor.Content = diagramViewModel.Diagram.Content;
			CodeEditor.PropertyChanged += _codeEditor_PropertyChanged;	// Subscribe after setting the content the first time.

			_autoRefresh = Property.New(this, p => p.AutoRefresh, OnPropertyChanged);
			_refreshIntervalSeconds = Property.New(this, p => p.RefreshIntervalSeconds, OnPropertyChanged);

			_saveCommand = new BoundRelayCommand<CodeEditorViewModel>(_ => Save(), p => p.IsModified, CodeEditor);
			_closeCommand = new RelayCommand(_ => Close());

			// The document has been opened first time. So, any changes
			// made to the document will require creating a backup.
			_firstSaveAfterOpen = true;

			_refreshTimer.Elapsed += refreshTimer_Elapsed;
		}

		/// <summary>
		/// The code editor.
		/// </summary>
		public CodeEditorViewModel CodeEditor
		{
			get { return _codeEditor; }
		}

		void _codeEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == modifiedPropertyName)
			{
				if (AutoRefresh)
				{
					if (_codeEditor.IsModified)
						_refreshTimer.TryStart();
					else
						_refreshTimer.TryStop();
				}
			}
		}
		private static readonly string modifiedPropertyName = Reflect.PropertyOf<CodeEditorViewModel, bool>(p => p.IsModified).Name;

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
			Save();
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
		/// Diagram code snippets.
		/// </summary>
		public IEnumerable<SnippetCategoryViewModel> Snippets
		{
			get { return _snippets.Value; }
			private set { _snippets.Value = value; }
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
			
			var diagramFile = new FileInfo(DiagramViewModel.Diagram.DiagramFilePath);

			// Create a backup if this is the first time the diagram being modified
			// after opening
			if (_firstSaveAfterOpen)
			{
				diagramFile.CopyTo(diagramFile.FullName + ".bak", true);
				_firstSaveAfterOpen = false;
			}

			DiagramViewModel.Diagram.Content = CodeEditor.Content;

			if (_saveExecuting)
				return;

			Progress.HasDiscreteProgress = false;
			IProgress<Tuple<int?, string>> progress = new Progress<Tuple<int?, string>>(p =>
			{
				Progress.PercentComplete = p.Item1;
				Progress.Message = p.Item2;
			});

			progress.Report(Tuple.Create((int?)100, "Saving and generating diagram..."));

			_saveExecuting = true;
			var saveTask = Task.Factory.StartNew(() =>
			{
				// A Bug in PlantUML which is having problem detecting encoding if the
				// first line is not an empty line.
				if (!Char.IsWhiteSpace(CodeEditor.Content, 0))
					CodeEditor.Content = Environment.NewLine + CodeEditor.Content;
				//Thread.Sleep(4000);
				// Save the diagram content using UTF-8 encoding to support 
				// various international characters, which ASCII won't support
				// and Unicode won't make it cross platform
				File.WriteAllText(diagramFile.FullName, CodeEditor.Content, Encoding.UTF8);

				_diagramCompiler.Compile(DiagramViewModel.Diagram);
			}, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);

			saveTask.ContinueWith(t =>
			{
				_saveExecuting = false;
				DiagramViewModel.DiagramImage = _diagramRenderer.Render(DiagramViewModel.Diagram);
				CodeEditor.IsModified = false;
				progress.Report(Tuple.Create((int?)null, "Saved."));
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);

			saveTask.ContinueWith(t =>
			{
				_saveExecuting = false;
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
		/// Contains current task progress information.
		/// </summary>
		public IProgressViewModel Progress { get; private set; }

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
		private readonly Property<DiagramViewModel> _diagramViewModel;
		private readonly Property<IEnumerable<SnippetCategoryViewModel>> _snippets;

		private readonly CodeEditorViewModel _codeEditor = new CodeEditorViewModel();

		private readonly IDiagramRenderer _diagramRenderer;
		private readonly IDiagramCompiler _diagramCompiler;
		private readonly ITimer _refreshTimer;
		private readonly TaskScheduler _taskScheduler;
		private readonly TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}