using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PlantUmlEditor.Converters
{
	/// <summary>
	/// Constructs a diagram's tab name.
	/// </summary>
	public class DiagramTabNameConverter : IMultiValueConverter
	{
		#region Implementation of IMultiValueConverter

		/// <see cref="IMultiValueConverter.Convert"/>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length != 2 || !(values[0] is string) || !(values[1] is bool))
			{
				return DependencyProperty.UnsetValue;
			}

			var fileName = (string)values[0];
			bool isModified = (bool)values[1];
			return isModified 
				? String.Format(ModifiedFormat, fileName) 
				: fileName;
		}

		/// <see cref="IMultiValueConverter.ConvertBack"/>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary>
		/// The string format to use when a tab represents modified data.
		/// </summary>
		public string ModifiedFormat { get; set; }
	}
}