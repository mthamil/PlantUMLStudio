//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Autofac.Features.Indexed;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.Imaging;
using PlantUmlStudio.Core.InputOutput;
using PlantUmlStudio.Properties;
using PlantUmlStudio.ViewModel.Notifications;
using SharpEssentials.Chronology;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Observable;

namespace PlantUmlStudio.ViewModel
{
    /// <summary>
    /// Represents a diagram editor.
    /// </summary>
    public class DiagramEditorViewModel : ViewModelBase, IDiagramEditor
	{
		/// <summary>
		/// Initializes a new diagram editor.
		/// </summary>
		/// <param name="diagram">The diagram being edited</param>
		/// <param name="codeEditor">The code editor</param>
		/// <param name="notifications">Creates objects that report progress</param>
		/// <param name="diagramRenderers">Responsible for converting diagram data to images</param>
		/// <param name="diagramIO">Saves diagrams</param>
		/// <param name="compiler">Compiles diagrams</param>
		/// <param name="autoSaveTimer">Determines how soon after a change a diagram will be autosaved</param>
		/// <param name="refreshTimer">Determines how long after the last code modification was made to automatically refresh a diagram's image</param>
		/// <param name="uiScheduler">A task scheduler for executing UI tasks</param>
		public DiagramEditorViewModel(Diagram diagram, 
                                      ICodeEditor codeEditor, 
                                      INotifications notifications,
			                          IIndex<ImageFormat, IDiagramRenderer> diagramRenderers, 
                                      IDiagramIOService diagramIO, 
                                      IDiagramCompiler compiler, 
			                          ITimer autoSaveTimer, 
                                      ITimer refreshTimer, 
                                      TaskScheduler uiScheduler) : this()
		{
			Diagram = diagram;

			_notifications = notifications;
			_diagramRenderers = diagramRenderers;
			_diagramIO = diagramIO;
			_compiler = compiler;
			_autoSaveTimer = autoSaveTimer;
			_refreshTimer = refreshTimer;
			_uiScheduler = uiScheduler;

			CodeEditor = codeEditor;
			CodeEditor.Content = Diagram.Content;
			CodeEditor.PropertyChanged += codeEditor_PropertyChanged;	// Subscribe after setting the content the first time.

			ImageFormat = Diagram.ImageFormat;

			IsIdle = true;

			// The document has been opened first time. So, any changes
			// made to the document will require creating a backup.
			_firstSaveAfterOpen = true;

			_autoSaveTimer.Elapsed += autoSaveTimerElapsed;
			_refreshTimer.Elapsed += refreshTimer_Elapsed;
		}

        private DiagramEditorViewModel()
        {
            _diagram = Property.New(this, p => p.Diagram);
            _imageFormat = Property.New(this, p => p.ImageFormat);
            _diagramImage = Property.New(this, p => p.DiagramImage);

            _isIdle = Property.New(this, p => p.IsIdle)
                              .AlsoChanges(p => p.CanSave)
                              .AlsoChanges(p => p.CanRefresh)
                              .AlsoChanges(p => p.CanClose);

            _autoSave = Property.New(this, p => p.AutoSave);
            _autoSaveInterval = Property.New(this, p => p.AutoSaveInterval);

            SaveCommand = Command.For(this).DependsOn(p => p.CanSave).ExecutesAsync(SaveAsync);
            RefreshCommand = Command.For(this).DependsOn(p => p.CanRefresh).ExecutesAsync(RefreshAsync);
            CloseCommand = Command.For(this).DependsOn(p => p.CanClose).Executes(Close);
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
				{
					_autoSaveTimer.Interval = value;

					if (AutoSave && _autoSaveTimer.Started)
						_autoSaveTimer.Restart();
				}
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
		/// The desired diagram image format.
		/// </summary>
		public ImageFormat ImageFormat
		{
			get { return _imageFormat.Value; }
			set 
			{
				if (_imageFormat.TrySetValue(value))
				{
					CancelRefreshes();
					RefreshAsync();
				}
			}
		}

		/// <summary>
		/// Whether an editor's content can currently be saved.
		/// </summary>
		public bool CanSave => CodeEditor.IsModified && IsIdle;

	    /// <summary>
		/// Command that saves a diagram's changes.
		/// </summary>
		public ICommand SaveCommand { get; }

	    /// <see cref="IDiagramEditor.SaveAsync"/>
		public Task SaveAsync()
		{
			_autoSaveTimer.TryStop();

			if (_saveExecuting)
				return Task.CompletedTask;

			_saveExecuting = true;
			IsIdle = false;
			CancelRefreshes();

			// PlantUML seems to have a problem detecting encoding if the
			// first line is not an empty line.
			if (!Char.IsWhiteSpace(CodeEditor.Content, 0))
				CodeEditor.Content = Environment.NewLine + CodeEditor.Content;

			Diagram.Content = CodeEditor.Content;
			Diagram.TryDeduceImageFile();

			// Create a backup if this is the first time the diagram being modified
			// after opening.
			bool makeBackup = false;
			if (_firstSaveAfterOpen)
			{
				makeBackup = true;
				_firstSaveAfterOpen = false;
			}

			var progress = _notifications.StartProgress(false);
			progress.Report(new ProgressUpdate 
			{ 
				PercentComplete = 100, 
				Message = String.Format(CultureInfo.CurrentCulture, Resources.Progress_SavingDiagram, Diagram.File.Name) 
			});

            var saveTask = UpdateDiagramAsync(makeBackup);
			saveTask.ContinueWith(t =>
			{
				if (t.IsFaulted && t.Exception != null)
				{
					progress.Report(ProgressUpdate.Failed(t.Exception.InnerException));
				}
				else if (!t.IsCanceled)
				{
					DiagramImage = _diagramRenderers[ImageFormat].Render(Diagram);
					Diagram.ImageFormat = ImageFormat;
					CodeEditor.IsModified = false;
					progress.Report(ProgressUpdate.Completed(Resources.Progress_DiagramSaved));
					OnSaved();
				}

				_saveExecuting = false;
				IsIdle = true;
				_refreshTimer.TryStop();

			}, CancellationToken.None, TaskContinuationOptions.None, _uiScheduler);

			return saveTask;
		}

	    private async Task UpdateDiagramAsync(bool makeBackup)
	    {
            await _diagramIO.SaveAsync(Diagram, makeBackup).ConfigureAwait(false);
	        await _compiler.CompileToFileAsync(Diagram.File, ImageFormat).ConfigureAwait(false);
	    }

		/// <see cref="IDiagramEditor.Saved"/>
		public event EventHandler Saved;

		private void OnSaved()
		{
            Saved?.Invoke(this, EventArgs.Empty);
		}

		async void autoSaveTimerElapsed(object sender, EventArgs e)
		{
			await SaveAsync();
		}

		/// <summary>
		/// Whether a diagram's image can currently be refreshed.
		/// </summary>
		public bool CanRefresh => IsIdle;

	    /// <summary>
		/// Refreshes a diagram's image without saving.
		/// </summary>
		public ICommand RefreshCommand { get; }

	    /// <see cref="IDiagramEditor.RefreshAsync"/>
		public async Task RefreshAsync()
		{
			if (_saveExecuting)
				return;

			var tcs = new CancellationTokenSource();

			var refreshTask = _compiler.CompileToImageAsync(CodeEditor.Content, ImageFormat, tcs.Token);
			_refreshCancellations[refreshTask] = tcs;

			try
			{
				DiagramImage = await refreshTask;
			}
			catch (OperationCanceledException)
			{
				// We don't care if refresh is canceled.
			}
			catch (IOException e)
			{
				_notifications.Notify(new ExceptionNotification(e));
			}
			catch (PlantUmlException e)
			{
				_notifications.Notify(new ExceptionNotification(e));
			}
			finally
			{
				_refreshCancellations.Remove(refreshTask);
			}	
		}

		async void refreshTimer_Elapsed(object sender, EventArgs e)
		{
			_refreshTimer.TryStop();
			await RefreshAsync();
		}

		/// <summary>
		/// Cancels all currently executing refresh tasks.
		/// </summary>
		private void CancelRefreshes()
		{
			foreach (var cts in _refreshCancellations)
				cts.Value.Cancel();
		}

		/// <summary>
		/// Whether an editor can currently be closed.
		/// </summary>
		public bool CanClose => IsIdle;

	    /// <summary>
		/// Command that closes a diagram editor.
		/// </summary>
		public ICommand CloseCommand { get; }

	    /// <summary>
		/// Closes a diagram editor.
		/// </summary>
		public void Close()
		{
			var cancelArgs = new CancelEventArgs();
			OnClosing(cancelArgs);

			if (!cancelArgs.Cancel)
			{
				CleanUpTimers();
				OnClosed();
			}
		}

		/// <see cref="IDiagramEditor.Closing"/>
		public event CancelEventHandler Closing;

		private void OnClosing(CancelEventArgs cancelArgs)
		{
            Closing?.Invoke(this, cancelArgs);
		}

		/// <see cref="IDiagramEditor.Closed"/>
		public event EventHandler Closed;

		private void OnClosed()
		{
            Closed?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// The code content editor.
		/// </summary>
		public ICodeEditor CodeEditor { get; }

		void codeEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CodeEditorViewModel.IsModified))
			{
				if (AutoSave)
				{
					if (CodeEditor.IsModified)
						_autoSaveTimer.TryStart();
					else
						_autoSaveTimer.TryStop();
				}

				OnPropertyChanged(nameof(CanSave));	// boooo
			}
			else if (e.PropertyName == nameof(CodeEditorViewModel.Content))
			{
				// After a code change, reset the refresh timer.
				_refreshTimer.TryStop();
				_refreshTimer.TryStart();
			}
		}

		private void CleanUpTimers()
		{
			_autoSaveTimer.Elapsed -= autoSaveTimerElapsed;
			_autoSaveTimer.TryStop();

			_refreshTimer.Elapsed -= refreshTimer_Elapsed;
			_refreshTimer.TryStop();
		}

		protected override void OnDisposing()
		{
			CleanUpTimers();
			(_autoSaveTimer as IDisposable)?.Dispose();
			(_refreshTimer as IDisposable)?.Dispose();
			(CodeEditor as IDisposable)?.Dispose();
		}

		private bool _firstSaveAfterOpen;
		private bool _saveExecuting;

	    private readonly IDictionary<Task, CancellationTokenSource> _refreshCancellations = new ConcurrentDictionary<Task, CancellationTokenSource>();

		private readonly Property<bool> _autoSave;
		private readonly Property<TimeSpan> _autoSaveInterval;
		private readonly Property<bool> _isIdle;
		private readonly Property<Diagram> _diagram;
		private readonly Property<ImageSource> _diagramImage;
		private readonly Property<ImageFormat> _imageFormat;

		private readonly INotifications _notifications;
		private readonly IIndex<ImageFormat, IDiagramRenderer> _diagramRenderers;
		private readonly IDiagramIOService _diagramIO;
		private readonly IDiagramCompiler _compiler;
		private readonly ITimer _autoSaveTimer;
		private readonly ITimer _refreshTimer;
		private readonly TaskScheduler _uiScheduler;
	}
}