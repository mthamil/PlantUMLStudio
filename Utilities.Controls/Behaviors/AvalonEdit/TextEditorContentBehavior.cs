using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable content property.
	/// </summary>
	internal class TextEditorContentBehavior
	{
		public TextEditorContentBehavior(TextEditor editor)
		{
			_editor = editor;
			editor.Document.TextChanged += contentEditor_TextChanged;
		}

		internal void UpdateContent(string content)
		{
			// If the change came from the editor itself, don't update.
			if (!_textChanged)
			{
				_contentBindingChanged = true;
				_editor.Document.Text = content;
			}
			else
			{
				_textChanged = false;
			}
		}

		void contentEditor_TextChanged(object sender, EventArgs e)
		{
			var document = sender as TextDocument;
			if (document == null)
				return;

			if (!_contentBindingChanged)
			{
				_textChanged = true;
				AvalonEditor.SetContent(_editor, document.Text);
			}
			else
			{
				_contentBindingChanged = false;
			}
		}

		private bool _textChanged;
		private bool _contentBindingChanged;

		private readonly TextEditor _editor;
	}
}