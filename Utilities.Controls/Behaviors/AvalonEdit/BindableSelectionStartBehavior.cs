using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable selection start property.
	/// </summary>
	internal class BindableSelectionStartBehavior
	{
		public BindableSelectionStartBehavior(TextEditor editor)
		{
			_editor = editor;
			editor.TextArea.SelectionChanged += TextArea_SelectionChanged;
			editor.DataContextChanged += editor_DataContextChanged;
		}

		void editor_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			_dataContextChanged = true;
			_lastUpdateFromBinding = false;
			_lastUpdateFromControl = false;
		}

		void TextArea_SelectionChanged(object sender, EventArgs e)
		{
			if (!_dataContextChanged)
			{
				if (!_lastUpdateFromBinding)
				{
					_lastUpdateFromControl = true;
					AvalonEditor.SetBindableSelectionStart(_editor, _editor.SelectionStart);
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

		internal void UpdateSelectionStart(int selectionStart)
		{
			// If the change came from the editor itself, don't update.
			if (!_lastUpdateFromControl)
			{
				_lastUpdateFromBinding = true;
				if (selectionStart > -1)
					_editor.SelectionStart = selectionStart;
			}
			else
			{
				_lastUpdateFromControl = false;
			}
		}

		private bool _lastUpdateFromControl;
		private bool _lastUpdateFromBinding;
		private bool _dataContextChanged;

		private readonly TextEditor _editor;
	}
}