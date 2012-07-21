using System.Windows.Input;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using Utilities.Mvvm.Commands;
using Snippet = PlantUmlEditor.Model.Snippets.Snippet;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a code snippet.
	/// </summary>
	public class SnippetViewModel : SnippetCategoryViewModel
	{
		public SnippetViewModel(Snippet snippet)
			: base(snippet.Name)
		{
			_snippet = snippet.Code;
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

		private readonly ICSharpCode.AvalonEdit.Snippets.Snippet _snippet;
	}
}