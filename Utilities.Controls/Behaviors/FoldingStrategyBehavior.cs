using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Behavior for managing a text editor's folding strategy.
	/// </summary>
	internal class FoldingStrategyBehavior
	{
		/// <summary>
		/// Initializes a folding strategy behavior for a text editor.
		/// </summary>
		/// <param name="document">The document being folded</param>
		/// <param name="textArea">The associated text area</param>
		/// <param name="foldingStrategy">The folding strategy to use</param>
		public FoldingStrategyBehavior(TextDocument document, TextArea textArea, AbstractFoldingStrategy foldingStrategy)
		{
			_document = document;
			_foldingStrategy = foldingStrategy;
			_foldingManager = FoldingManager.Install(textArea);

			_document.TextChanged += document_TextChanged;

			foldingStrategy.UpdateFoldings(_foldingManager, _document);
		}

		void document_TextChanged(object sender, EventArgs e)
		{
			_foldingStrategy.UpdateFoldings(_foldingManager, _document);
		}

		private readonly FoldingManager _foldingManager;
		private readonly TextDocument _document;
		private readonly AbstractFoldingStrategy _foldingStrategy;
	}
}