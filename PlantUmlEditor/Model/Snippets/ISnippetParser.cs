using System.IO;

namespace PlantUmlEditor.Model.Snippets
{
	/// <summary>
	/// Parses code snippets.
	/// </summary>
	public interface ISnippetParser
	{
		/// <summary>
		/// Reads the content of a stream and creates a Snippet from it.
		/// </summary>
		/// <param name="snippetSource">The source stream</param>
		/// <returns>A snippet.</returns>
		Snippet Parse(Stream snippetSource);
	}
}