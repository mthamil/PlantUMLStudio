//  PlantUML Editor
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
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
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using Utilities.Collections;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Manages open diagrams and also serves as the main application driver.
	/// </summary>
	public class DiagramManagerViewModel : ViewModelBase
	{
		public DiagramManagerViewModel(IDiagramExplorer explorer, Func<Diagram, IDiagramEditor> editorFactory, ISettings settings)
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
			SaveAllCommand = new AggregateBoundRelayCommand<DiagramManagerViewModel, IDiagramEditor, IEnumerable<IDiagramEditor>>(
				_ => SaveAll(),
				p => p.OpenDiagrams,
				c => c.Any(p => p.CanSave), this);

			_explorer.OpenPreviewRequested += explorer_OpenPreviewRequested;
		}

		/// <summary>
		/// Initializes the diagram manager.
		/// </summary>
		public async Task InitializeAsync()
		{
			// Restore previously opened files.
			if (_settings.RememberOpenFiles)
				await _settings.OpenFiles.Select(f => _explorer.OpenDiagramAsync(new Uri(f.FullName))).ToList();
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
				diagramEditor = _editorFactory(diagram.Diagram);
				diagramEditor.DiagramImage = diagram.ImagePreview;
				diagramEditor.Closing += diagramEditor_Closing;
				diagramEditor.Closed += diagramEditor_Closed;
				diagramEditor.Saved += diagramEditor_Saved;
				OpenDiagrams.Add(diagramEditor);
			}

			OpenDiagram = diagramEditor;
		}

		/// <summary>
		/// Command to save all modified open diagrams.
		/// </summary>
		public ICommand SaveAllCommand { get; private set; }

		private async void SaveAll()
		{
			await OpenDiagrams.Where(d => d.CanSave).Select(d => d.SaveAsync());
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
		/// Command executed when closing a diagram manager.
		/// </summary>
		public ICommand CloseCommand { get; private set; }

		/// <summary>
		/// Closes a diagram manager.
		/// </summary>
		private void Close()
		{
			if (_settings.RememberOpenFiles)
				_settings.OpenFiles = OpenDiagrams.Select(diagram => diagram.Diagram.File).ToList();

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
		private readonly Func<Diagram, IDiagramEditor> _editorFactory;
		private readonly ISettings _settings;
	}
}