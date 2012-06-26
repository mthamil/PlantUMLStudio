using System;
using System.Globalization;
using System.Windows.Data;

namespace PlantUmlEditor.Converters
{
	/// <summary>
	/// Converts between strings and URIs.
	/// </summary>
	public class UriConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Uri uri = value as Uri;
			if (uri == null)
				return string.Empty;

			if (uri.IsFile)
				return uri.LocalPath;

			return uri.ToString();
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string uri = value as string;
			if (String.IsNullOrEmpty(uri))
				return null;

			return new Uri(uri, UriKind.RelativeOrAbsolute);
		}

		#endregion
	}
}