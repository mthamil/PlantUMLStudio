using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlEditor.Model;
using Utilities.Concurrency;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	public class DiagramsViewModel : ViewModelBase
	{
		public DiagramsViewModel(IProgressViewModel progressViewModel, IDiagramIOService diagramIO, Func<DiagramViewModel, IDiagramEditor> editorFactory, 
			Func<Diagram, DiagramViewModel> diagramFactory)
		{
			Progress = progressViewModel;
			_diagramIO = diagramIO;
			_editorFactory = editorFactory;
			_diagramFactory = diagramFactory;

			_diagrams = Property.New(this, p => Diagrams, OnPropertyChanged);
			_diagrams.Value = new ObservableCollection<DiagramViewModel>();

			_currentDiagram = Property.New(this, p => p.CurrentDiagram, OnPropertyChanged);

			_openDiagrams = Property.New(this, p => OpenDiagrams, OnPropertyChanged);
			_openDiagrams.Value = new ObservableCollection<IDiagramEditor>();

			_openDiagram = Property.New(this, p => p.OpenDiagram, OnPropertyChanged);

			_diagramLocation = Property.New(this, p => p.DiagramLocation, OnPropertyChanged)
				.AlsoChanges(p => p.IsDiagramLocationValid);

			_newDiagramUri = Property.New(this, p => p.NewDiagramUri, OnPropertyChanged);

			_loadDiagramsCommand = new BoundRelayCommand<DiagramsViewModel>(_ => LoadDiagrams(), p => p.IsDiagramLocationValid, this);
			_addNewDiagramCommand = new RelayCommand(uri => AddNewDiagram((Uri)uri));
			_openDiagramCommand = new RelayCommand(d => OpenDiagramForEdit((DiagramViewModel)d), d => (d as DiagramViewModel) != null);
		}

		/// <summary>
		/// The code used for new diagrams.
		/// </summary>
		public string NewDiagramTemplate { get; set; }

		/// <summary>
		/// The location to load diagrams from.
		/// </summary>
		public DirectoryInfo DiagramLocation
		{
			get { return _diagramLocation.Value; }
			set 
			{
				if (_diagramLocation.TrySetValue(value))
					LoadDiagrams();
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
		/// The currently selected diagram.
		/// </summary>
		public DiagramViewModel CurrentDiagram
		{
			get { return _currentDiagram.Value; }
			set { _currentDiagram.Value = value; }
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
		/// The currently available diagrams.
		/// </summary>
		public ICollection<DiagramViewModel> Diagrams
		{
			get { return _diagrams.Value; }
		}

		/// <summary>
		/// Adds a new diagram with a given URI.
		/// </summary>
		public ICommand AddNewDiagramCommand
		{
			get { return _addNewDiagramCommand; }
		}

		private void AddNewDiagram(Uri newDiagramUri)
		{
			string newFilePath = newDiagramUri.LocalPath;
			var newDiagram = new Diagram
			{
				DiagramFilePath = newFilePath,
				Content = String.Format(NewDiagramTemplate, Path.GetFileNameWithoutExtension(newFilePath) + ".png")
			};

			_diagramLocation.Value = new DirectoryInfo(Path.GetDirectoryName(newFilePath));

			_diagramIO.SaveAsync(newDiagram, false)
				.ContinueWith(st =>
				{
					LoadDiagrams().ContinueWith(lt =>
					{
						CurrentDiagram = lt.Result.SingleOrDefault(d => d.Diagram.DiagramFilePath == newFilePath);
						OpenDiagramForEdit(CurrentDiagram);
					}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);
				}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);
		}

		/// <summary>
		/// The diagram currently open for editing.
		/// </summary>
		public IDiagramEditor OpenDiagram
		{
			get { return _openDiagram.Value; }
			set { _openDiagram.Value = value; }
		}

		/// <summary>
		/// The currently open diagrams.
		/// </summary>
		public ICollection<IDiagramEditor> OpenDiagrams
		{
			get { return _openDiagrams.Value; }
		}

		/// <summary>
		/// Command to open a diagram for editing.
		/// </summary>
		public ICommand OpenDiagramCommand
		{
			get { return _openDiagramCommand; }
		}

		private void OpenDiagramForEdit(DiagramViewModel diagram)
		{
			var diagramEditor = OpenDiagrams.FirstOrDefault(d => d.DiagramViewModel.Equals(diagram));
			if (diagramEditor == null)
			{
				diagramEditor = _editorFactory(diagram);
				diagramEditor.Closed += diagramEditor_Closed;
				OpenDiagrams.Add(diagramEditor);
			}

			OpenDiagram = diagramEditor;
		}

		void diagramEditor_Closed(object sender, EventArgs e)
		{
			var diagramEditor = (IDiagramEditor)sender;
			diagramEditor.Closed -= diagramEditor_Closed;
			OpenDiagrams.Remove(diagramEditor);
		}

		/// <summary>
		/// Command that loads diagrams from the current diagram location.
		/// </summary>
		public ICommand LoadDiagramsCommand
		{
			get { return _loadDiagramsCommand; }
		}

		private Task<ICollection<DiagramViewModel>> LoadDiagrams()
		{
			_diagrams.Value.Clear();

			if (!IsDiagramLocationValid)
				return Tasks.FromResult(_diagrams.Value);

			Progress.HasDiscreteProgress = true;
			IProgress<Tuple<int?, string>> progress = new Progress<Tuple<int?, string>>(p =>
			{
				Progress.PercentComplete = p.Item1;
				Progress.Message = p.Item2;
			});

			progress.Report(Tuple.Create((int?)0, "Loading diagrams..."));
			var loadTask = _diagramIO.ReadDiagramsAsync(DiagramLocation, progress);

			loadTask.ContinueWith(t =>
			{
				if (t.Exception != null)
					progress.Report(Tuple.Create((int?)null, t.Exception.InnerException.Message));

			}, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, _uiScheduler);

			return loadTask.ContinueWith(t =>
			{
				foreach (var diagramFile in t.Result)
					_diagrams.Value.Add(_diagramFactory(diagramFile));

				progress.Report(Tuple.Create((int?)null, "Diagrams loaded."));

				return _diagrams.Value;
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);
		}

		/// <summary>
		/// Contains current task progress information.
		/// </summary>
		public IProgressViewModel Progress { get; private set; }

		private readonly Property<DiagramViewModel> _currentDiagram;
		private readonly Property<ICollection<DiagramViewModel>> _diagrams;

		private readonly Property<DirectoryInfo> _diagramLocation;
		private readonly Property<Uri> _newDiagramUri;

		private readonly Property<IDiagramEditor> _openDiagram;
		private readonly Property<ICollection<IDiagramEditor>> _openDiagrams;

		private readonly ICommand _loadDiagramsCommand;
		private readonly ICommand _addNewDiagramCommand;
		private readonly ICommand _openDiagramCommand;

		private readonly IDiagramIOService _diagramIO;
		private readonly Func<DiagramViewModel, IDiagramEditor> _editorFactory;
		private readonly Func<Diagram, DiagramViewModel> _diagramFactory;
		private readonly TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}