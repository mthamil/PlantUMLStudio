using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// A multi-value converter that uses test input to determine Visibility.
	/// </summary>
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
		/// <see cref="IMultiValueConverter.Convert"/>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];

                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

		/// <see cref="IMultiValueConverter.ConvertBack"/>
		/// <exception cref="NotImplementedException">Always throws</exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
