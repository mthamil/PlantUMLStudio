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
					if (tokens[0].Trim().ToLower() == NameToken)
					{
						name = tokens[1].Trim();
					}
					else if (tokens[0].Trim().ToLower() == CategoryToken)
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

					SnippetReplaceableTextElement symbol;
					if (symbols.TryGetValue(token, out symbol))
					{
						root.Elements.Add(new SnippetBoundElement { TargetElement = symbol });
					}
					else
					{
						symbol = new SnippetReplaceableTextElement { Text = token };
						symbols[token] = symbol;

						root.Elements.Add(symbol);
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

		private const string NameToken = "name";
		private const string CategoryToken = "category";
		private const char Delimiter = ':';
		private static readonly Regex ReplaceableTokenPattern = new Regex(@"(^|[\s]*)(?<token>%[A-Za-z_][A-Za-z_0-9]*%)($|[\s]*)");
	}
}