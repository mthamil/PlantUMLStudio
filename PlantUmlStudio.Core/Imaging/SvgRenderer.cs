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

using System.IO;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace PlantUmlStudio.Core.Imaging
{
	/// <summary>
	/// Renders SVG diagrams.
	/// </summary>
	public class SvgRenderer : IDiagramRenderer
	{
		/// <see cref="IDiagramRenderer.Render(Diagram)"/>
		public ImageSource Render(Diagram diagram)
		{
			if (diagram.ImageFile == null || !diagram.ImageFile.Exists)
				return null;

			using (var converter = new FileSvgReader(_settings))
			{
				var drawingGroup = converter.Read(diagram.ImageFile.FullName);
				return CreateFrozenDrawing(drawingGroup);
			}
		}

		/// <see cref="IDiagramRenderer.Render(Stream)"/>
		public ImageSource Render(Stream imageData)
		{
			using (var reader = new StreamReader(imageData))
			using (var converter = new FileSvgReader(_settings))
			{
				var drawingGroup = converter.Read(reader);
				return CreateFrozenDrawing(drawingGroup);
			}
		}

		/// <summary>
		/// Freezing is necessary to use an image on a different thread from that on which
		/// it was created.
		/// </summary>
		private DrawingImage CreateFrozenDrawing(DrawingGroup drawingGroup)
		{
			var drawing = new DrawingImage(drawingGroup);
			drawing.Freeze();
			return drawing;
		}

		private readonly WpfDrawingSettings _settings = new WpfDrawingSettings();
	}
}