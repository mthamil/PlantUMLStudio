//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Utilities.Controls.Behaviors.AvalonEdit.Folding
{
	/// <summary>
	/// Creates folding regions based on start and end pattern definitions.
	/// </summary>
	public class PatternBasedFoldingStrategy : AbstractFoldingStrategy
	{
		/// <summary>
		/// Initializes a new folding strategy.
		/// </summary>
		/// <param name="foldRegions">The patterns that define fold regions</param>
		public PatternBasedFoldingStrategy(IEnumerable<FoldedRegionDefinition> foldRegions)
		{
			int counter = 1;
			tokens = foldRegions.ToDictionary(region => counter++.ToString(), region => region);

			startTokens = new Regex(String.Join("|", tokens.Select(t => String.Format(@"(?<{0}>{1})", t.Key, t.Value.StartPattern))),
				RegexOptions.ExplicitCapture);
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

				// Determine if there is a match for a start token.
				var startMatch = TryMatchStartToken(lineText);
				if (startMatch != null)
				{
					var foldDefinition = startMatch.Item1;
					var match = startMatch.Item2;
					if (match.Success && !Regex.IsMatch(lineText, foldDefinition.EndPattern))
					{
						int startOffset = line.Offset + match.Index;
						string foldedDisplay = lineText.Substring(match.Index, Math.Min(lineText.Length, 15));

						// If the start pattern specifies an identifier that must be included in the end pattern,
						// construct the end pattern using this identifier.
						string identifier = match.Groups["id"].Success ? match.Groups["id"].Value + @"($|\s+)" : string.Empty;

						openRegions.Push(new PotentialFoldRegion(startOffset, foldedDisplay)
						{
							StartToken = foldDefinition.StartPattern,
							EndToken = foldDefinition.EndPattern + identifier
						});
					}
				}

				if (openRegions.Count > 0)
				{
					if (Regex.IsMatch(lineText, openRegions.Peek().EndToken))
					{
						var region = openRegions.Pop();
						foldings.Add(new NewFolding(region.StartOffset, line.Offset + line.Length) { Name = region.StartLine + "..." });
					}
				}
			}
			return foldings.OrderBy(f => f.StartOffset);
		}

		#endregion

		private Tuple<FoldedRegionDefinition, Match> TryMatchStartToken(string input)
		{
			var matches = startTokens.Matches(input);
			if (matches.Count > 0 && matches[0].Success)
			{
				foreach (string groupName in tokens.Keys)
				{
					if (matches[0].Groups[groupName].Success)
					{
						FoldedRegionDefinition foldDefinition;
						tokens.TryGetValue(groupName, out foldDefinition);
						return Tuple.Create(foldDefinition, matches[0]);
					}
				}
			}

			return null;
		}

		private readonly IDictionary<string, FoldedRegionDefinition> tokens;

		/// <summary>
		/// A pattern that can match any of the start tokens.
		/// </summary>
		private readonly Regex startTokens;

		private sealed class PotentialFoldRegion
		{
			public PotentialFoldRegion(int startOffset, string startLine)
			{
				StartOffset = startOffset;
				StartLine = startLine;
			}

			public string StartToken { get; set; }
			public string EndToken { get; set; }

			public int StartOffset { get; private set; }
			public string StartLine { get; private set; }
		}
	}
}