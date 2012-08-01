using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	public class StubFoldingStrategy : AbstractFoldingStrategy
	{
		#region Overrides of AbstractFoldingStrategy

		public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}