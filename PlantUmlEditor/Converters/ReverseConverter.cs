using System;
using System.Globalization;
using System.Windows.Data;

namespace PlantUmlEditor.Converters
{
	public class ReverseConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return InnerConverter.ConvertBack(value, targetType, parameter, culture);
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return InnerConverter.Convert(value, targetType, parameter, culture);
		}

		#endregion

		public IValueConverter InnerConverter { get; set; }
	}
}