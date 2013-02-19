using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using Moq;
using PlantUmlEditor.View;
using Xunit;
using Xunit.Extensions;
using T = System.Tuple;

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

		public static IEnumerable<object[]> TransformData
		{
			get
			{
				return new TheoryDataSet<string, Color, IEnumerable<Tuple<int, int>>>
				{
					{ @"[#ABCD01]", Color.FromRgb(0xAB, 0xCD, 0x01), new [] { T.Create(0, 1), T.Create(1, 7), T.Create(8, 1) } },
					{ @"<color:#FF0012> ", Color.FromRgb(0xFF, 0x00, 0x12), new [] { T.Create(0, 7), T.Create(7, 7), T.Create(14, 2) } }
				};
			}
		}

		[Theory]
		[PropertyData("TransformData")]
		public void Test_Transform(string inputText, Color expectedColor, IEnumerable<Tuple<int, int>> expectedTextRuns)
		{
			// Arrange.
			view.Document.Text = inputText;

			var visualLine = view.GetOrConstructVisualLine(view.Document.Lines.Single());

			runContext.SetupGet(rc => rc.VisualLine)
					  .Returns(visualLine);

			runContext.Setup(rc => rc.GetText(It.IsAny<int>(), It.IsAny<int>()))
					  .Returns(new StringSegment(inputText));

			var visualElements = new List<VisualLineElement> { visualLine.Elements.Single() };

			// Act.
			highlighter.Transform(runContext.Object, visualElements);

			// Assert.
			var elementColors = visualElements.Select(ve => ve.TextRunProperties.ForegroundBrush)
			                                  .Cast<SolidColorBrush>()
			                                  .Select(b => b.Color)
			                                  .ToList();

			Assert.Equal(3, visualElements.Count);
			AssertThat.SequenceEqual(new[] { Colors.Black, expectedColor, Colors.Black }, elementColors);
			AssertThat.SequenceEqual(
				expectedTextRuns, 
				visualElements.Select(e => T.Create(e.RelativeTextOffset, e.VisualLength)));
		}

		private readonly HexColorCodeTextHighlighter highlighter = new HexColorCodeTextHighlighter();

		private readonly TextView view = new TextView { Document = new TextDocument() };
		private readonly Mock<ITextRunConstructionContext> runContext = new Mock<ITextRunConstructionContext>();
	}
}