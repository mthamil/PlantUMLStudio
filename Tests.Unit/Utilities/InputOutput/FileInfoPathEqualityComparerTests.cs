using System.Collections.Generic;
using System.IO;
using Utilities.InputOutput;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.Utilities.InputOutput
{
	public class FileInfoPathEqualityComparerTests
	{
		public static IEnumerable<object[]> EqualsData
		{
			get
			{
				var sameFile = new FileInfo(@"c:\test.txt");
				return new TheoryDataSet<bool, FileInfo, FileInfo>
				{
					{ true,  sameFile, sameFile },
					{ true,  new FileInfo(@"c:\test.txt"), new FileInfo(@"c:\test.txt") },
					{ true,  new FileInfo(@"c:\test.txt"), new FileInfo(@"C:\test.TXT") },
					{ false, new FileInfo(@"c:\test.txt"), new FileInfo(@"test.txt") },
					{ false, new FileInfo(@"c:\test.txt"), new FileInfo(@"c:\testDir\test.txt") },
					{ false, new FileInfo(@"c:\test.txt"), null }
				};
			}
		}

		[Theory]
		[PropertyData("EqualsData")]
		public void Test_Equals(bool expected, FileInfo first, FileInfo second)
		{
			// Act.
			bool actual = comparer.Equals(first, second);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(@"c:\test.txt")]
		[InlineData(@"C:\test.TXT")]
		public void Test_GetHashCode(string path)
		{
			// Arrange.
			var file = new FileInfo(path);
			var expected = comparer.GetHashCode(new FileInfo(@"c:\test.txt"));

			// Act.
			int actual = comparer.GetHashCode(file);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly FileSystemInfoPathEqualityComparer comparer = new FileSystemInfoPathEqualityComparer();
	}
}