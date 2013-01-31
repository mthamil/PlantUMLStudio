using System;
using System.Collections.Generic;
using System.Globalization;
using Utilities.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.Utilities.Controls.Converters
{
	public class UriConverterTests
	{
		public static IEnumerable<object[]> ConvertData
		{
			get
			{
				return new TheoryDataSet<Uri, string>
				{
					{ new Uri("http://somesite"),	  "http://somesite" },
					{ new Uri("file:///C:/somefile"), "C:\\somefile" },
					{ null,							  "" },
					{ null,							  "http://somesite\\" }
				};
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
				return new TheoryDataSet<string, Uri>
				{
					{ "http://somesite/", new Uri("http://somesite") },
					{ "C:\\somefile",	  new Uri("file:///C:/somefile") },
					{ "",				  null }
				};
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