using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	public class DiagramsViewModel : ViewModelBase
	{
		public DiagramsViewModel(IPreviewDiagrams previews, Func<PreviewDiagramViewModel, IDiagramEditor> editorFactory)
		{
			_previews = previews;
			Previews.NewDiagramCreated += previews_NewDiagramCreated;
			_editorFactory = editorFactory;

			_openDiagrams = Property.New(this, p => OpenDiagrams, OnPropertyChanged);
			_openDiagrams.Value = new ObservableCollection<IDiagramEditor>();

			_openDiagram = Property.New(this, p => p.OpenDiagram, OnPropertyChanged);

			_closingDiagram = Property.New(this, p => p.ClosingDiagram, OnPropertyChanged);

			_saveClosingDiagramCommand = new RelayCommand(() => _editorsNeedingSaving.Add(ClosingDiagram));
			_openDiagramCommand = new RelayCommand<PreviewDiagramViewModel>(OpenDiagramForEdit, d => d != null);
			_closeCommand = new RelayCommand(Close);
		}

		void previews_NewDiagramCreated(object sender, NewDiagramCreatedEventArgs e)
		{
			OpenDiagramForEdit(e.NewDiagramPreview);
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

		private void OpenDiagramForEdit(PreviewDiagramViewModel diagram)
		{
			var diagramEditor = OpenDiagrams.FirstOrDefault(d => d.Diagram.Equals(diagram.Diagram));
			if (diagramEditor == null)
			{
				diagramEditor = _editorFactory(diagram);
				diagramEditor.Closing += diagramEditor_Closing;
				diagramEditor.Closed += diagramEditor_Closed;
				diagramEditor.Saved += diagramEditor_Saved;
				OpenDiagrams.Add(diagramEditor);
			}

			OpenDiagram = diagramEditor;
		}

		void diagramEditor_Saved(object sender, EventArgs e)
		{
			var diagramEditor = (IDiagramEditor)sender;
			var preview = Previews.PreviewDiagrams.FirstOrDefault(d => d.Diagram.Equals(diagramEditor.Diagram));
			if (preview != null)
				preview.ImagePreview = diagramEditor.DiagramImage;
		}

		void diagramEditor_Closing(object sender, CancelEventArgs e)
		{
			ClosingDiagram = null;
			ClosingDiagram = (IDiagramEditor)sender;
		}

		void diagramEditor_Closed(object sender, EventArgs e)
		{
			var diagramEditor = (IDiagramEditor)sender;
			if (_editorsNeedingSaving.Contains(diagramEditor))
			{
				var saveTask = diagramEditor.Save();
				_editorSaveTasks.Add(saveTask);
				saveTask.ContinueWith(t =>_editorSaveTasks.Remove(t), 
					CancellationToken.None, TaskContinuationOptions.None, _uiScheduler);

				_editorsNeedingSaving.Remove(diagramEditor);
			}

			diagramEditor.Closing -= diagramEditor_Closing;
			diagramEditor.Closed -= diagramEditor_Closed;
			diagramEditor.Saved -= diagramEditor_Saved;
			OpenDiagrams.Remove(diagramEditor);
		}

		/// <summary>
		/// Saves the currently closing diagram.
		/// </summary>
		public ICommand SaveClosingDiagramCommand
		{
			get { return _saveClosingDiagramCommand; }
		}

		/// <summary>
		/// The diagram currently being closed, if any.
		/// </summary>
		public IDiagramEditor ClosingDiagram
		{
			get { return _closingDiagram.Value; }
			set { _closingDiagram.Value = value; }
		}

		/// <summary>
		/// Command executed when closing the main application window.
		/// </summary>
		public ICommand CloseCommand
		{
			get { return _closeCommand; }
		}

		private void Close()
		{
			var unsavedOpenDiagrams = OpenDiagrams.Where(od => od.CodeEditor.IsModified).ToList();
			foreach (var openDiagram in unsavedOpenDiagrams)
			{
				openDiagram.CloseCommand.Execute(null);
			}

			Task.WaitAll(_editorSaveTasks.ToArray());

			//if (IsDiagramLocationValid)
			//{
			//    Settings.Default.LastPath = DiagramLocation.FullName;
			//    Settings.Default.Save();
			//}
		}

		/// <summary>
		/// Diagram previews.
		/// </summary>
		public IPreviewDiagrams Previews
		{
			get { return _previews; }
		}

		private readonly Property<IDiagramEditor> _openDiagram;
		private readonly Property<ICollection<IDiagramEditor>> _openDiagrams;
		private readonly Property<IDiagramEditor> _closingDiagram;

		private readonly ICommand _openDiagramCommand;
		private readonly ICommand _closeCommand;

		private readonly ICommand _saveClosingDiagramCommand;
		private readonly ICollection<IDiagramEditor> _editorsNeedingSaving = new HashSet<IDiagramEditor>();
		private readonly ICollection<Task> _editorSaveTasks = new HashSet<Task>();

		private readonly IPreviewDiagrams _previews;
		private readonly Func<PreviewDiagramViewModel, IDiagramEditor> _editorFactory;
		private readonly TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}