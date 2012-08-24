using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.Core.InputOutput;
using PlantUmlEditor.Properties;
using Utilities.Concurrency;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Manages diagram previews.
	/// </summary>
	public class DiagramExplorerViewModel : ViewModelBase, IDiagramExplorer
	{
		public DiagramExplorerViewModel(IProgressRegistration progressFactory, IDiagramIOService diagramIO, 
			Func<Diagram, PreviewDiagramViewModel> previewDiagramFactory, ISettings settings, TaskScheduler uiScheduler)
		{
			_progressFactory = progressFactory;
			_diagramIO = diagramIO;
			_previewDiagramFactory = previewDiagramFactory;
			_settings = settings;
			_uiScheduler = uiScheduler;

			_previewDiagrams = Property.New(this, p => PreviewDiagrams, OnPropertyChanged);
			_previewDiagrams.Value = new ObservableCollection<PreviewDiagramViewModel>();

			_currentPreviewDiagram = Property.New(this, p => p.CurrentPreviewDiagram, OnPropertyChanged);

			_diagramLocation = Property.New(this, p => p.DiagramLocation, OnPropertyChanged)
				.AlsoChanges(p => p.IsDiagramLocationValid);

			_newDiagramUri = Property.New(this, p => p.NewDiagramUri, OnPropertyChanged);

			LoadDiagramsCommand = new BoundRelayCommand<DiagramExplorerViewModel>(_ => LoadDiagramsAsync(), p => p.IsDiagramLocationValid, this);
			AddNewDiagramCommand = new RelayCommand<Uri>(AddNewDiagram);
			RequestOpenPreviewCommand = new RelayCommand<PreviewDiagramViewModel>(RequestOpenPreview, p => p != null);
			DeleteDiagramCommand = new RelayCommand<PreviewDiagramViewModel>(DeleteDiagram, p => p != null);

			_diagramIO.DiagramFileDeleted += diagramIO_DiagramFileDeleted;
			_diagramIO.DiagramFileAdded += diagramIO_DiagramFileAdded;
		}

		/// <summary>
		/// The code used for new diagrams.
		/// </summary>
		public string NewDiagramTemplate { get; set; }

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
		/// A new diagram's selected URI.
		/// </summary>
		public Uri NewDiagramUri
		{
			get { return _newDiagramUri.Value; }
			set { _newDiagramUri.Value = value; }
		}

		/// <summary>
		/// The currently selected preview diagram.
		/// </summary>
		public PreviewDiagramViewModel CurrentPreviewDiagram
		{
			get { return _currentPreviewDiagram.Value; }
			set { _currentPreviewDiagram.Value = value; }
		}

		/// <summary>
		/// The currently available diagrams.
		/// </summary>
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

		/// <summary>
		/// Event raised when a preview diagram should be opened for editing.
		/// </summary>
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

		private async void AddNewDiagram(Uri newDiagramUri)
		{
			string newFilePath = newDiagramUri.LocalPath;

			if (String.IsNullOrEmpty(Path.GetExtension(newFilePath)))
				newFilePath += ".puml";

			var newDiagram = new Diagram
			{
				File = new FileInfo(newFilePath),
				Content = String.Format(NewDiagramTemplate, Path.GetFileNameWithoutExtension(newFilePath) + ".png")
			};

			_diagramLocation.Value = new DirectoryInfo(Path.GetDirectoryName(newFilePath));

			var progress = _progressFactory.New(false);
			try
			{
				await _diagramIO.SaveAsync(newDiagram, false);

				var preview = _previewDiagramFactory(newDiagram);
				PreviewDiagrams.Add(preview);

				CurrentPreviewDiagram = preview;
				OnOpenPreviewRequested(preview);
			}
			catch (Exception e)
			{
				progress.Report(ProgressUpdate.Failed(e));
			}
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

			var progress = _progressFactory.New();
			progress.Report(new ProgressUpdate { PercentComplete = 0, Message = Resources.Progress_LoadingDiagrams });

			// Capture diagrams as they are read for a more responsive UI.
			var readProgress = new Progress<ReadDiagramsProgress>(p =>
			{
				if (p.Diagram.HasValue)
					PreviewDiagrams.Add(_previewDiagramFactory(p.Diagram.Value));
			});

			// Report progress to UI by passing up progress data.
			progress.Wrap(readProgress, p => new ProgressUpdate
			{
				PercentComplete = (int?)(p.ProcessedDiagramCount / (double)p.TotalDiagramCount * 100),
				Message = String.Format(Resources.Progress_LoadingFile, p.ProcessedDiagramCount, p.TotalDiagramCount)
			});

			try
			{
				await _diagramIO.ReadDiagramsAsync(DiagramLocation, readProgress);
				progress.Report(ProgressUpdate.Completed(Resources.Progress_DiagramsLoaded));
			}
			catch (Exception e)
			{
				progress.Report(ProgressUpdate.Failed(e));
			}
		}

		/// <summary>
		/// Command to delete a diagram.
		/// </summary>
		public ICommand DeleteDiagramCommand { get; private set; }

		private async void DeleteDiagram(PreviewDiagramViewModel preview)
		{
			var progress = _progressFactory.New(false);
			try
			{
				await _diagramIO.DeleteAsync(preview.Diagram);
				PreviewDiagrams.Remove(preview);
			}
			catch (Exception e)
			{
				progress.Report(ProgressUpdate.Failed(e));
			}
		}

		void diagramIO_DiagramFileDeleted(object sender, DiagramFileDeletedEventArgs e)
		{
			Task.Factory.StartNew(() =>
			{
				var existingPreview = PreviewDiagrams.FirstOrDefault(pd => pd.Diagram.File.FullName == e.DeletedDiagramFile.FullName);
				if (existingPreview != null)
				{
					OnDiagramDeleted(existingPreview.Diagram);
					PreviewDiagrams.Remove(existingPreview);
				}
			}, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
		}


		void diagramIO_DiagramFileAdded(object sender, DiagramFileAddedEventArgs e)
		{
			Task.Factory.StartNew(async () =>
			{
				// Make sure a preview doesn't already exist for the file and make sure the current directory still matches.
				var existingPreview = PreviewDiagrams.FirstOrDefault(pd => pd.Diagram.File.FullName == e.NewDiagramFile.FullName);
				if (existingPreview == null && e.NewDiagramFile.Directory.FullName == DiagramLocation.FullName)
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
		private readonly IProgressRegistration _progressFactory;

		private readonly Property<PreviewDiagramViewModel> _currentPreviewDiagram;
		private readonly Property<ICollection<PreviewDiagramViewModel>> _previewDiagrams;

		private readonly Property<DirectoryInfo> _diagramLocation;
		private readonly Property<Uri> _newDiagramUri;

		private readonly IDiagramIOService _diagramIO;

		private readonly Func<Diagram, PreviewDiagramViewModel> _previewDiagramFactory;
		private readonly ISettings _settings;
		private readonly TaskScheduler _uiScheduler;
	}
}