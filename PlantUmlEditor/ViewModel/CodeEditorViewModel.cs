using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using Utilities.Mvvm;
using Utilities.PropertyChanged;
using Utilities.Reflection;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram code editor based on an AvalonEdit control.
	/// </summary>
	public class CodeEditorViewModel : ViewModelBase, ICodeEditor
	{
		/// <summary>
		/// Initializes a new code editor.
		/// </summary>
		public CodeEditorViewModel(AbstractFoldingStrategy foldingStrategy, IHighlightingDefinition highlightingDefinition, IEnumerable<MenuViewModel> snippets)
		{
			FoldingStrategy = foldingStrategy;
			HighlightingDefinition = highlightingDefinition;
			Snippets = snippets;

			_contentIndex = Property.New(this, p => p.ContentIndex, OnPropertyChanged);
			_contentIndex.Value = 0;

			_selectionStart = Property.New(this, p => p.SelectionStart, OnPropertyChanged);
			_selectionLength = Property.New(this, p => p.SelectionLength, OnPropertyChanged);

			_document = Property.New(this, p => p.Document, OnPropertyChanged);

			_scrollOffset = Property.New(this, p => p.ScrollOffset, OnPropertyChanged);

			_isModified = Property.New(this, p => IsModified, OnPropertyChanged);
		}

		/// <summary>
		/// The editor folding strategy.
		/// </summary>
		public AbstractFoldingStrategy FoldingStrategy { get; private set; }

		/// <summary>
		/// The code highlighting definition.
		/// </summary>
		public IHighlightingDefinition HighlightingDefinition { get; private set; }

		/// <summary>
		/// The available code snippets.
		/// </summary>
		public IEnumerable<MenuViewModel> Snippets { get; private set; }

		/// <summary>
		/// The text document representing diagram code.
		/// </summary>
		public TextDocument Document
		{
			get { return _document.Value; }
			set { _document.Value = value; }
		}

		/// <summary>
		/// The content being edited.
		/// </summary>
		public string Content
		{
			get { return Document.Text; }
			set 
			{
				if (Document == null)
				{
					Document = new TextDocument(value);
					Document.UndoStack.PropertyChanged += UndoStack_PropertyChanged;
					Document.Changed += Document_Changed;
				}
				else
				{
					Document.Text = value;
				}
			}
		}

		void UndoStack_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == isOriginalFilePropertyName)
				_isModified.Value = !Document.UndoStack.IsOriginalFile;
		}
		private static readonly string isOriginalFilePropertyName = Reflect.PropertyOf<UndoStack>(p => p.IsOriginalFile).Name;

		void Document_Changed(object sender, DocumentChangeEventArgs e)
		{
			OnPropertyChanged(contentPropertyName);
		}
		private static readonly string contentPropertyName = Reflect.PropertyOf<CodeEditorViewModel>(p => p.Content).Name;

		/// <summary>
		/// The current index into the content.
		/// </summary>
		public int ContentIndex
		{
			get { return _contentIndex.Value; }
			set { _contentIndex.Value = value; }
		}

		/// <summary>
		/// The current selection start position.
		/// </summary>
		public int SelectionStart
		{
			get { return _selectionStart.Value; }
			set { _selectionStart.Value = value; }
		}

		/// <summary>
		/// The current selection length.
		/// </summary>
		public int SelectionLength
		{
			get { return _selectionLength.Value; }
			set { _selectionLength.Value = value; }
		}

		/// <summary>
		/// The current scroll offset of a code editor.
		/// </summary>
		public Vector ScrollOffset
		{
			get { return _scrollOffset.Value; }
			set { _scrollOffset.Value = value; }
		}


		/// <summary>
		/// Whether content has been modified since the last save.
		/// </summary>
		public bool IsModified
		{
			get { return _isModified.Value; }
			set 
			{
				if (_isModified.TrySetValue(value))
				{
					if (!value)
						Document.UndoStack.MarkAsOriginalFile();
				}
			}
		}

		/// <see cref="ViewModelBase.Dispose(bool)"/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!_disposed)
				{
					if (Document != null)
					{
						Document.UndoStack.PropertyChanged -= UndoStack_PropertyChanged;
						Document.Changed -= Document_Changed;
					}

					_disposed = true;
				}
			}
		}

		private bool _disposed;

		private readonly Property<int> _contentIndex;
		private readonly Property<int> _selectionStart;
		private readonly Property<int> _selectionLength; 
		private readonly Property<TextDocument> _document;
		private readonly Property<Vector> _scrollOffset;
		private readonly Property<bool> _isModified;
	}
}