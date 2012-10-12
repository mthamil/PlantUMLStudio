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
		public static async Task CopyToAsync(this FileInfo source, FileInfo destination, bool overwrite, CancellationToken cancellationToken)
		{
			var destinationOverwriteMode = overwrite ? FileMode.OpenOrCreate : FileMode.CreateNew;
			using (var sourceStream = source.OpenRead())
			using (var destinationStream = destination.Open(destinationOverwriteMode))
			{
				await sourceStream.Async().CopyToAsync(destinationStream, cancellationToken).ConfigureAwait(false);
			}
		}
	}
}