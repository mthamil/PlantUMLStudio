using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utilities.InputOutput;
using Xunit;

namespace Unit.Tests.Utilities.InputOutput
{
	public class AsyncStreamTests
	{
		[Fact]
		public async Task Test_ReadAsync()
		{
			// Arrange.
			var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			byte[] buffer = new byte[9];

			// Act.
			var result = await stream.Async().ReadAsync(buffer, 4, 3, CancellationToken.None);

			// Assert.
			Assert.Equal(3, result);
			AssertThat.SequenceEqual(new byte[] { 0, 0, 0, 0, 1, 2, 3, 0, 0 }, buffer);
		}

		[Fact]
		public async Task Test_ReadAllBytesAsync()
		{
			// Arrange.
			var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

			// Act.
			var result = await stream.Async().ReadAllBytesAsync(CancellationToken.None);

			// Assert.
			AssertThat.SequenceEqual(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, result);
		}

		[Fact]
		public async Task Test_WriteAsync()
		{
			// Arrange.
			var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var stream = new MemoryStream();

			// Act.
			await stream.Async().WriteAsync(data, 4, 3, CancellationToken.None);

			byte[] streamData = new byte[data.Length];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(streamData, 0, streamData.Length);

			// Assert.
			AssertThat.SequenceEqual(new byte[] { 5, 6, 7, 0, 0, 0, 0, 0, 0 }, streamData);
		}

		[Fact]
		public async Task Test_WriteAllBytesAsync()
		{
			// Arrange.
			var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var stream = new MemoryStream();

			// Act.
			await stream.Async().WriteAllBytesAsync(data, CancellationToken.None);

			byte[] streamData = new byte[data.Length];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(streamData, 0, streamData.Length);

			// Assert.
			AssertThat.SequenceEqual(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, streamData);
		}

		[Fact]
		public async Task Test_CopyToAsync()
		{
			// Arrange.
			var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var source = new MemoryStream(data);
			var destination = new MemoryStream();

			// Act.
			await source.Async().CopyToAsync(destination, CancellationToken.None);

			byte[] destinationData = new byte[data.Length];
			destination.Seek(0, SeekOrigin.Begin);
			destination.Read(destinationData, 0, destinationData.Length);

			// Assert.
			AssertThat.SequenceEqual(data, destinationData);
		}
	}
}
