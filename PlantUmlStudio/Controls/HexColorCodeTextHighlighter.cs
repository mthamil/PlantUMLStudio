using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PlantUmlStudio.Controls
{
	/// <summary>
	/// Makes textual occurences of a color, ie. #FF0000, appear as that color.
	/// </summary>
	public class HexColorCodeTextHighlighter : DocumentColorizingTransformer
	{
		public HexColorCodeTextHighlighter()
		{
			hexColorPattern = new Regex(@"(\s+|(\s*(\[|""|:)?))(?<color>#[0-9a-fA-F]{6})(\s+|((\]|""|>)?\s*))");
		}

		protected override void ColorizeLine(DocumentLine line)
		{
			var lineOffset = line.Offset;
			var lineText = CurrentContext.GetText(line.Offset, line.Length).Text;
			var matchGroups = hexColorPattern.Matches(lineText)
			                                 .Cast<Match>()
			                                 .Where(m => m.Success)
			                                 .Select(match => match.Groups["color"]);

			foreach (var hexCodeGroup in matchGroups)
			{
				var start = lineOffset + hexCodeGroup.Index;
				var end = lineOffset + hexCodeGroup.Index + hexCodeGroup.Length;
				ChangeLinePart(start, end, lineElement =>
				{
					var color = ParseColorFromHexString(hexCodeGroup.Value);
					lineElement.TextRunProperties.SetForegroundBrush(new SolidColorBrush(color));

					var existingTypeface = lineElement.TextRunProperties.Typeface;
					lineElement.TextRunProperties.SetTypeface(new Typeface(
						existingTypeface.FontFamily,
						existingTypeface.Style,
						FontWeights.SemiBold,
						existingTypeface.Stretch));
				});
			}
		}

		private static Color ParseColorFromHexString(string hexCodeString)
		{
			var hexCode = hexCodeString.TrimStart('#');

			var rgb = new byte[3];
			for (int i = 0; i < hexCode.Length; i += 2)
				rgb[i/2] = Byte.Parse(hexCode.Substring(i, 2), NumberStyles.HexNumber);

			return Color.FromRgb(rgb[0], rgb[1], rgb[2]);
		}

		private readonly Regex hexColorPattern;
	}
}