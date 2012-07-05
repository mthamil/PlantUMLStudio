using System;
using System.Globalization;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Converts between a string representing a quantity of seconds and a TimeSpan.
	/// </summary>
	[ValueConversion(typeof(TimeSpan), typeof(string))]
	public class SecondsToTimeSpanConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((TimeSpan)value).TotalSeconds.ToString(culture);
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return TimeSpan.FromSeconds(Int32.Parse((string)value));
		}

		#endregion
	}
}