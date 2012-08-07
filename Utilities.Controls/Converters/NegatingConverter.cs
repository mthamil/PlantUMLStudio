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