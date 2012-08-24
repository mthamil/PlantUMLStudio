using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlEditor.Configuration;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;
using Utilities.Reflection;

namespace PlantUmlEditor.ViewModel
{
	public class DiagramManagerViewModel : ViewModelBase
	{
		public DiagramManagerViewModel(IDiagramExplorer explorer, Func<PreviewDiagramViewModel, IDiagramEditor> editorFactory, ISettings settings)
		{
			_explorer = explorer;
			_editorFactory = editorFactory;
			_settings = settings;

			_openDiagrams = Property.New(this, p => OpenDiagrams, OnPropertyChanged);
			_openDiagrams.Value = new ObservableCollection<IDiagramEditor>();

			_openDiagram = Property.New(this, p => p.OpenDiagram, OnPropertyChanged);

			_closingDiagram = Property.New(this, p => p.ClosingDiagram, OnPropertyChanged);

			SaveClosingDiagramCommand = new RelayCommand(() => _editorsNeedingSaving.Add(ClosingDiagram));
			OpenDiagramCommand = new RelayCommand<PreviewDiagramViewModel>(OpenDiagramForEdit, d => d != null);
			CloseCommand = new RelayCommand(Close);
			SaveAllCommand = new BoundRelayCommand<DiagramManagerViewModel>(_ => SaveAll(), p => p.CanSaveAll, this);

			_explorer.OpenPreviewRequested += explorer_OpenPreviewRequested;
		}

		void explorer_OpenPreviewRequested(object sender, OpenPreviewRequestedEventArgs e)
		{
			OpenDiagramForEdit(e.RequestedPreview);
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
		public ICommand OpenDiagramCommand { get; private set; }

		private void OpenDiagramForEdit(PreviewDiagramViewModel diagram)
		{
			var diagramEditor = OpenDiagrams.FirstOrDefault(d => d.Diagram.Equals(diagram.Diagram));
			if (diagramEditor == null)
			{
				diagramEditor = _editorFactory(diagram);
				diagramEditor.Closing += diagramEditor_Closing;
				diagramEditor.Closed += diagramEditor_Closed;
				diagramEditor.Saved += diagramEditor_Saved;
				diagramEditor.PropertyChanged += diagramEditor_PropertyChanged;
				OpenDiagrams.Add(diagramEditor);
			}

			OpenDiagram = diagramEditor;
		}

		void diagramEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == canSavePropertyName)
				OnPropertyChanged(canSaveAllPropertyName);
		}
		private static readonly string canSavePropertyName = Reflect.PropertyOf<IDiagramEditor>(p => p.CanSave).Name;

		/// <summary>
		/// Whether any diagrams need saving.
		/// </summary>
		public bool CanSaveAll
		{
			get { return OpenDiagrams.Any(d => d.CanSave); }
		}
		private static readonly string canSaveAllPropertyName = Reflect.PropertyOf<DiagramManagerViewModel>(p => p.CanSaveAll).Name;

		/// <summary>
		/// Command to save all modified open diagrams.
		/// </summary>
		public ICommand SaveAllCommand { get; private set; }

		private async void SaveAll()
		{
			var modifiedEditors = OpenDiagrams.Where(d => d.CanSave).ToList();
			await Task.WhenAll(modifiedEditors.Select(d => d.SaveAsync()));
			OnPropertyChanged(canSaveAllPropertyName);
		}

		void diagramEditor_Saved(object sender, EventArgs e)
		{
			var diagramEditor = (IDiagramEditor)sender;
			var preview = Explorer.PreviewDiagrams.FirstOrDefault(d => d.Diagram.Equals(diagramEditor.Diagram));
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
				SaveClosingEditor(diagramEditor);
			}
			else
			{
				RemoveEditor(diagramEditor);
			}
		}

		private async void SaveClosingEditor(IDiagramEditor diagramEditor)
		{
			var saveTask = diagramEditor.SaveAsync();
			_editorSaveTasks.Add(saveTask);
			_editorsNeedingSaving.Remove(diagramEditor);
			await saveTask;
			_editorSaveTasks.Remove(saveTask);
			RemoveEditor(diagramEditor);
		}

		private void RemoveEditor(IDiagramEditor editor)
		{
			editor.PropertyChanged -= diagramEditor_PropertyChanged;
			editor.Closing -= diagramEditor_Closing;
			editor.Closed -= diagramEditor_Closed;
			editor.Saved -= diagramEditor_Saved;
			OpenDiagrams.Remove(editor);
			editor.Dispose();
		}

		/// <summary>
		/// Saves the currently closing diagram.
		/// </summary>
		public ICommand SaveClosingDiagramCommand { get; private set; }

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
		public ICommand CloseCommand { get; private set; }

		private void Close()
		{
			var unsavedOpenDiagrams = OpenDiagrams.Where(od => od.CodeEditor.IsModified).ToList();
			foreach (var openDiagram in unsavedOpenDiagrams)
			{
				openDiagram.Close();
			}

			Task.WaitAll(_editorSaveTasks.ToArray());

			_settings.Save();
		}

		/// <summary>
		/// Diagram previews.
		/// </summary>
		public IDiagramExplorer Explorer
		{
			get { return _explorer; }
		}

		private readonly Property<IDiagramEditor> _openDiagram;
		private readonly Property<ICollection<IDiagramEditor>> _openDiagrams;
		private readonly Property<IDiagramEditor> _closingDiagram;

		private readonly ICollection<IDiagramEditor> _editorsNeedingSaving = new HashSet<IDiagramEditor>();
		private readonly ICollection<Task> _editorSaveTasks = new HashSet<Task>();

		private readonly IDiagramExplorer _explorer;
		private readonly Func<PreviewDiagramViewModel, IDiagramEditor> _editorFactory;
		private readonly ISettings _settings;
	}
}