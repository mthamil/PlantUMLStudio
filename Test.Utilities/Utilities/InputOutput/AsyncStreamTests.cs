using System.IO;
using Utilities.InputOutput;
using Xunit;

namespace Unit.Tests.Utilities.InputOutput
{
	public class AsyncStreamTests
	{
		[Fact]
		public void Test_ReadAsync()
		{
			// Arrange.
			var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			byte[] buffer = new byte[9];

			// Act.
			var readTask = stream.Async().ReadAsync(buffer, 4, 3);
			readTask.Wait();

			// Assert.
			Assert.Equal(3, readTask.Result);
			AssertThat.SequenceEqual(new byte[] { 0, 0, 0, 0, 1, 2, 3, 0, 0 }, buffer);
		}

		[Fact]
		public void Test_ReadAllBytesAsync()
		{
			// Arrange.
			var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

			// Act.
			var readTask = stream.Async().ReadAllBytesAsync();
			readTask.Wait();

			// Assert.
			AssertThat.SequenceEqual(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, readTask.Result);
		}

		[Fact]
		public void Test_WriteAsync()
		{
			// Arrange.
			var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var stream = new MemoryStream();

			// Act.
			var writeTask = stream.Async().WriteAsync(data, 4, 3);
			writeTask.Wait();

			byte[] streamData = new byte[data.Length];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(streamData, 0, streamData.Length);

			// Assert.
			AssertThat.SequenceEqual(new byte[] { 5, 6, 7, 0, 0, 0, 0, 0, 0 }, streamData);
		}

		[Fact]
		public void Test_WriteAllBytesAsync()
		{
			// Arrange.
			var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var stream = new MemoryStream();

			// Act.
			var writeTask = stream.Async().WriteAllBytesAsync(data);
			writeTask.Wait();

			byte[] streamData = new byte[data.Length];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(streamData, 0, streamData.Length);

			// Assert.
			AssertThat.SequenceEqual(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, streamData);
		}
	}
}
