﻿//  PlantUML Editor 2
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

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Converter that negates boolean values.
	/// </summary>
	public class NegatingConverter : IValueConverter
	{
		#region IValueConverter Members

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
				return !(bool)value;

			throw new ArgumentException(@"Argument is invalid type", "value");
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
				return !(bool)value;

			throw new ArgumentException(@"Argument is invalid type", "value");
		}
		#endregion
	}
}