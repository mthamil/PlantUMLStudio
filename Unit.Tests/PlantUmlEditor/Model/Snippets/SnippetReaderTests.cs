using System.IO;
using System.Text;
using PlantUmlEditor.Model.Snippets;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.Model.Snippets
{
	public class SnippetReaderTests
	{
		[Fact]
		public void Test_ReadSnippet()
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
			var snippet = reader.Read(stream);

			// Assert.
			Assert.Equal("test snippet", snippet.Name);
			Assert.Equal("snippets", snippet.Category);
			Assert.Equal("(*) --> test\r\ntest --> (*)\r\n", snippet.Code);
		}

		private readonly SnippetReader reader = new SnippetReader();
	}
}