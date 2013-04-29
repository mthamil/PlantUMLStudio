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

using System;
using System.Globalization;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Converts between a string representing a quantity of seconds and a TimeSpan.
	/// </summary>
	[ValueConversion(typeof(TimeSpan?), typeof(string))]
	public class SecondsToTimeSpanConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return string.Empty;

			return ((TimeSpan)value).TotalSeconds.ToString(culture);
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int seconds;
			if (Int32.TryParse((string)value, out seconds))
				return TimeSpan.FromSeconds(seconds);

			return null;
		}

		#endregion
	}
}