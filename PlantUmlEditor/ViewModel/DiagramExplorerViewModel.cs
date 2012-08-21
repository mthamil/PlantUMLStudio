using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
			Func<Diagram, PreviewDiagramViewModel> previewDiagramFactory, ISettings settings)
		{
			_progressFactory = progressFactory;
			_diagramIO = diagramIO;
			_previewDiagramFactory = previewDiagramFactory;
			_settings = settings;

			_previewDiagrams = Property.New(this, p => PreviewDiagrams, OnPropertyChanged);
			_previewDiagrams.Value = new ObservableCollection<PreviewDiagramViewModel>();

			_currentPreviewDiagram = Property.New(this, p => p.CurrentPreviewDiagram, OnPropertyChanged);

			_diagramLocation = Property.New(this, p => p.DiagramLocation, OnPropertyChanged)
				.AlsoChanges(p => p.IsDiagramLocationValid);

			_newDiagramUri = Property.New(this, p => p.NewDiagramUri, OnPropertyChanged);

			_loadDiagramsCommand = new BoundRelayCommand<DiagramExplorerViewModel>(_ => LoadDiagramsAsync(), p => p.IsDiagramLocationValid, this);
			_addNewDiagramCommand = new RelayCommand<Uri>(AddNewDiagram);
			_requestOpenPreviewCommand = new RelayCommand<PreviewDiagramViewModel>(RequestOpenPreview, p => p != null);
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
						_settings.LastDiagramLocation = value;

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
		public ICommand RequestOpenPreviewCommand
		{
			get { return _requestOpenPreviewCommand; }
		}

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

		/// <summary>
		/// Adds a new diagram with a given URI.
		/// </summary>
		public ICommand AddNewDiagramCommand
		{
			get { return _addNewDiagramCommand; }
		}

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
				await LoadDiagramsAsync();

				CurrentPreviewDiagram = PreviewDiagrams.SingleOrDefault(d => d.Diagram.File.FullName == newFilePath);
				OnOpenPreviewRequested(CurrentPreviewDiagram);
			}
			catch (Exception e)
			{
				progress.Report(ProgressUpdate.Failed(e));
			}
		}

		/// <summary>
		/// Command that loads diagrams from the current diagram location.
		/// </summary>
		public ICommand LoadDiagramsCommand
		{
			get { return _loadDiagramsCommand; }
		}

		private async Task LoadDiagramsAsync()
		{
			PreviewDiagrams.Clear();

			if (!IsDiagramLocationValid)
				return;

			var progress = _progressFactory.New();
			progress.Report(new ProgressUpdate { PercentComplete = 0, Message = Resources.Progress_LoadingDiagrams });

			var readProgress = new Progress<Tuple<int, int>>();
			progress.Wrap(readProgress, p => new ProgressUpdate
			{
				PercentComplete = (int?)(p.Item1 / (double)p.Item2 * 100),
				Message = String.Format(Resources.Progress_LoadingFile, p.Item1, p.Item2)
			});

			try
			{
				var diagrams = await _diagramIO.ReadDiagramsAsync(DiagramLocation, readProgress);
				foreach (var diagramFile in diagrams)
					PreviewDiagrams.Add(_previewDiagramFactory(diagramFile));

				progress.Report(ProgressUpdate.Completed(Resources.Progress_DiagramsLoaded));
			}
			catch (Exception e)
			{
				progress.Report(ProgressUpdate.Failed(e));
			}
		}

		/// <summary>
		/// Contains current task progress information.
		/// </summary>
		private readonly IProgressRegistration _progressFactory;

		private readonly Property<PreviewDiagramViewModel> _currentPreviewDiagram;
		private readonly Property<ICollection<PreviewDiagramViewModel>> _previewDiagrams;

		private readonly Property<DirectoryInfo> _diagramLocation;
		private readonly Property<Uri> _newDiagramUri;

		private readonly ICommand _loadDiagramsCommand;
		private readonly ICommand _addNewDiagramCommand;
		private readonly ICommand _requestOpenPreviewCommand;

		private readonly IDiagramIOService _diagramIO;

		private readonly Func<Diagram, PreviewDiagramViewModel> _previewDiagramFactory;
		private readonly ISettings _settings;
	}

	/// <summary>
	/// Event args containing information about when a diagram should be opened for editing.
	/// </summary>
	public class OpenPreviewRequestedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="requestedPreview">The preview to open for editing</param>
		public OpenPreviewRequestedEventArgs(PreviewDiagramViewModel requestedPreview)
		{
			RequestedPreview = requestedPreview;
		}

		/// <summary>
		/// The preview to open for editing.
		/// </summary>
		public PreviewDiagramViewModel RequestedPreview { get; private set; }
	}
}