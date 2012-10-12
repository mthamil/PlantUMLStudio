using System;
using System.Globalization;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Converts between a string representing a quantity of seconds and a TimeSpan.
	/// </summary>
	[ValueConversion(typeof(TimeSpan?), typeof(string))]
	public class SecondsToTimeSpanConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return string.Empty;

			return ((TimeSpan)value).TotalSeconds.ToString(culture);
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int seconds;
			if (Int32.TryParse((string)value, out seconds))
				return TimeSpan.FromSeconds(seconds);

			return null;
		}

		#endregion
	}
}