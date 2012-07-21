using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlantUmlEditor.Model.Snippets
{
	/// <summary>
	/// Responsible for loading and managing diagram code snippets.
	/// </summary>
	public class SnippetProvider
	{
		public SnippetProvider(ISnippetParser snippetParser)
		{
			_snippetParser = snippetParser;
		}

		/// <summary>
		/// The available snippets.
		/// </summary>
		public IEnumerable<CodeSnippet> Snippets { get { return _snippets; } }

		/// <summary>
		/// Loads snippets.
		/// </summary>
		public void Load()
		{
			_snippets = SnippetLocation.GetFiles("*.snip", SearchOption.AllDirectories)
				.Select(snippetFile => snippetFile.OpenRead())
				.Select(_snippetParser.Parse);
		}

		/// <summary>
		/// The directory to use for snippets.
		/// </summary>
		public DirectoryInfo SnippetLocation { get; set; }

		private IEnumerable<CodeSnippet> _snippets;
		private readonly ISnippetParser _snippetParser;
	}
}