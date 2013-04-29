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

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// A multi-value converter that converters boolean conditions to Visibility values.
	/// </summary>
	public class BoolSwitchVisibilityConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		/// <see cref="IMultiValueConverter.Convert"/>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length < 2) 
				return Visibility.Visible;

			bool trueCondition = (values[0] is bool ? (bool)values[0] : true);
			bool falseCondition = (values[1] is bool ? (bool)values[1] : false);
            
			if (trueCondition && !falseCondition)
				return Visibility.Visible;

			return Visibility.Collapsed;
		}

		/// <see cref="IMultiValueConverter.ConvertBack"/>
		/// <exception cref="NotSupportedException">Always throws</exception>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}