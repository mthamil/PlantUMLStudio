﻿//  PlantUML Studio
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
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Converts between DirectoryInfos and URIs.
	/// </summary>
	[ValueConversion(typeof(DirectoryInfo), typeof(Uri))]
	public class DirectoryInfoToUriConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var directoryInfo = value as DirectoryInfo;
			if (directoryInfo == null)
				return null;

			return new Uri(directoryInfo.FullName, UriKind.Absolute);
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var uri = value as Uri;
			if (uri == null)
				return null;

			return new DirectoryInfo(uri.LocalPath);
		}

		#endregion
	}
}