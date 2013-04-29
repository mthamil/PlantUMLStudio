//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.IO;
using System.Threading;
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
		/// <param name="cancellationToken">Allows cancellation of the read operation</param>
		/// <returns>A task with the total number of bytes read</returns>
		Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

		/// <summary>
		/// Reads all bytes in a stream asynchronously.
		/// </summary>
		/// <param name="cancellationToken">Allows cancellation of the read operation</param>
		/// <returns>A task that can be used to retrieve the result</returns>
		Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Writes bytes to a stream asynchronously.
		/// </summary>
		/// <param name="buffer">The buffer to write data from</param>
		/// <param name="offset">The byte offset into the buffer at which to begin copying</param>
		/// <param name="count">The number of bytes to write</param>
		/// <param name="cancellationToken">Allows cancellation of the write operation</param>
		/// <returns>A task representing the write operation</returns>
		Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

		/// <summary>
		/// Writes the given bytes to a stream asynchronously.
		/// </summary>
		/// <param name="data">The bytes to write</param>
		/// <param name="cancellationToken">Allows cancellation of the write operation</param>
		/// <returns>A task that can be used to wait for the operation to complete</returns>
		Task WriteAllBytesAsync(byte[] data, CancellationToken cancellationToken);

		/// <summary>
		/// Asynchronously reads all bytes from the current stream and writes them to a 
		/// destination stream.
		/// </summary>
		/// <param name="destination">The stream being copied to</param>
		/// <param name="cancellationToken">Allows cancellation of the copy operation</param>
		/// <returns>A Task representing the copy operation</returns>
		Task CopyToAsync(Stream destination, CancellationToken cancellationToken);
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
			public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				return _stream.ReadAsync(buffer, offset, count, cancellationToken);
			}

			/// <see cref="IAsyncStreamOperations.ReadAllBytesAsync"/>
			public async Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken)
			{
				// We don't really care about the number of bytes read, so return the buffer instead.
				var tcs = new TaskCompletionSource<byte[]>();
				try
				{
					byte[] buffer = new byte[_stream.Length];
					await ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
					tcs.TrySetResult(buffer);
				}
				catch (TaskCanceledException)
				{
					tcs.TrySetCanceled();
				}
				catch (Exception e)
				{
					tcs.TrySetException(e);
				}

				return await tcs.Task.ConfigureAwait(false);
			}

			/// <see cref="IAsyncStreamOperations.WriteAsync"/>
			public Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				return _stream.WriteAsync(buffer, offset, count, cancellationToken);
			}

			/// <see cref="IAsyncStreamOperations.WriteAllBytesAsync"/>
			public Task WriteAllBytesAsync(byte[] data, CancellationToken cancellationToken)
			{
				return WriteAsync(data, 0, data.Length, cancellationToken);
			}

			/// <see cref="IAsyncStreamOperations.CopyToAsync"/>
			public Task CopyToAsync(Stream destination, CancellationToken cancellationToken)
			{
				return _stream.CopyToAsync(destination, DefaultCopyBufferSize, cancellationToken);
			}

			#endregion

			private readonly Stream _stream;

			private const int DefaultCopyBufferSize = 81920; 
		}
	}
}