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
			Assert.Equal(2, snippet.Code.Elements.Count);
			Assert.Equal("(*) --> test\r\n", ((SnippetTextElement)snippet.Code.Elements[0]).Text);
			Assert.Equal("test --> (*)\r\n", ((SnippetTextElement)snippet.Code.Elements[1]).Text);
		}

		[Fact]
		public void Test_Parse_ReplaceableText()
		{
			// Arrange.
			string snippetText =
@"
name:test snippet
Category: snippets
(*) [%COMMENT%] --> %ACTIVITY%
%ACTIVITY% --> (*)";

			var stream = new MemoryStream(Encoding.UTF8.GetBytes(snippetText));

			// Act.
			var snippet = parser.Parse(stream);

			// Assert.
			Assert.Equal("test snippet", snippet.Name);
			Assert.Equal("snippets", snippet.Category);
			Assert.Equal(7, snippet.Code.Elements.Count);
			Assert.Equal("(*) [", ((SnippetTextElement)snippet.Code.Elements[0]).Text);
			Assert.Equal("%COMMENT%", ((SnippetReplaceableTextElement)snippet.Code.Elements[1]).Text);
			Assert.Equal("] --> ", ((SnippetTextElement)snippet.Code.Elements[2]).Text);
			Assert.Equal("%ACTIVITY%", ((SnippetReplaceableTextElement)snippet.Code.Elements[3]).Text);
			Assert.Equal("\r\n", ((SnippetTextElement)snippet.Code.Elements[4]).Text);
			Assert.Equal(snippet.Code.Elements[3], ((SnippetBoundElement)snippet.Code.Elements[5]).TargetElement);
			Assert.Equal(" --> (*)\r\n", ((SnippetTextElement)snippet.Code.Elements[6]).Text);
		}

		private readonly SnippetParser parser = new SnippetParser();
	}
}