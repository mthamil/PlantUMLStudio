//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Renderer that highlights the line an editor's cursor is currently on.
	/// </summary>
	public class CurrentLineHighlighter : IBackgroundRenderer
	{
		/// <summary>
		/// Initializes a new renderer.
		/// </summary>
		/// <param name="textArea">The text area the renderer is associated with</param>
		public CurrentLineHighlighter(TextArea textArea)
		{
			_textArea = textArea;
		}

		/// <see cref="IBackgroundRenderer.Layer"/>
		public KnownLayer Layer
		{
			get { return KnownLayer.Background; }
		}

		/// <see cref="IBackgroundRenderer.Draw"/>
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (textView.Document == null)
				return;
			
			textView.EnsureVisualLines();
			var currentLine = textView.Document.GetLineByOffset(_textArea.Caret.Offset);
			foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
			{
				drawingContext.DrawRectangle(
					new SolidColorBrush(Color.FromArgb(255, 0xFF, 0xFF, 0xE0)), 
					new Pen(new SolidColorBrush(Color.FromArgb(255, 0xF2, 0xEA, 0xEA)), 1), 
					new Rect(rect.Location, new Size(textView.ActualWidth - 2, rect.Height)));
			}
		}

		private readonly TextArea _textArea;
	}
}