using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlEditor.Model;
using Utilities.Chronology;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram editor.
	/// </summary>
	public class DiagramEditorViewModel : ViewModelBase
	{
		public DiagramEditorViewModel(DiagramViewModel diagramViewModel, IEnumerable<SnippetCategoryViewModel> snippets, 
			IDiagramRenderer diagramRenderer, IDiagramCompiler diagramCompiler, ITimer refreshTimer, TaskScheduler taskScheduler)
		{
			_diagramViewModel = Property.New(this, p => p.DiagramViewModel, OnPropertyChanged);
			DiagramViewModel = diagramViewModel;

			_snippets = Property.New(this, p => p.Snippets, OnPropertyChanged);
			Snippets = new ObservableCollection<SnippetCategoryViewModel>(snippets);

			_diagramRenderer = diagramRenderer;
			_diagramCompiler = diagramCompiler;
			_refreshTimer = refreshTimer;
			_taskScheduler = taskScheduler;

			_content = Property.New(this, p => p.Content, OnPropertyChanged);
			_content.Value = diagramViewModel.Diagram.Content;

			_contentIndex = Property.New(this, p => p.ContentIndex, OnPropertyChanged);
			_contentIndex.Value = 0;

			_autoRefresh = Property.New(this, p => p.AutoRefresh, OnPropertyChanged);
			_refreshIntervalSeconds = Property.New(this, p => p.RefreshIntervalSeconds, OnPropertyChanged);

			_saveCommand = new RelayCommand(_ => Save());
			_closeCommand = new RelayCommand(_ => Close());

			// The document has been opened first time. So, any changes
			// made to the document will require creating a backup.
			_firstSaveAfterOpen = true;

			_refreshTimer.Elapsed += refreshTimer_Elapsed;
		}

		/// <summary>
		/// Whether to automatically refresh.
		/// </summary>
		public bool AutoRefresh
		{
			get { return _autoRefresh.Value; }
			set { _autoRefresh.Value = value; }
		}

		/// <summary>
		/// The content being edited.
		/// </summary>
		public string Content
		{
			get { return _content.Value; }
			set 
			{
				if (_content.TrySetValue(value))
				{
					if (AutoRefresh)
					{
						if (!_refreshDiagramTimerStarted)
						{
							_refreshDiagramTimerStarted = true;
							_refreshTimer.Start();
						}
					}
				}
			}
		}

		/// <summary>
		/// The current index into the content.
		/// </summary>
		public int ContentIndex
		{
			get { return _contentIndex.Value; }
			set { _contentIndex.Value = value; }
		}

		void refreshTimer_Elapsed(object sender, EventArgs e)
		{
			Save();
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
			if (_refreshDiagramTimerStarted)
				_refreshTimer.Stop();
			_refreshDiagramTimerStarted = false;
			
			var diagramFileName = DiagramViewModel.Diagram.DiagramFilePath;

			// Create a backup if this is the first time the diagram being modified
			// after opening
			if (_firstSaveAfterOpen)
			{
				File.Copy(diagramFileName, diagramFileName.Replace(new FileInfo(diagramFileName).Extension, ".bak"), true);
				_firstSaveAfterOpen = false;
			}

			DiagramViewModel.Diagram.Content = Content;

			if (_saveExecuting)
				return;

			//OnBeforeSave(DiagramViewModel.Diagram);

			_saveExecuting = true;
			var saveTask = Task.Factory.StartNew(() =>
			{
				// A Bug in PlantUML which is having problem detecting encoding if the
				// first line is not an empty line.
				if (!Char.IsWhiteSpace(Content, 0))
					Content = Environment.NewLine + Content;

				// Save the diagram content using UTF-8 encoding to support 
				// various international characters, which ASCII won't support
				// and Unicode won't make it cross platform
				File.WriteAllText(diagramFileName, Content, Encoding.UTF8);

				_diagramCompiler.Compile(DiagramViewModel.Diagram);
			}, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);

			saveTask.ContinueWith(t =>
			{
				_saveExecuting = false;
				DiagramViewModel.DiagramImage = _diagramRenderer.Render(DiagramViewModel.Diagram);
				//OnAfterSave(DiagramViewModel.Diagram);
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);

			saveTask.ContinueWith(t =>
			{
				_saveExecuting = false;
				//OnAfterSave(DiagramViewModel.Diagram);
				if (t.Exception != null)
				{
					throw t.Exception.InnerException;
					//MessageBox.Show(Window.GetWindow(this), t.Exception.Message, "Error running PlantUml",
					//				MessageBoxButton.OK, MessageBoxImage.Error);
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
		private bool _refreshDiagramTimerStarted;
		private bool _saveExecuting;

		private readonly ICommand _saveCommand;
		private readonly ICommand _closeCommand;

		private readonly Property<string> _content;
		private readonly Property<int> _contentIndex;
		private readonly Property<bool> _autoRefresh;
		private readonly Property<int> _refreshIntervalSeconds;
		private readonly Property<DiagramViewModel> _diagramViewModel;
		private readonly Property<IEnumerable<SnippetCategoryViewModel>> _snippets;

		private readonly IDiagramRenderer _diagramRenderer;
		private readonly IDiagramCompiler _diagramCompiler;
		private readonly ITimer _refreshTimer;
		private readonly TaskScheduler _taskScheduler;
		private readonly TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}