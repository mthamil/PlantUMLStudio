using System.IO;
using System.Text;
using Utilities.InputOutput;
using Xunit;

namespace Unit.Tests.Utilities.InputOutput
{
	public class StreamExtensionsTests
	{
		[Fact]
		public void Test_Lines()
		{
			const string input = 
@"Line 1
Line 2
Line 3
Line 4";
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

			// Act.
			var lines = stream.Lines();

			// Assert.
			AssertThat.SequenceEqual(new [] { "Line 1", "Line 2", "Line 3", "Line 4" }, lines);
		}
	}
}
