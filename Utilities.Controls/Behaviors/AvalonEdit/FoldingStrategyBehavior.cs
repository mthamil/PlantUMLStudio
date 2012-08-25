using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Behavior for managing a text editor's folding strategy.
	/// </summary>
	/// <remarks>This is a mess!</remarks>
	internal class FoldingStrategyBehavior
	{
		/// <summary>
		/// Initializes a folding strategy behavior for a text editor.
		/// </summary>
		/// <param name="editor">The text editor</param>
		/// <param name="foldingStrategy">The folding strategy to use</param>
		public FoldingStrategyBehavior(TextEditor editor, AbstractFoldingStrategy foldingStrategy)
		{
			_editor = editor;
			_foldingStrategy = foldingStrategy;

			editor.DataContextChanged += editor_DataContextChanged;
			editor.DocumentChanged += editor_DocumentChanged;

			EditorDocumentChanged();
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_currentFoldingManager != null)
			{
				// Currently the DefaultClosed property does not seem to be respected when foldings are restored.
				var foldings = _currentFoldingManager
					.AllFoldings
					.Select(f => new NewFolding(f.StartOffset, f.EndOffset) { DefaultClosed = f.IsFolded, Name = f.Title });

				_documents.Remove(_currentDocument);
				_documents.Add(_currentDocument, foldings.ToList());
				WeakEventManager<TextDocument, DocumentChangeEventArgs>.RemoveHandler(_currentDocument, "Changed", document_Changed);
				FoldingManager.Uninstall(_currentFoldingManager);
			}
		}

		void editor_DocumentChanged(object sender, EventArgs e)
		{
			EditorDocumentChanged();
		}

		private void EditorDocumentChanged()
		{
			_currentDocument = _editor.Document;
			if (_currentDocument != null)
			{
				_currentFoldingManager = FoldingManager.Install(_editor.TextArea);
				_foldingStrategy.UpdateFoldings(_currentFoldingManager, _currentDocument);

				IEnumerable<NewFolding> foldings;
				if (_documents.TryGetValue(_currentDocument, out foldings))
					_currentFoldingManager.UpdateFoldings(foldings, -1);
				else
					_documents.Add(_currentDocument, Enumerable.Empty<NewFolding>());
				
				WeakEventManager<TextDocument, DocumentChangeEventArgs>.AddHandler(_editor.Document, "Changed", document_Changed);
			}
		}

		void document_Changed(object sender, DocumentChangeEventArgs e)
		{
			var document = sender as TextDocument;
			if (document == null)
				return;

			if (document != _editor.Document)
				return;

			_foldingStrategy.UpdateFoldings(_currentFoldingManager, document);
		}

		private FoldingManager _currentFoldingManager;
		private TextDocument _currentDocument;
		private readonly TextEditor _editor;
		private readonly AbstractFoldingStrategy _foldingStrategy;

		private readonly ConditionalWeakTable<TextDocument, IEnumerable<NewFolding>> _documents = new ConditionalWeakTable<TextDocument, IEnumerable<NewFolding>>();
	}
}