using System.IO;
using System.Threading.Tasks;
using Utilities.InputOutput;
using Xunit;

namespace Tests.Unit.Utilities.InputOutput
{
	public class FileInfoExtensionsTests
	{
		[Fact]
		public async Task Test_CopyToAsync()
		{
			using (var temp1 = new TemporaryFile())
			using (var temp2 = new TemporaryFile())
			{
				// Arrange.
				File.WriteAllText(temp1.File.FullName, "CONTENTS");

				// Act.
				await temp1.File.CopyToAsync(temp2.File, true);

				// Assert.
				Assert.Equal("CONTENTS", File.ReadAllText(temp2.File.FullName));
			}
		} 
	}
}