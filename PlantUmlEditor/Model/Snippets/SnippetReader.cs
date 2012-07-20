using System.IO;
using System.Text;
using Utilities.InputOutput;

namespace PlantUmlEditor.Model.Snippets
{
	/// <summary>
	/// Reads code snippets.
	/// </summary>
	public class SnippetReader : ISnippetReader
	{
		/// <see cref="ISnippetReader.Read"/>
		public Snippet Read(Stream snippetSource)
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

			return new Snippet(name, category, code.ToString());
		}

		private const string NameToken = "name";
		private const string CategoryToken = "category";
		private const char Delimiter = ':';
	}
}