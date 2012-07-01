using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Creates folding regions for PlantUML diagrams.
	/// </summary>
	public class PlantUmlFoldingStrategy : AbstractFoldingStrategy
	{
		/// <summary>
		/// Initializes a new strategy.
		/// </summary>
		public PlantUmlFoldingStrategy()
		{
			startTokens = String.Join("|", tokens.Select(t => t.Key));
		}

		#region Overrides of AbstractFoldingStrategy

		/// <see cref="AbstractFoldingStrategy.CreateNewFoldings"/>
		public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
		{
			firstErrorOffset = -1;
			var foldings = new List<NewFolding>();

			var openRegions = new Stack<PotentialFoldRegion>();
			foreach (var line in document.Lines.Where(l => l.Length > 0))	// Filter out empty lines.
			{
				var lineText = document.GetText(line.Offset, line.Length);	// Get the line's text content.
				foreach (var tokenPair in tokens)
				{
					// Try to find a start token match.
					var match = Regex.Match(lineText, tokenPair.Key);
					if (match.Success && !Regex.IsMatch(lineText, tokenPair.Value))
					{
						int startOffset = line.Offset + match.Index;
						string foldedDisplay = lineText.Substring(match.Index, Math.Min(lineText.Length, 15));

						// If the start pattern specifies an identifier that must be included in the end pattern,
						// construct the end pattern using this identifier.
						string identifier = match.Groups["id"].Success ? match.Groups["id"].Value : string.Empty;

						openRegions.Push(new PotentialFoldRegion(startOffset, foldedDisplay)
						{
							StartToken = tokenPair.Key,
							EndToken = tokenPair.Value + identifier
						});
					}
				}

				if (openRegions.Count > 0)
				{
					if (Regex.IsMatch(lineText, openRegions.Peek().EndToken))
					{
						var region = openRegions.Pop();
						foldings.Add(new NewFolding(region.Start, line.Offset + line.Length) { Name = region.StartLine + "..." });
					}
				}
			}
			return foldings.OrderBy(f => f.StartOffset);
		}

		#endregion

		private static readonly IDictionary<string, string> tokens = new Dictionary<string, string>
		{
			{ @"(^|\s+)note left", @"(^|\s+)end note" },
			{ @"(^|\s+)note right", @"(^|\s+)end note" },
			{ @"(^|\s+)package", @"(^|\s+)end package" },
			{ @"(^|\s+)activate\s+(?<id>\w+)", @"(^|\s+)deactivate " },
			{ @"(^|\s+)if.+then", @"(^|\s+)endif($|\s+)" }
		};

		private readonly string startTokens;

		private class PotentialFoldRegion
		{
			public PotentialFoldRegion(int start, string startLine)
			{
				Start = start;
				StartLine = startLine;
			}

			public string StartToken { get; set; }
			public string EndToken { get; set; }
			public int Start { get; private set; }
			public string StartLine { get; private set; }
		}
	}
}