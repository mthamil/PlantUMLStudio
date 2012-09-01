using System;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable selection length property.
	/// </summary>
	internal class BindableSelectionLengthBehavior
	{
		public BindableSelectionLengthBehavior(TextEditor editor)
		{
			_editor = editor;
			editor.TextArea.SelectionChanged += TextArea_SelectionChanged;
			editor.DataContextChanged += editor_DataContextChanged;
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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
					AvalonEditor.SetBindableSelectionLength(_editor, _editor.SelectionLength);
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

		internal void UpdateSelectionLength(int selectionLength)
		{
			// If the change came from the editor itself, don't update.
			if (!_lastUpdateFromControl)
			{
				_lastUpdateFromBinding = true;
				if (selectionLength > -1)
					_editor.SelectionLength = selectionLength;
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