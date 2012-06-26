using System.Linq;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Represents a diagram code snippet.
	/// </summary>
	public class Snippet : SnippetCategory
	{
		/// <summary>
		/// Creates a new snippet.
		/// </summary>
		/// <param name="name">The name of the snippet</param>
		/// <param name="code">The snippet code</param>
		public Snippet(string name, string code)
			: base(name, Enumerable.Empty<SnippetCategory>())
		{
			Code = code;
		}

		/// <summary>
		/// The snippet code.
		/// </summary>
		public string Code { get; private set; }
	}
}