//  PlantUML Editor
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

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlantUmlEditor.Core.Imaging
{
	/// <summary>
	/// Renders diagram images to bitmaps.
	/// </summary>
	public class BitmapRenderer : IDiagramRenderer
	{
		/// <see cref="IDiagramRenderer.Render(Diagram)"/>
		public ImageSource Render(Diagram diagram)
		{
			Uri imageUri;
			if (!String.IsNullOrEmpty(diagram.ImageFilePath) && Uri.TryCreate(diagram.ImageFilePath, UriKind.RelativeOrAbsolute, out imageUri))
			{
				var bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.UriSource = imageUri;

				// OMAR: Trick #6
				// Unless we use this option, the image file is locked and cannot be modified.
				// Looks like WPF holds read lock on the images. Very bad.
				bitmap.CacheOption = BitmapCacheOption.OnLoad;

				// Unless we use this option, an image cannot be refreshed. It loads from 
				// cache. Looks like WPF caches every image it loads in memory. Very bad.
				bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

				try
				{
					bitmap.EndInit();
					return bitmap;
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}
			}

			return null;
		}

		/// <see cref="IDiagramRenderer.Render(Stream)"/>
		public ImageSource Render(Stream imageData)
		{
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.StreamSource = imageData;
			bitmap.EndInit();
			bitmap.Freeze();
			return bitmap;
		}
	}
}
