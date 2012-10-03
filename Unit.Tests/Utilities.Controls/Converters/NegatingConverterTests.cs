using System.Globalization;
using Utilities.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.Utilities.Controls.Converters
{
	public class NegatingConverterTests
	{
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public void Test_Convert(bool expected, bool input)
		{
			// Act.
			bool actual = (bool)converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public void Test_ConvertBack(bool expected, bool input)
		{
			// Act.
			bool actual = (bool)converter.ConvertBack(input, typeof(bool), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly NegatingConverter converter = new NegatingConverter();
	}
}