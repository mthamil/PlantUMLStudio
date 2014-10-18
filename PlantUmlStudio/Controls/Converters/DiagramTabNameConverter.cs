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
using System.Windows;
using System.Windows.Data;

namespace PlantUmlStudio.Controls.Converters
{
	/// <summary>
	/// Constructs a diagram's tab name.
	/// </summary>
	public class DiagramTabNameConverter : IMultiValueConverter
	{
		#region Implementation of IMultiValueConverter

		/// <see cref="IMultiValueConverter.Convert"/>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length != 2 || !(values[0] is string) || !(values[1] is bool))
			{
				return DependencyProperty.UnsetValue;
			}

			var fileName = (string)values[0];
			bool isModified = (bool)values[1];
			return isModified 
				? String.Format(culture, ModifiedFormat, fileName) 
				: fileName;
		}

		/// <see cref="IMultiValueConverter.ConvertBack"/>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary>
		/// The string format to use when a tab represents modified data.
		/// </summary>
		public string ModifiedFormat { get; set; }
	}
}