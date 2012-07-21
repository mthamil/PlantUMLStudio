using ICSharpCode.AvalonEdit.Snippets;

namespace PlantUmlEditor.Model.Snippets
{
	/// <summary>
	/// Represents a diagram code snippet.
	/// </summary>
	public class CodeSnippet
	{
		/// <summary>
		/// Creates a new snippet.
		/// </summary>
		/// <param name="name">The name of the snippet</param>
		/// <param name="category">The snippet category</param>
		/// <param name="code">The snippet code structure</param>
		public CodeSnippet(string name, string category, Snippet code)
		{
			Name = name;
			Category = category;
			Code = code;
		}

		/// <summary>
		/// The name of the category.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The snippet category.
		/// </summary>
		public string Category { get; private set; }

		/// <summary>
		/// The snippet code.
		/// </summary>
		public Snippet Code { get; private set; }
	}
}