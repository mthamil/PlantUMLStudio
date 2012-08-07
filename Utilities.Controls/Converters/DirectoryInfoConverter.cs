using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Utilities.Controls.Converters
{
	/// <summary>
	/// Converts between strings and DirectoryInfos.
	/// </summary>
	public class DirectoryInfoConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var directory = value as DirectoryInfo;
			if (directory == null)
				return string.Empty;

			return directory.FullName;
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string path = value as string;
			if (String.IsNullOrEmpty(path))
				return null;

			return new DirectoryInfo(path);
		}

		#endregion
	}
}