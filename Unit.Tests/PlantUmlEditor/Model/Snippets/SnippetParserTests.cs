using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Snippets;
using PlantUmlEditor.Model.Snippets;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.Model.Snippets
{
	public class SnippetParserTests
	{
		[Fact]
		public void Test_Parse()
		{
			// Arrange.
			string snippetText =
@"
name:test snippet
Category: snippets
(*) --> test
test --> (*)";

			var stream = new MemoryStream(Encoding.UTF8.GetBytes(snippetText));

			// Act.
			var snippet = parser.Parse(stream);

			// Assert.
			Assert.Equal("test snippet", snippet.Name);
			Assert.Equal("snippets", snippet.Category);
			Assert.Equal("(*) --> test\r\ntest --> (*)\r\n", ((SnippetTextElement)snippet.Code.Elements.Single()).Text);
		}

		private readonly SnippetParser parser = new SnippetParser();
	}
}