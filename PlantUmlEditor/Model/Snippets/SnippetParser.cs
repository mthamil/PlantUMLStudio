using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Snippets;
using Utilities.InputOutput;

namespace PlantUmlEditor.Model.Snippets
{
	/// <summary>
	/// Parses code snippets.
	/// </summary>
	public class SnippetParser : ISnippetParser
	{
		/// <see cref="ISnippetParser.Parse"/>
		public CodeSnippet Parse(Stream snippetSource)
		{
			string name = null;
			string category = null;

			var symbols = new Dictionary<string, SnippetReplaceableTextElement>();
			var root = new Snippet();

			foreach (string line in snippetSource.Lines())
			{
				if (name != null && category != null)
					ParseCodeLine(line, root, symbols);

				if (name == null || category == null)
				{
					var tokens = line.Trim().Split(Delimiter);
					string first = tokens[0].Trim().ToLower();

					if (first == NameToken)
					{
						name = tokens[1].Trim();
					}
					else if (first == CategoryToken)
					{
						category = tokens[1].Trim();
					}
				}
			}

			return new CodeSnippet(name, category, root);
		}

		private static void ParseCodeLine(string line, Snippet root, IDictionary<string, SnippetReplaceableTextElement> symbols)
		{
			var matchResult = ReplaceableTokenPattern.Matches(line);
			var matches = matchResult.Cast<Match>().Where(m => m.Success).OrderBy(m => m.Index).ToArray();
			if (matchResult.Count > 0)
			{
				int lastIndex = 0;
				foreach (var match in matches)
				{
					var group = match.Groups["token"];
					string token = group.Value;

					if (lastIndex < group.Index)
					{
						root.Elements.Add(new SnippetTextElement { Text = line.Substring(lastIndex, group.Index - lastIndex) });
					}

					switch (token)
					{
						case "%END%":
						{
							// %END% is a special case token that describes where to place the
							// cursor after completion of snippet insertion.
							root.Elements.Add(new SnippetCaretElement());
							break;
						}
						case "%SELECTION%":
						{
							// %SELECTION% is a special case token that describes where to place the
							// text that was selected before snippet insertion.  This is useful for
							// snippets that "surround" other text.
							int tabCount = line.Substring(0, group.Index).Count(c => c == '\t');
							root.Elements.Add(new SnippetSelectionElement { Indentation = tabCount });
							break;
						}
						default:
						{
							SnippetReplaceableTextElement symbol;
							if (symbols.TryGetValue(token, out symbol))
							{
								root.Elements.Add(new SnippetBoundElement { TargetElement = symbol });
							}
							else
							{
								symbol = new SnippetReplaceableTextElement { Text = token.Trim('%') };
								symbols[token] = symbol;

								root.Elements.Add(symbol);
							}
							break;
						}
					}

					lastIndex = group.Index + group.Length;
				}

				if (lastIndex != line.Length)
					root.Elements.Add(new SnippetTextElement { Text = line.Substring(lastIndex, line.Length - lastIndex) + Environment.NewLine });
				else
					root.Elements.Add(new SnippetTextElement { Text = Environment.NewLine });
			}
			else
			{
				root.Elements.Add(new SnippetTextElement { Text = line + Environment.NewLine });
			}
		}

		private const string NameToken = "name";
		private const string CategoryToken = "category";
		private const char Delimiter = ':';
		private static readonly Regex ReplaceableTokenPattern = new Regex(@"(^|[\s]*)(?<token>%[A-Za-z_][A-Za-z_0-9]*%)($|[\s]*)");
	}
}