using System.Windows.Input;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using PlantUmlEditor.Model.Snippets;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a code snippet.
	/// </summary>
	public class SnippetViewModel : SnippetCategoryViewModel
	{
		public SnippetViewModel(CodeSnippet codeSnippet)
			: base(codeSnippet.Name)
		{
			_snippet = codeSnippet.Code;
			InsertCommand = new RelayCommand<TextArea>(Insert);
		}

		/// <summary>
		/// A command that inserts a code snippet.
		/// </summary>
		public ICommand InsertCommand { get; private set; }

		private void Insert(TextArea editor)
		{
			_snippet.Insert(editor);
		}

		private readonly Snippet _snippet;
	}
}