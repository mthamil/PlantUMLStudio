﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using Moq;
using PlantUmlStudio.Controls;
using SharpEssentials.Testing;
using SharpEssentials.Testing.Controls.WPF;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.View
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
				return new TheoryData<string, Color, IList<Tuple<int, int>>>
				{
					{ @"[#ABCD01]", Color.FromRgb(0xAB, 0xCD, 0x01), new[] { Tuple.Create(0, 1), Tuple.Create(1, 7), Tuple.Create(8, 1) } },
					{ @"<color:#FF0012> ", Color.FromRgb(0xFF, 0x00, 0x12), new[] { Tuple.Create(0, 7), Tuple.Create(7, 7), Tuple.Create(14, 2) } }
				};
			}
		}

		[WpfTheory]
		[MemberData(nameof(TransformData))]
		public void Test_Transform(string inputText, Color expectedColor, IList<Tuple<int, int>> expectedTextRuns)
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

			Assert.Equal(expectedTextRuns.Count, visualElements.Count);
			AssertThat.SequenceEqual(new[] { Colors.Black, expectedColor, Colors.Black }, elementColors);
			AssertThat.SequenceEqual(
				expectedTextRuns, 
				visualElements.Select(e => Tuple.Create(e.RelativeTextOffset, e.VisualLength)));
		}

		private readonly HexColorCodeTextHighlighter highlighter = new HexColorCodeTextHighlighter();

		private readonly TextView view = new TextView { Document = new TextDocument() };
		private readonly Mock<ITextRunConstructionContext> runContext = new Mock<ITextRunConstructionContext>();
	}
}
