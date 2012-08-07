using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	///<see cref="IValueConverter"/>
	/// <remarks>
	/// Converts between booleans and Visibility enum
	/// </remarks>
	public class BooleanToVisibilityValueConverter : IValueConverter
	{
		///<see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				if (((bool)value))
					return Visibility.Visible;
				return Visibility.Collapsed;
			}

			return Visibility.Collapsed;
		}

		///<see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null && ((Visibility)value) == Visibility.Visible;
		}
	}
}
