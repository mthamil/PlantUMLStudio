using System.IO;
using System.Text;
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
		public Snippet Parse(Stream snippetSource)
		{
			string name = null;
			string category = null;
			var code = new StringBuilder();
			foreach (string line in snippetSource.Lines())
			{
				if (name != null && category != null)
					code.AppendLine(line);

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

			var snippet = new ICSharpCode.AvalonEdit.Snippets.Snippet
			{
				Elements =
					{
						new SnippetTextElement { Text = code.ToString() }
					}
			};

			return new Snippet(name, category, snippet);
		}

		private const string NameToken = "name";
		private const string CategoryToken = "category";
		private const char Delimiter = ':';
	}
}