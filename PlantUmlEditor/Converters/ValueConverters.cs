using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace PlantUmlEditor.Converters
{
    public class UriToCachedImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!string.IsNullOrEmpty(value.ToString()))
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(value.ToString());
                // OMAR: Trick #6
                // Unless we use this option, the image file is locked and cannot be modified.
                // Looks like WPF holds read lock on the images. Very bad.
                bi.CacheOption = BitmapCacheOption.OnLoad;
                // Unless we use this option, an image cannot be refrehsed. It loads from 
                // cache. Looks like WPF caches every image it loads in memory. Very bad.
                bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                try
                {
                    bi.EndInit();
                    return bi;
                }
                catch
                {
                    return default(BitmapImage);
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Two way conversion is not supported.");
        }
    }

    public class ProgressIndicatorWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return 0;
            
            int percentComplete = (values[0] is int ? (int)values[0] : 0);
            double availableWidth = (values[1] is double ? (double)values[1] : 0);
            return availableWidth * (percentComplete / 100.0);
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(Properties.Resources.NotImplemented);
        }
    }

    public class BoolSwitchVisibilityConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return Visibility.Visible;
            bool trueCondition = (values[0] is bool ? (bool)values[0] : true);
            bool falseCondition = (values[1] is bool ? (bool)values[1] : false);
            
            if (trueCondition && !falseCondition)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(Properties.Resources.NotImplemented);
        }
        #endregion
    }

    public class BoolANDVisibilityConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == values || values.Length < 2) return Visibility.Visible;

            bool condition1 = (values[0] is bool ? (bool)values[0] : true);
            bool condition2 = (values[1] is bool ? (bool)values[1] : true);

            if (condition1 && condition2)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(Properties.Resources.NotImplemented);
        }
        #endregion
    }

    public class TrimStringConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return value.ToString().Trim();
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(Properties.Resources.NotImplemented);
        }
        #endregion
    }

    /// <summary>
    /// This converter is used to show DateTime in short date format
    /// </summary>
    public class DateFormattingConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return ((DateTime)value).ToShortDateString();

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string dateString = (string)value;

            // Append first month and day if just the year was entered
            if (dateString.Length == 4)
                dateString = "1/1/" + dateString;

            DateTime date;
            DateTime.TryParse(dateString, out date);
            if (date == DateTime.MinValue)
                return null;
            else
                return date;
        }
        #endregion
    }

    /// <summary>
    /// This converter is used to show possessive first name. Note: doesn't handle names that end in 's' correctly yet.
    /// </summary>
    public class FirstNamePossessiveFormConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                // Simply add "'s". Can be extended to check for correct grammar.
                return value.ToString() + "'s ";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(Properties.Resources.NotImplemented);
        }
        #endregion
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            if (value is bool && (bool)value == true)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(Properties.Resources.NotImplemented);
        }
        #endregion
    }

    public class NotConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                return !(bool)value;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(Properties.Resources.NotImplemented);
        }
        #endregion
    }

    public class ComposingConverter : IValueConverter
    {
        #region IValueConverter Members
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			object converted = value;
            for (int i = 0; i < _converters.Count; i++)
            {
				converted = _converters[i].Convert(converted, targetType, parameter, culture);
            }
			return converted;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
			object converted = value;
            for (int i = _converters.Count - 1; i >= 0; i--)
            {
				converted = _converters[i].ConvertBack(converted, targetType, parameter, culture);
            }
			return converted;
        }

        #endregion

		public Collection<IValueConverter> Converters
		{
			get { return new Collection<IValueConverter>(_converters); }
		}

		private readonly IList<IValueConverter> _converters = new List<IValueConverter>();
    }

    /// <summary>
    /// This converter is used to show DateTime in short date format
    /// </summary>
    public class DoubleToGridLengthConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                double realValue = (double)value;
                return new GridLength(realValue);
            }
            else
            {
                return GridLength.Auto;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                GridLength realValue = (GridLength)value;
                return realValue.Value;
            }
            else
            {
                return (double)0;
            }
        }
        #endregion
    }
}
