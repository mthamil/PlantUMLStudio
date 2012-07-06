using System;
using System.IO;
using System.Threading.Tasks;

namespace Utilities.InputOutput
{
	/// <summary>
	/// Interface for a class that provides asynchronous Stream operations.
	/// </summary>
	public interface IAsyncStreamOperations
	{
		/// <summary>
		/// Reads bytes from a stream asynchronously.
		/// </summary>
		/// <param name="buffer">The buffer to read bytes into</param>
		/// <param name="offset">The byte offset into the buffer at which to begin writing</param>
		/// <param name="count">The number of bytes to read from the stream</param>
		/// <returns>A task with the total number of bytes read</returns>
		Task<int> ReadAsync(byte[] buffer, int offset, int count);

		/// <summary>
		/// Reads all bytes in a stream asynchronously.
		/// </summary>
		/// <returns>A task that can be used to retrieve the result</returns>
		Task<byte[]> ReadAllBytesAsync();

		/// <summary>
		/// Writes bytes to a stream asynchronously.
		/// </summary>
		/// <param name="buffer">The buffer to write data from</param>
		/// <param name="offset">The byte offset into the buffer at which to begin copying</param>
		/// <param name="count">The number of bytes to write</param>
		/// <returns>A task representing the write operation</returns>
		Task WriteAsync(byte[] buffer, int offset, int count);

		/// <summary>
		/// Writes the given bytes to a stream asynchronously.
		/// </summary>
		/// <param name="data">The bytes to write</param>
		/// <returns>A task that can be used to wait for the operation to complete</returns>
		Task WriteAllBytesAsync(byte[] data);
	}

	/// <summary>
	/// Contains extension methods for Streams.
	/// </summary>
	public static class AsyncStream
	{
		/// <summary>
		/// Provides access to asynchronous Stream operations.
		/// </summary>
		/// <param name="stream">The stream to operate on</param>
		/// <returns>Asynchronous Stream operations</returns>
		public static IAsyncStreamOperations Async(this Stream stream)
		{
			return AsyncStreamFactory(stream);
		}

		internal static Func<Stream, IAsyncStreamOperations> AsyncStreamFactory = stream => new AsyncStreamOperations(stream);

		private class AsyncStreamOperations : IAsyncStreamOperations
		{
			public AsyncStreamOperations(Stream stream)
			{
				_stream = stream;
			}

			#region Implementation of IAsyncStreamOperations

			/// <see cref="IAsyncStreamOperations.ReadAsync"/>
			public Task<int> ReadAsync(byte[] buffer, int offset, int count)
			{
				return Task<int>.Factory.FromAsync(
					_stream.BeginRead,
					_stream.EndRead,
					buffer, offset, count,
					TaskCreationOptions.None);
			}

			/// <see cref="IAsyncStreamOperations.ReadAllBytesAsync"/>
			public Task<byte[]> ReadAllBytesAsync()
			{
				byte[] buffer = new byte[_stream.Length];
				var readTask = ReadAsync(buffer, 0, buffer.Length);

				// We don't really care about the number of bytes read, so return the buffer instead.
				return readTask.ContinueWith(t =>
				{
					var tcs = new TaskCompletionSource<byte[]>();
					if (t.IsFaulted && t.Exception != null)
						tcs.TrySetException(t.Exception);
					else if (t.IsCanceled)
						tcs.TrySetCanceled();
					else
						tcs.TrySetResult(buffer);

					return tcs.Task;
				}).Unwrap();
			}

			/// <see cref="IAsyncStreamOperations.WriteAsync"/>
			public Task WriteAsync(byte[] buffer, int offset, int count)
			{
				return Task.Factory.FromAsync(
					_stream.BeginWrite,
					_stream.EndWrite,
					buffer, offset, count,
					TaskCreationOptions.None);
			}

			/// <see cref="IAsyncStreamOperations.WriteAllBytesAsync"/>
			public Task WriteAllBytesAsync(byte[] data)
			{
				return WriteAsync(data, 0, data.Length);
			}

			#endregion

			private readonly Stream _stream;
		}
	}
}