using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Performs a logical OR on multiple boolean values.
	/// </summary>
	public class OrConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		/// <see cref="IMultiValueConverter.Convert"/>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			bool result = values.OfType<bool>().Aggregate(false, (first, second) => first || second);
			return result;
		}

		/// <see cref="IMultiValueConverter.ConvertBack"/>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}