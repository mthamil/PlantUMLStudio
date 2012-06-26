using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace PlantUmlEditor.Converters
{
	/// <summary>
	/// Converts between DirectoryInfos and URIs.
	/// </summary>
	public class DirectoryInfoUriConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var directoryInfo = value as DirectoryInfo;
			if (directoryInfo == null)
				return null;

			return new Uri(directoryInfo.FullName, UriKind.Absolute);
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var uri = value as Uri;
			if (uri == null)
				return null;

			return new DirectoryInfo(uri.LocalPath);
		}

		#endregion
	}
}