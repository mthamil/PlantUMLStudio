using System.Globalization;
using System.Windows;
using Utilities.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.Utilities.Controls.Converters
{
	public class BooleanToVisibilityValueConverterTests
	{
		[Theory]
		[InlineData(Visibility.Visible, true)]
		[InlineData(Visibility.Collapsed, false)]
		[InlineData(Visibility.Collapsed, null)]
		public void Test_Convert(Visibility expected, bool? input)
		{
			// Act.
			Visibility actual = (Visibility)converter.Convert(input, typeof(Visibility), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(true, Visibility.Visible)]
		[InlineData(false, Visibility.Collapsed)]
		[InlineData(false, Visibility.Hidden)]
		[InlineData(false, null)]
		public void Test_ConvertBack(bool expected, Visibility? input)
		{
			// Act.
			bool actual = (bool)converter.ConvertBack(input, typeof(bool), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly BooleanToVisibilityValueConverter converter = new BooleanToVisibilityValueConverter();
	}
}