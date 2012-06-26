using System;
using System.Globalization;
using System.Windows.Data;

namespace PlantUmlEditor.Converters
{
	/// <summary>
	///  A value converter where null is false and not-null is true.
	/// </summary>
	public class NullToFalseValueConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		///<see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}

		///<see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}