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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlStudio.Configuration;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.InputOutput;
using PlantUmlStudio.Properties;
using PlantUmlStudio.ViewModel.Notifications;
using Utilities.Collections;
using Utilities.Concurrency;
using Utilities.InputOutput;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.Observable;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Manages diagram previews.
	/// </summary>
	public class DiagramExplorerViewModel : ViewModelBase, IDiagramExplorer
	{
		public DiagramExplorerViewModel(INotifications notifications, IDiagramIOService diagramIO, 
			Func<Diagram, PreviewDiagramViewModel> previewDiagramFactory, ISettings settings, TaskScheduler uiScheduler)
				: this()
		{
			_notifications = notifications;
			_diagramIO = diagramIO;
			_previewDiagramFactory = previewDiagramFactory;
			_settings = settings;
			_uiScheduler = uiScheduler;

			_diagramIO.DiagramFileDeleted += diagramIO_DiagramFileDeleted;
			_diagramIO.DiagramFileAdded += diagramIO_DiagramFileAdded;
		}

		/// <summary>
		/// Handles initialization that does not use injected dependencies.
		/// </summary>
		protected DiagramExplorerViewModel()
		{
			_previewDiagrams = Property.New(this, p => PreviewDiagrams, OnPropertyChanged);
			_previewDiagrams.Value = new ObservableCollection<PreviewDiagramViewModel>();

			_currentPreviewDiagram = Property.New(this, p => p.CurrentPreviewDiagram, OnPropertyChanged);

			_diagramLocation = Property.New(this, p => p.DiagramLocation, OnPropertyChanged)
			                           .AlsoChanges(p => p.IsDiagramLocationValid);

			_isLoadingDiagrams = Property.New(this, p => p.IsLoadingDiagrams, OnPropertyChanged);

			LoadDiagramsCommand = Command.For(this).DependsOn(p => p.IsDiagramLocationValid).Asynchronously().Executes(LoadDiagramsAsync);
			AddNewDiagramCommand = new AsyncRelayCommand<Uri>(AddNewDiagramAsync);
			RequestOpenPreviewCommand = new RelayCommand<PreviewDiagramViewModel>(RequestOpenPreview, p => p != null);
			OpenDiagramCommand = new RelayCommand<Uri>(async uri =>
			{
				try { await OpenDiagramAsync(uri); }
				catch (Exception e) { _notifications.Notify(new ExceptionNotification(e)); }
			});
			DeleteDiagramCommand = new AsyncRelayCommand<PreviewDiagramViewModel>(DeleteDiagramAsync, preview => preview != null);
			_cancelLoadDiagramsCommand = Property.New(this, p => p.CancelLoadDiagramsCommand, OnPropertyChanged);
		}

		/// <summary>
		/// The code used for new diagrams.
		/// </summary>
		public string NewDiagramTemplate { get; set; }

		/// <summary>
		/// The diagram file extension.
		/// </summary>
		public string FileExtension { get; set; }

		/// <see cref="IDiagramExplorer.DiagramLocation"/>
		public DirectoryInfo DiagramLocation
		{
			get { return _diagramLocation.Value; }
			set
			{
				if (_diagramLocation.TrySetValue(value))
				{
					if (IsDiagramLocationValid)
					{
						_settings.LastDiagramLocation = value;
						_diagramIO.StartMonitoring(value);
					}
					else
					{
						_diagramIO.StopMonitoring();
					}

					LoadDiagramsAsync();
				}
			}
		}

		/// <summary>
		/// Whether the current diagram location is valid.
		/// </summary>
		public bool IsDiagramLocationValid
		{
			get { return DiagramLocation != null && DiagramLocation.Exists; }
		}

		/// <summary>
		/// The currently selected preview diagram.
		/// </summary>
		public PreviewDiagramViewModel CurrentPreviewDiagram
		{
			get { return _currentPreviewDiagram.Value; }
			set { _currentPreviewDiagram.Value = value; }
		}

		/// <see cref="IDiagramExplorer.PreviewDiagrams"/>
		public ICollection<PreviewDiagramViewModel> PreviewDiagrams
		{
			get { return _previewDiagrams.Value; }
		}

		/// <summary>
		/// Opens a preview for editing.
		/// </summary>
		public ICommand RequestOpenPreviewCommand { get; private set; }

		private void RequestOpenPreview(PreviewDiagramViewModel preview)
		{
			OnOpenPreviewRequested(preview);
		}

		/// <see cref="IDiagramExplorer.OpenPreviewRequested"/>
		public event EventHandler<OpenPreviewRequestedEventArgs> OpenPreviewRequested;

		private void OnOpenPreviewRequested(PreviewDiagramViewModel preview)
		{
			var localEvent = OpenPreviewRequested;
			if (localEvent != null)
				localEvent(this, new OpenPreviewRequestedEventArgs(preview));
		}

		/// <see cref="IDiagramExplorer.DiagramDeleted"/>
		public event EventHandler<DiagramDeletedEventArgs> DiagramDeleted;

		private void OnDiagramDeleted(Diagram deletedDiagram)
		{
			var localEvent = DiagramDeleted;
			if (localEvent != null)
				localEvent(this, new DiagramDeletedEventArgs(deletedDiagram));
		}

		/// <summary>
		/// Adds a new diagram with a given URI.
		/// </summary>
		public ICommand AddNewDiagramCommand { get; private set; }

		private async Task AddNewDiagramAsync(Uri newDiagramUri)
		{
			string newFilePath = newDiagramUri.LocalPath;

			if (String.IsNullOrEmpty(Path.GetExtension(newFilePath)))
				newFilePath += FileExtension;

			var newDiagram = new Diagram
			{
				File = new FileInfo(newFilePath),
				Content = String.Format(NewDiagramTemplate, Path.GetFileNameWithoutExtension(newFilePath) + ".png")
			};

			_diagramLocation.Value = new DirectoryInfo(Path.GetDirectoryName(newFilePath));

			var progress = _notifications.StartProgress(false);
			try
			{
				await _diagramIO.SaveAsync(newDiagram, false);
				var refreshedDiagram = await _diagramIO.ReadAsync(newDiagram.File);

				var preview = PreviewDiagrams.SingleOrDefault(p => fileComparer.Equals(p.Diagram.File, refreshedDiagram.File));
				if (preview == null)
				{
					preview = _previewDiagramFactory(refreshedDiagram);
					PreviewDiagrams.Add(preview);
				}

				CurrentPreviewDiagram = preview;
				OnOpenPreviewRequested(preview);

				progress.Report(ProgressUpdate.Completed(string.Empty));
			}
			catch (Exception e)
			{
				progress.Report(ProgressUpdate.Failed(e));
			}
		}

		/// <summary>
		/// Whether diagrams are currently being loaded.
		/// </summary>
		public bool IsLoadingDiagrams
		{
			get { return _isLoadingDiagrams.Value; }
			private set { _isLoadingDiagrams.Value = value; }
		}

		/// <summary>
		/// Command that loads diagrams from the current diagram location.
		/// </summary>
		public ICommand LoadDiagramsCommand { get; private set; }

		private async Task LoadDiagramsAsync()
		{
			PreviewDiagrams.Clear();

			if (!IsDiagramLocationValid)
				return;

			using (var cts = new CancellationTokenSource())
			{
				IsLoadingDiagrams = true;

				var progress = _notifications.StartProgress();
				progress.Report(new ProgressUpdate { PercentComplete = 0, Message = Resources.Progress_LoadingDiagrams });

				CancelLoadDiagramsCommand = new CancelTaskCommand(cts);

				// Capture diagrams as they are read for a more responsive UI.
				var readProgress = new Progress<ReadDiagramsProgress>(p =>
					p.Diagram.Apply(d => PreviewDiagrams.Add(_previewDiagramFactory(d))));

				// Report progress to UI by passing up progress data.
				progress.Wrap(readProgress, p => new ProgressUpdate
				{
					PercentComplete = (int?)(p.ProcessedDiagramCount / (double)p.TotalDiagramCount * 100),
					Message = String.Format(Resources.Progress_LoadingFile, p.ProcessedDiagramCount, p.TotalDiagramCount)
				});

				try
				{
					await _diagramIO.ReadDiagramsAsync(DiagramLocation, readProgress, cts.Token);
					progress.Report(ProgressUpdate.Completed(Resources.Progress_DiagramsLoaded));
				}
				catch (OperationCanceledException)
				{
					progress.Report(ProgressUpdate.Completed(Resources.Progress_DiagramLoadCanceled));
				}
				catch (Exception e)
				{
					progress.Report(ProgressUpdate.Failed(e));
				}
				finally
				{
					CancelLoadDiagramsCommand = null;
					IsLoadingDiagrams = false;
				}
			}
		}

		/// <summary>
		/// Cancels loading of diagrams.
		/// </summary>
		public ICommand CancelLoadDiagramsCommand
		{
			get { return _cancelLoadDiagramsCommand.Value; }
			private set { _cancelLoadDiagramsCommand.Value = value; }
		}

		/// <summary>
		/// Command to delete a diagram.
		/// </summary>
		public ICommand DeleteDiagramCommand { get; private set; }

		private async Task DeleteDiagramAsync(PreviewDiagramViewModel preview)
		{
			try
			{
				await _diagramIO.DeleteAsync(preview.Diagram);
				PreviewDiagrams.Remove(preview);
			}
			catch (Exception e)
			{
				_notifications.Notify(new ExceptionNotification(e));
			}
		}

		/// <summary>
		/// Command to open a diagram.
		/// </summary>
		public ICommand OpenDiagramCommand { get; private set; }

		/// <see cref="IDiagramExplorer.OpenDiagramAsync"/>
		public async Task<Diagram> OpenDiagramAsync(Uri diagramPath)
		{
			var fileToOpen = new FileInfo(diagramPath.LocalPath);

			Diagram diagram;
			var preview = PreviewDiagrams.SingleOrDefault(p => fileComparer.Equals(p.Diagram.File, fileToOpen));
			if (preview == null)
			{
				diagram = await _diagramIO.ReadAsync(fileToOpen);
				preview = _previewDiagramFactory(diagram);
			}
			else
			{
				diagram = preview.Diagram;
			}

			OnOpenPreviewRequested(preview);
			return diagram;
		}

		/// <summary>
		/// Asynchronously opens a sequence of diagrams.
		/// </summary>
		/// <param name="diagramFiles">The diagram files to open</param>
		/// <returns>A task representing the open operation</returns>
		public async Task OpenDiagramFilesAsync(IEnumerable<FileInfo> diagramFiles)
		{
			await diagramFiles.Select(file => OpenDiagramAsync(new Uri(file.FullName))).ToList();
		}

		void diagramIO_DiagramFileDeleted(object sender, DiagramFileDeletedEventArgs e)
		{
			Task.Factory.StartNew(() => 
				PreviewDiagrams.FirstOrNone(pd => fileComparer.Equals(pd.Diagram.File, e.DeletedDiagramFile)).Apply(existingPreview =>
				{
					OnDiagramDeleted(existingPreview.Diagram);
					PreviewDiagrams.Remove(existingPreview);
				}), CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
		}


		void diagramIO_DiagramFileAdded(object sender, DiagramFileAddedEventArgs e)
		{
			Task.Factory.StartNew(async () =>
			{
				// Make sure a preview doesn't already exist for the file and make sure the current directory still matches.
				bool previewExists = PreviewDiagrams.Select(pd => pd.Diagram.File).Contains(e.NewDiagramFile, fileComparer);
				if (!previewExists && e.NewDiagramFile.Directory.FullName.Trim('\\') == DiagramLocation.FullName.Trim('\\'))
				{
					var newlyAddedDiagram = await _diagramIO.ReadAsync(e.NewDiagramFile);
					if (newlyAddedDiagram != null)
						PreviewDiagrams.Add(_previewDiagramFactory(newlyAddedDiagram));
				}
			}, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
		}

		/// <summary>
		/// Contains current task progress information.
		/// </summary>
		private readonly INotifications _notifications;

		private readonly Property<PreviewDiagramViewModel> _currentPreviewDiagram;
		private readonly Property<ICollection<PreviewDiagramViewModel>> _previewDiagrams;

		private readonly Property<DirectoryInfo> _diagramLocation;

		private readonly Property<ICommand> _cancelLoadDiagramsCommand;
		private readonly Property<bool> _isLoadingDiagrams;

		private readonly IDiagramIOService _diagramIO;

		private readonly Func<Diagram, PreviewDiagramViewModel> _previewDiagramFactory;
		private readonly ISettings _settings;
		private readonly TaskScheduler _uiScheduler;

		private static readonly IEqualityComparer<FileInfo> fileComparer = FileSystemInfoPathEqualityComparer.Instance;
	}
}