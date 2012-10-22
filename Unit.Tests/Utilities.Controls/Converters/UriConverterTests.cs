using System;
using System.Collections.Generic;
using System.Globalization;
using Utilities.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.Utilities.Controls.Converters
{
	public class UriConverterTests
	{
		public static IEnumerable<object[]> ConvertData
		{
			get
			{
				yield return new object[] { new Uri("http://somesite"),		"http://somesite" };
				yield return new object[] { new Uri("file:///C:/somefile"), "C:\\somefile" };
				yield return new object[] { null,							"" };
				yield return new object[] { null,							"http://somesite\\" };
			}
		}

		[Theory]
		[PropertyData("ConvertData")]
		public void Test_Convert(Uri expected, string input)
		{
			// Act.
			var actual = converter.Convert(input, typeof(Uri), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		public static IEnumerable<object[]> ConvertBackData
		{
			get
			{
				yield return new object[] { "http://somesite/", new Uri("http://somesite") };
				yield return new object[] { "C:\\somefile",		new Uri("file:///C:/somefile") };
				yield return new object[] { "",					null };
			}
		}

		[Theory]
		[PropertyData("ConvertBackData")]
		public void Test_ConvertBack(string expected, Uri input)
		{
			// Act.
			var actual = converter.ConvertBack(input, typeof(string), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly UriConverter converter = new UriConverter();
	}
}