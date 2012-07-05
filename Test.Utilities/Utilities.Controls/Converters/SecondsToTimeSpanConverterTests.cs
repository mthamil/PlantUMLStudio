using System;
using System.Globalization;
using Utilities.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.Utilities.Controls.Converters
{
	public class SecondsToTimeSpanConverterTests
	{
		[Theory]
		[InlineData(4, "4")]
		[InlineData(15, "15")]
		public void Test_Convert(int seconds, string expected)
		{
			// Act.
			var actual = (string)converter.Convert(TimeSpan.FromSeconds(4), typeof(string), null, CultureInfo.GetCultureInfo("en-US"));

			// Assert.
			Assert.Equal("4", actual);
		}

		[Fact]
		public void Test_ConvertBack()
		{
			// Act.
			var actual = (TimeSpan)converter.ConvertBack("15", typeof(string), null, CultureInfo.GetCultureInfo("en-US"));

			// Assert.
			Assert.Equal(TimeSpan.FromSeconds(15), actual);
		}

		private readonly SecondsToTimeSpanConverter converter = new SecondsToTimeSpanConverter();
	}
}