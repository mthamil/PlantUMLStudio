using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace PlantUmlEditor.Controls.Behaviors
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
		}

		internal void UpdateIndex(int index)
		{
			// If the change came from the editor itself, don't update.
			if (!_positionChanged)
			{
				_indexBindingChanged = true;

				if (index <= _editor.Text.Length)
					_editor.TextArea.Caret.Offset = index;
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

		private bool _positionChanged;
		private bool _indexBindingChanged;

		private readonly TextEditor _editor;
	}
}