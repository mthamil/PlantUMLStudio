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
		public SnippetProvider(ISnippetReader snippetReader)
		{
			_snippetReader = snippetReader;
		}

		/// <summary>
		/// The available snippets.
		/// </summary>
		public IEnumerable<Snippet> Snippets { get { return _snippets; } }

		/// <summary>
		/// Loads snippets.
		/// </summary>
		public void Load()
		{
			_snippets = SnippetLocation.GetFiles("*.snip", SearchOption.AllDirectories)
				.Select(snippetFile => snippetFile.OpenRead())
				.Select(_snippetReader.Read);
		}

		/// <summary>
		/// The directory to use for snippets.
		/// </summary>
		public DirectoryInfo SnippetLocation { get; set; }

		private IEnumerable<Snippet> _snippets;
		private readonly ISnippetReader _snippetReader;
	}
}