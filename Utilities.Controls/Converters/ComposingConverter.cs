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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Chains several value converters together.
	/// </summary>
	public class ComposingConverter : IValueConverter
    {
		#region IValueConverter Members

		/// <see cref="IValueConverter.Convert"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			object converted = value;
            for (int i = 0; i < _converters.Count; i++)
            {
				converted = _converters[i].Convert(converted, targetType, parameter, culture);
            }
			return converted;
        }

		/// <see cref="IValueConverter.ConvertBack"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
			object converted = value;
            for (int i = _converters.Count - 1; i >= 0; i--)
            {
				converted = _converters[i].ConvertBack(converted, targetType, parameter, culture);
            }
			return converted;
        }

        #endregion

		/// <summary>
		/// The converters to compose.
		/// </summary>
		public Collection<IValueConverter> Converters
		{
			get { return _converters; }
		}

		private readonly Collection<IValueConverter> _converters = new Collection<IValueConverter>();
    }
}
