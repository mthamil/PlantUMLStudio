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

		//var loopCounter = new SnippetReplaceableTextElement { Text = "i" };
		//Snippet snippet = new Snippet {
		//Elements = {
		//new SnippetTextElement { Text = "for(int " },
		//new SnippetBoundElement { TargetElement = loopCounter },
		//new SnippetTextElement { Text = " = " },
		//new SnippetReplaceableTextElement { Text = "0" },
		//new SnippetTextElement { Text = "; " },
		//loopCounter,
		//new SnippetTextElement { Text = " < " },
		//new SnippetReplaceableTextElement { Text = "end" },
		//new SnippetTextElement { Text = "; " },
		//new SnippetBoundElement { TargetElement = loopCounter },
		//new SnippetTextElement { Text = "++) { \t" },
		//new SnippetCaretElement(),
		//new SnippetTextElement { Text = " }" }
		//    }
		//};
		//snippet.Insert(textEditor.TextArea);

		private readonly ICSharpCode.AvalonEdit.Snippets.Snippet _snippet;
	}
}