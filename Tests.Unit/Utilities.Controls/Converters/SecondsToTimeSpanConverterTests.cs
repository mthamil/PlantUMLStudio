using System;
using System.Globalization;
using Utilities.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.Utilities.Controls.Converters
{
	public class SecondsToTimeSpanConverterTests
	{
		[Theory]
		[Culture("en-US")]
		[InlineData(4, "4")]
		[InlineData(15, "15")]
		public void Test_Convert(int seconds, string expected)
		{
			// Act.
			var actual = (string)converter.Convert(TimeSpan.FromSeconds(4), typeof(string), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.Equal("4", actual);
		}

		[Fact]
		[Culture("en-US")]
		public void Test_Convert_NullTimeSpan()
		{
			// Act.
			var actual = (string)converter.Convert(null, typeof(string), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.Equal("", actual);
		}

		[Fact]
		[Culture("en-US")]
		public void Test_ConvertBack()
		{
			// Act.
			var actual = (TimeSpan)converter.ConvertBack("15", typeof(TimeSpan), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.Equal(TimeSpan.FromSeconds(15), actual);
		}

		[Fact]
		[Culture("en-US")]
		public void Test_ConvertBack_FailedToParse()
		{
			// Act.
			var actual = converter.ConvertBack("", typeof(TimeSpan), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.Equal(null, actual);
		}

		private readonly SecondsToTimeSpanConverter converter = new SecondsToTimeSpanConverter();
	}
}