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
		public DiagramsViewModel(IProgressViewModel progressViewModel, IDiagramReader diagramReader, Func<DiagramViewModel, DiagramEditorViewModel> editorFactory, 
			Func<Diagram, DiagramViewModel> diagramFactory, TaskScheduler taskScheduler)
		{
			Progress = progressViewModel;
			_diagramReader = diagramReader;
			_editorFactory = editorFactory;
			_diagramFactory = diagramFactory;
			_taskScheduler = taskScheduler;

			_diagrams = Property.New(this, p => Diagrams, OnPropertyChanged);
			_diagrams.Value = new ObservableCollection<DiagramViewModel>();

			_currentDiagram = Property.New(this, p => p.CurrentDiagram, OnPropertyChanged)
				.AlsoChanges(p => p.CanOpenDiagram);

			_openDiagrams = Property.New(this, p => OpenDiagrams, OnPropertyChanged);
			_openDiagrams.Value = new ObservableCollection<DiagramEditorViewModel>();

			_openDiagram = Property.New(this, p => p.OpenDiagram, OnPropertyChanged);

			_diagramLocation = Property.New(this, p => p.DiagramLocation, OnPropertyChanged)
				.AlsoChanges(p => p.IsDiagramLocationValid);

			_newDiagramUri = Property.New(this, p => p.NewDiagramUri, OnPropertyChanged);

			_loadDiagramsCommand = new BoundRelayCommand<DiagramsViewModel>(_ => LoadDiagrams(), p => p.IsDiagramLocationValid, this);
			_openDiagramCommand = new BoundRelayCommand<DiagramsViewModel>(_ => OpenDiagramForEdit(), p => p.CanOpenDiagram, this);
		}

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
		/// A new diagram's URI.
		/// </summary>
		public Uri NewDiagramUri
		{
			get { return _newDiagramUri.Value; }
			set
			{
				if (_newDiagramUri.TrySetValue(value))
				{
					var newFilePath = value.LocalPath;
					string contents = String.Format(
							"@startuml \"{0}.png\"{1}{2}{3}@enduml",
							Path.GetFileNameWithoutExtension(newFilePath), 
							Environment.NewLine,
							Environment.NewLine,
							Environment.NewLine);
					File.WriteAllText(newFilePath, contents);

					_diagramLocation.Value = new DirectoryInfo(Path.GetDirectoryName(newFilePath));
					LoadDiagrams().ContinueWith(t =>
					{
						CurrentDiagram = t.Result.SingleOrDefault(d => d.Diagram.DiagramFilePath == newFilePath);
						OpenDiagramForEdit();
					}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);
				}
			}
		}

		/// <summary>
		/// The currently available diagrams.
		/// </summary>
		public ICollection<DiagramViewModel> Diagrams
		{
			get { return _diagrams.Value; }
		}

		/// <summary>
		/// The diagram currently open for editing.
		/// </summary>
		public DiagramEditorViewModel OpenDiagram
		{
			get { return _openDiagram.Value; }
			set { _openDiagram.Value = value; }
		}

		/// <summary>
		/// The currently open diagrams.
		/// </summary>
		public ICollection<DiagramEditorViewModel> OpenDiagrams
		{
			get { return _openDiagrams.Value; }
		}

		/// <summary>
		/// Whether a diagram can be opened for editing.
		/// </summary>
		public bool CanOpenDiagram
		{
			get { return CurrentDiagram != null; }
		}

		/// <summary>
		/// Command to open a diagram for editing.
		/// </summary>
		public ICommand OpenDiagramCommand
		{
			get { return _openDiagramCommand; }
		}

		private void OpenDiagramForEdit()
		{
			var diagramEditor = OpenDiagrams.FirstOrDefault(d => d.DiagramViewModel.Equals(CurrentDiagram));
			if (diagramEditor == null)
			{
				diagramEditor = _editorFactory(CurrentDiagram);
				diagramEditor.Closed += diagramEditor_Closed;
				OpenDiagrams.Add(diagramEditor);
			}

			OpenDiagram = diagramEditor;
		}

		void diagramEditor_Closed(object sender, EventArgs e)
		{
			var diagramEditor = (DiagramEditorViewModel)sender;
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
			var loadTask = Task.Factory.StartNew(() =>
			{
				var diagrams = new List<Diagram>();

				FileInfo[] files = DiagramLocation.GetFiles("*.txt");
				int numberOfFiles = files.Length;
				int processed = 0;
				foreach (FileInfo file in files)
				{
					var diagram = _diagramReader.Read(file);
					if (diagram != null)
						diagrams.Add(diagram);
					//Thread.Sleep(1000);
					processed++;
					progress.Report(Tuple.Create(
						(int?)(processed / (double)numberOfFiles * 100),
						String.Format("Loading {0} of {1}", processed, numberOfFiles)));
				}

				return diagrams;
			}, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);

			loadTask.ContinueWith(t =>
			{
				if (t.Exception != null)
				{
					progress.Report(Tuple.Create((int?)null, t.Exception.InnerException.Message));
					throw t.Exception.InnerException;
				}

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

		private readonly Property<DiagramEditorViewModel> _openDiagram;
		private readonly Property<ICollection<DiagramEditorViewModel>> _openDiagrams;

		private readonly ICommand _loadDiagramsCommand;
		private readonly ICommand _openDiagramCommand;

		private readonly IDiagramReader _diagramReader;
		private readonly Func<DiagramViewModel, DiagramEditorViewModel> _editorFactory;
		private readonly Func<Diagram, DiagramViewModel> _diagramFactory;
		private readonly TaskScheduler _taskScheduler;
		private readonly TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}