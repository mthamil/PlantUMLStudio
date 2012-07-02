using System;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable content index property.
	/// </summary>
	internal class TextEditorContentIndexBehavior
	{
		public TextEditorContentIndexBehavior(TextEditor editor)
		{
			_editor = editor;
			editor.TextArea.Caret.PositionChanged += caret_PositionChanged;
			editor.DataContextChanged += editor_DataContextChanged;
		}

		internal void UpdateIndex(int index)
		{
			// If the change came from the editor itself, don't update.
			if (!_positionChanged)
			{
				_indexBindingChanged = true;

				if (0 <= index && index <= _editor.Text.Length)	 // TODO: Really fix this.
				{
					_editor.TextArea.Caret.Offset = index;
					_editor.TextArea.Caret.BringCaretToView();
				}
			}
			else
			{
				_positionChanged = false;
			}
		}

		void caret_PositionChanged(object sender, EventArgs e)
		{
			var caret = sender as Caret;
			if (caret == null)
				return;

			if (!_dataContextChanged)
			{
				if (!_indexBindingChanged)
				{
					_positionChanged = true;
					AvalonEditor.SetContentIndex(_editor, caret.Offset);
				}
				else
				{
					_indexBindingChanged = false;
				}
			}
			else
			{
				_dataContextChanged = false;
			}
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_dataContextChanged = true;
			_positionChanged = false;
			_indexBindingChanged = false;
		}

		private bool _positionChanged;
		private bool _indexBindingChanged;
		private bool _dataContextChanged;

		private readonly TextEditor _editor;
	}
}