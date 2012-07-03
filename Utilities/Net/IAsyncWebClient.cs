using System;
using System.Net;
using System.Threading.Tasks;
using Utilities.Concurrency;

namespace Utilities.Net
{
	/// <summary>
	/// Interface for WebClient operations that use asynchronous tasks.
	/// </summary>
	public interface IAsyncWebClient
	{
		/// <summary>
		/// Downloads, to a local file, the resource with the specified URI. This method does not block the calling thread.
		/// </summary>
		/// <param name="address">The URI of the resource to download. </param>
		/// <param name="fileName">The name of the file to be placed on the local computer. </param>
		/// <param name="progress">An optional progress reporter</param>
		/// 
		/// <exception cref="T:System.Net.WebException">
		/// The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress"/> and <paramref name="address"/> 
		/// is invalid.-or- An error occurred while downloading the resource. 
		/// </exception>
		/// 
		/// <exception cref="T:System.InvalidOperationException">The local file specified by <paramref name="fileName"/> 
		/// is in use by another thread.
		/// </exception>
		/// 
		/// <returns>A task representing the asynchronous operation</returns>
		Task DownloadFileAsync(Uri address, string fileName, IProgress<DownloadProgressChangedEventArgs> progress = null);
	}
}