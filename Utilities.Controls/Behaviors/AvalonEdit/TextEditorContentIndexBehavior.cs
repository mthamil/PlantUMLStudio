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

		internal void UpdateIndex(int index, bool firstUpdate)
		{
			// If the change came from the editor itself, don't update.
			if (!_lastUpdateFromControl)
			{
				if (0 <= index && index <= _editor.Text.Length) // TODO: Really fix this.
				{
					_editor.Select(index, 0);
					//_editor.TextArea.Caret.BringCaretToView();
				}

				if (!firstUpdate)
					_lastUpdateFromBinding = true;
			}
			else
			{
				_lastUpdateFromControl = false;
			}
		}

		void caret_PositionChanged(object sender, EventArgs e)
		{
			var caret = sender as Caret;
			if (caret == null)
				return;

			if (!_dataContextChanged)
			{
				if (!_lastUpdateFromBinding)
				{
					_lastUpdateFromControl = true;
					AvalonEditor.SetContentIndex(_editor, caret.Offset);
				}
				else
				{
					_lastUpdateFromBinding = false;
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
			_lastUpdateFromControl = false;
			_lastUpdateFromBinding = false;
		}

		private bool _lastUpdateFromControl;
		private bool _lastUpdateFromBinding;
		private bool _dataContextChanged;

		private readonly TextEditor _editor;
	}
}