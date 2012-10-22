//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 

using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PlantUmlEditor.Core;
using Utilities.Controls.Converters;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Renders diagrams to bitmaps.
	/// </summary>
	public class DiagramBitmapRenderer : IDiagramRenderer
	{
		/// <see cref="IDiagramRenderer.Render"/>
		public ImageSource Render(Diagram diagram)
		{
			Uri imageUri;
			if (!String.IsNullOrEmpty(diagram.ImageFilePath) && Uri.TryCreate(diagram.ImageFilePath, UriKind.RelativeOrAbsolute, out imageUri))
				return (ImageSource)_uriToImageConverter.Convert(imageUri, null, null, null);

			return null;
		}

		private readonly IValueConverter _uriToImageConverter = new UriToCachedImageConverter
		{
			// OMAR: Trick #6
			// Unless we use this option, the image file is locked and cannot be modified.
			// Looks like WPF holds read lock on the images. Very bad.
			CacheOption = BitmapCacheOption.OnLoad,

			// Unless we use this option, an image cannot be refreshed. It loads from 
			// cache. Looks like WPF caches every image it loads in memory. Very bad.
			InitOptions = BitmapCreateOptions.IgnoreImageCache
		};
	}
}
