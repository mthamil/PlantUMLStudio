using System.Collections.Generic;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Represents a logical grouping of snippets.
	/// </summary>
	public class SnippetCategory
	{
		/// <summary>
		/// Creates a new snippet category.
		/// </summary>
		/// <param name="name">The name of the category</param>
		/// <param name="snippets">The snippets/sub-categories in the category</param>
		public SnippetCategory(string name, IEnumerable<SnippetCategory> snippets)
		{
			Name = name;
			Snippets = snippets;
		}

		/// <summary>
		/// The name of the category.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The snippets/sub-categories in the category.
		/// </summary>
		public IEnumerable<SnippetCategory> Snippets { get; private set; }
	}
}