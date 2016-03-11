using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using Moq;
using PlantUmlEditor.View;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PlantUmlEditor.View
{
	public class HexColorCodeTextHighlighterTests
	{
		public HexColorCodeTextHighlighterTests()
		{
			runContext.SetupGet(rc => rc.Document)
					  .Returns(view.Document);

			runContext.SetupGet(rc => rc.TextView)
					  .Returns(view);
		}

		[Fact]
		//[InlineData()
		public void Test_Transform()//Color expected, string input)
		{
			// Arrange.
			string text = @"[#ABCD01]";
			view.Document.Text = text;

			var visualLine = view.GetOrConstructVisualLine(view.Document.Lines.Single());

			runContext.SetupGet(rc => rc.VisualLine)
					  .Returns(visualLine);

			runContext.Setup(rc => rc.GetText(It.IsAny<int>(), It.IsAny<int>()))
			          .Returns(new StringSegment(text));

			var visualElement = visualLine.Elements.Single();

			// Act.
			highlighter.Transform(runContext.Object, new List<VisualLineElement> { visualElement });

			// Assert.
			
		}

		private readonly HexColorCodeTextHighlighter highlighter = new HexColorCodeTextHighlighter();

		private readonly TextView view = new TextView { Document = new TextDocument() };
		private readonly Mock<ITextRunConstructionContext> runContext = new Mock<ITextRunConstructionContext>();
	}
}