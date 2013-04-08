using System;
using System.Globalization;
using System.IO;
using Utilities.Controls.Converters;
using Xunit;

namespace Tests.Unit.Utilities.Controls.Converters
{
	public class DirectoryInfoToUriConverterTests
	{
		[Fact]
		public void Test_Convert()
		{
			// Arrange.
			var directory = new DirectoryInfo(@"C:\Windows");

			// Act.
			var uri = (Uri)converter.Convert(directory, typeof(Uri), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.NotNull(uri);
			Assert.Equal(@"C:\Windows", uri.LocalPath);
		}

		[Fact]
		public void Test_Convert_Null()
		{
			// Act.
			var uri = (Uri)converter.Convert(null, typeof(Uri), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.Null(uri);
		}

		[Fact]
		public void Test_ConvertBack()
		{
			// Arrange.
			var uri = new Uri(@"C:\Windows");

			// Act.
			var file = (DirectoryInfo)converter.ConvertBack(uri, typeof(DirectoryInfo), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.NotNull(file);
			Assert.Equal(@"C:\Windows", file.FullName);
		}

		[Fact]
		public void Test_ConvertBack_Null()
		{
			// Act.
			var file = (DirectoryInfo)converter.ConvertBack(null, typeof(DirectoryInfo), null, CultureInfo.CurrentCulture);

			// Assert.
			Assert.Null(file);
		}

		private readonly DirectoryInfoToUriConverter converter = new DirectoryInfoToUriConverter();
	}
}