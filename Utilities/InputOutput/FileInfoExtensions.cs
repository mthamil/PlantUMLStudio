//  PlantUML Editor
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

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.InputOutput
{
	/// <summary>
	/// Contains FileInfo extension methods.
	/// </summary>
	public static class FileInfoExtensions
	{
		/// <summary>
		/// Copies an existing file to a new file asynchronously.
		/// </summary>
		/// <param name="source">The file to copy</param>
		/// <param name="destination">The file to copy to</param>
		/// <param name="overwrite">Whether to overwrite an existing destination file it it exists</param>
		/// <param name="cancellationToken">Allows cancellation of the copy operation</param>
		/// <returns>A Task representing the copy operation</returns>
		public static async Task CopyToAsync(this FileInfo source, FileInfo destination, bool overwrite, CancellationToken cancellationToken = default(CancellationToken))
		{
			var destinationOverwriteMode = overwrite ? FileMode.OpenOrCreate : FileMode.CreateNew;
			using (var sourceStream = source.OpenRead())
			using (var destinationStream = destination.Open(destinationOverwriteMode))
			{
				await sourceStream.Async().CopyToAsync(destinationStream, cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Copies an existing file to a new file asynchronously.
		/// </summary>
		/// <param name="source">The file to copy</param>
		/// <param name="destinationFileName">The file path and name to copy to</param>
		/// <param name="overwrite">Whether to overwrite an existing destination file it it exists</param>
		/// <param name="cancellationToken">Allows cancellation of the copy operation</param>
		/// <returns>A Task representing the copy operation</returns>
		public static Task CopyToAsync(this FileInfo source, string destinationFileName, bool overwrite, CancellationToken cancellationToken = default(CancellationToken))
		{
			return source.CopyToAsync(new FileInfo(destinationFileName), overwrite, cancellationToken);
		}
	}
}