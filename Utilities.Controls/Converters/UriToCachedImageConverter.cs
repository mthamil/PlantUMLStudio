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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Converts a URI source to a bitmap image.
	/// </summary>
	[ValueConversion(typeof(Uri), typeof(ImageSource))]
	public class UriToCachedImageConverter : IValueConverter
	{
		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			string imagePath = value.ToString();

			Uri imageUri;
			if (!String.IsNullOrEmpty(imagePath) && Uri.TryCreate(imagePath, UriKind.RelativeOrAbsolute, out imageUri))
			{
				BitmapImage bi = new BitmapImage();
				bi.BeginInit();
				bi.UriSource = imageUri;
				// OMAR: Trick #6
				// Unless we use this option, the image file is locked and cannot be modified.
				// Looks like WPF holds read lock on the images. Very bad.
				bi.CacheOption = BitmapCacheOption.OnLoad;
				// Unless we use this option, an image cannot be refreshed. It loads from 
				// cache. Looks like WPF caches every image it loads in memory. Very bad.
				bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
				try
				{
					bi.EndInit();
					return bi;
				}
				catch
				{
					return default(BitmapImage);
				}
			}

			return null;
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		/// <exception cref="NotImplementedException">Always throws</exception>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("Two way conversion is not supported.");
		}
	}
}