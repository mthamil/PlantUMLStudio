﻿using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Utilities.Controls.Behaviors.AvalonEdit;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram code editor.
	/// </summary>
	public class CodeEditorViewModel : ViewModelBase, ICodeEditor
	{
		/// <summary>
		/// Initializes a new code editor.
		/// </summary>
		public CodeEditorViewModel(AbstractFoldingStrategy foldingStrategy, Uri highlightingDefinition, IEnumerable<MenuViewModel> snippets)
		{
			FoldingStrategy = foldingStrategy;
			HighlightingDefinition = highlightingDefinition;
			Snippets = snippets;

			_content = Property.New(this, p => p.Content, OnPropertyChanged);

			_contentIndex = Property.New(this, p => p.ContentIndex, OnPropertyChanged);
			_contentIndex.Value = 0;

			_isModified = Property.New(this, p => IsModified, OnPropertyChanged);
		}

		/// <summary>
		/// The editor folding strategy.
		/// </summary>
		public AbstractFoldingStrategy FoldingStrategy { get; private set; }

		/// <summary>
		/// The location of the code highlighting definition.
		/// </summary>
		public Uri HighlightingDefinition { get; private set; }

		/// <summary>
		/// The available code snippets.
		/// </summary>
		public IEnumerable<MenuViewModel> Snippets { get; private set; }

		/// <summary>
		/// The content being edited.
		/// </summary>
		public string Content
		{
			get { return _content.Value; }
			set
			{
				if (_content.TrySetValue(value))
				{
					if (_originalContent != null)
						IsModified = true;//!Equals(_content.Value, _originalContent);	// could be perf issue
					else
						_originalContent = _content.Value;
				}
			}
		}

		/// <summary>
		/// The current index into the content.
		/// </summary>
		public int ContentIndex
		{
			get { return _contentIndex.Value; }
			set { _contentIndex.Value = value; }
		}

		/// <summary>
		/// Whether content has been modified since the last save.
		/// </summary>
		public bool IsModified
		{
			get { return _isModified.Value; }
			set { _isModified.Value = value; }
		}

		#region Implementation of IUndoProvider

		/// <see cref="IUndoProvider.UndoStack"/>
		public UndoStack UndoStack
		{
			get { return _undoStack; }
		}

		#endregion

		private string _originalContent;

		private readonly UndoStack _undoStack = new UndoStack();

		private readonly Property<string> _content;
		private readonly Property<int> _contentIndex;
		private readonly Property<bool> _isModified;
	}
}