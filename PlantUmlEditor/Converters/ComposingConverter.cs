using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;

namespace PlantUmlEditor.Converters
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

		public Collection<IValueConverter> Converters
		{
			get { return new Collection<IValueConverter>(_converters); }
		}

		private readonly IList<IValueConverter> _converters = new List<IValueConverter>();
    }
}
