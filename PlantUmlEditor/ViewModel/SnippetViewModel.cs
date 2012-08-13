using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Snippets;
using PlantUmlEditor.Model.Snippets;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a code snippet.
	/// </summary>
	public class SnippetViewModel : MenuViewModel
	{
		public SnippetViewModel(CodeSnippet codeSnippet)
		{
			_snippet = codeSnippet.Code;
			Name = codeSnippet.Name;
			Command = new RelayCommand<TextEditor>(Insert);
		}

		private void Insert(TextEditor editor)
		{
			_snippet.Insert(editor.TextArea);
		}

		private readonly Snippet _snippet;
	}
}