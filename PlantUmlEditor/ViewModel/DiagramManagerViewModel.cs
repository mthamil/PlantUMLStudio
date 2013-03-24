//  PlantUML Editor
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
	public class DiagramManagerViewModel : ViewModelBase, IDiagramManager
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
			SaveAllCommand = Command.For(this)
			                        .DependsOnCollection(p => p.OpenDiagrams)
			                        .When(c => c.Any(p => p.CanSave))
									.Asynchronously()
			                        .Executes(SaveAllAsync);

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

		private void explorer_OpenPreviewRequested(object sender, OpenPreviewRequestedEventArgs e)
		{
			OpenDiagramForEdit(e.RequestedPreview);
		}

		/// <see cref="IDiagramManager.OpenDiagram"/>
		public IDiagramEditor OpenDiagram
		{
			get { return _openDiagram.Value; }
			set { _openDiagram.Value = value; }
		}

		/// <see cref="IDiagramManager.OpenDiagrams"/>
		public ICollection<IDiagramEditor> OpenDiagrams
		{
			get { return _openDiagrams.Value; }
		}

		/// <summary>
		/// Command to open a diagram for editing.
		/// </summary>
		public ICommand OpenDiagramCommand { get; private set; }

		/// <see cref="IDiagramManager.OpenDiagramForEdit"/>
		public void OpenDiagramForEdit(PreviewDiagramViewModel diagram)
		{
			if (diagram == null)
				throw new ArgumentNullException("diagram");

			OpenDiagram = OpenDiagrams.FirstOrNone(d => d.Diagram.Equals(diagram.Diagram)).GetOrElse(() =>
			{
				var newEditor = _editorFactory(diagram.Diagram);
				newEditor.DiagramImage = diagram.ImagePreview;
				newEditor.Closing += diagramEditor_Closing;
				newEditor.Closed += diagramEditor_Closed;
				newEditor.Saved += diagramEditor_Saved;
				OpenDiagrams.Add(newEditor);
				return newEditor;
			});
		}

		/// <see cref="IDiagramManager.DiagramClosed"/>
		public event EventHandler<DiagramClosedEventArgs> DiagramClosed;

		private void OnDiagramClosed(Diagram diagram)
		{
			var localEvent = DiagramClosed;
			if (localEvent != null)
				localEvent(this, new DiagramClosedEventArgs(diagram));
		}

		/// <summary>
		/// Command to save all modified open diagrams.
		/// </summary>
		public ICommand SaveAllCommand { get; private set; }

		/// <see cref="IDiagramManager.SaveAllAsync"/>
		public async Task SaveAllAsync()
		{
			await OpenDiagrams.Where(d => d.CanSave).Select(d => d.SaveAsync());
		}

		void diagramEditor_Saved(object sender, EventArgs e)
		{
			var diagramEditor = (IDiagramEditor)sender;
			Explorer.PreviewDiagrams.FirstOrNone(d => d.Diagram.Equals(diagramEditor.Diagram)).Apply(preview =>
			{
				preview.ImagePreview = diagramEditor.DiagramImage; // Update preview with new image.

				// If for some reason the matching preview's Diagram is a different instance than the 
				// editor's Diagram (which may occur if the preview list was refreshed or the diagram was
				// restored through the 'restore open diagrams' feature), update the preview's Diagram's
				// data.
				if (!ReferenceEquals(preview.Diagram, diagramEditor.Diagram))
				{
					preview.Diagram.Content = diagramEditor.Diagram.Content;
					preview.Diagram.ImageFile = diagramEditor.Diagram.ImageFile;
				}
			});
		}

		void diagramEditor_Closing(object sender, CancelEventArgs e)
		{
			ClosingDiagram = null;
			ClosingDiagram = (IDiagramEditor)sender;
		}

		async void diagramEditor_Closed(object sender, EventArgs e)
		{
			var diagramEditor = (IDiagramEditor)sender;
			if (_editorsNeedingSaving.Contains(diagramEditor))
			{
				await SaveClosingEditorAsync(diagramEditor);
			}
			else
			{
				RemoveEditor(diagramEditor);
			}
		}

		private async Task SaveClosingEditorAsync(IDiagramEditor diagramEditor)
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
			var diagram = editor.Diagram;

			editor.Closing -= diagramEditor_Closing;
			editor.Closed -= diagramEditor_Closed;
			editor.Saved -= diagramEditor_Saved;
			OpenDiagrams.Remove(editor);
			editor.Dispose();

			OnDiagramClosed(diagram);
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

		/// <see cref="IDiagramManager.Close"/>
		public void Close()
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