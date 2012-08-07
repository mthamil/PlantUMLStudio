using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	public class BoolSwitchVisibilityConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		/// <see cref="IMultiValueConverter.Convert"/>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length < 2) 
				return Visibility.Visible;

			bool trueCondition = (values[0] is bool ? (bool)values[0] : true);
			bool falseCondition = (values[1] is bool ? (bool)values[1] : false);
            
			if (trueCondition && !falseCondition)
				return Visibility.Visible;

			return Visibility.Collapsed;
		}

		/// <see cref="IMultiValueConverter.ConvertBack"/>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}