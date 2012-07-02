using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable UndoStack property.
	/// </summary>
	internal class TextEditorUndoStackBehavior
	{
		public TextEditorUndoStackBehavior(TextEditor editor)
		{
			_editor = editor;
			editor.DataContextChanged += editor_DataContextChanged;
		}

		internal void UpdateUndoStack(UndoStack undoStack)
		{
			if (!_dataContextChanged)
			{
				if (undoStack != null)
					_editor.Document.UndoStack = undoStack;
			}
			else
			{
				_dataContextChanged = false;
			}
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var undoProvider = e.NewValue as IUndoProvider;
			if (undoProvider == null)
				return;

			_dataContextChanged = true;
			_editor.Document.UndoStack = undoProvider.UndoStack;
		}

		private bool _dataContextChanged;

		private readonly TextEditor _editor;
	}
}