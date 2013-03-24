using System.IO;
using Utilities.InputOutput;
using Xunit;

namespace Tests.Unit.Utilities.InputOutput
{
	public class TemporaryFileTests
	{
		[Fact]
		public void Test_File()
		{
			// Arrange.
			var temp = new TemporaryFile();

			// Act.
			var file = temp.File;

			// Assert.
			Assert.NotNull(file);
			Assert.Equal(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), file.DirectoryName.TrimEnd(Path.DirectorySeparatorChar));
		}

		[Fact]
		public void Test_Dispose()
		{
			// Arrange.
			var temp = new TemporaryFile();
			temp.File.Create().Close();

			Assert.True(temp.File.Exists);

			// Act.
			temp.Dispose();
			temp.File.Refresh();

			// Assert.
			Assert.False(temp.File.Exists);
		}
	}
}