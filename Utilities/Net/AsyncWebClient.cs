using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Concurrency;

namespace Utilities.Net
{
	/// <summary>
	/// Provides asynchronous WebClient operations.
	/// </summary>
	internal class AsyncWebClient : IAsyncWebClient
	{
		/// <summary>
		/// Initializes a new AsyncWebClient.
		/// </summary>
		/// <param name="webClient">The web client to use</param>
		public AsyncWebClient(WebClient webClient)
		{
			_webClient = webClient;
		}

		#region Implementation of IAsyncWebClient

		/// <see cref="IAsyncWebClient.DownloadFileAsync"/>
		public Task DownloadFileAsync(Uri address, string fileName, CancellationToken cancellationToken, IProgress<DownloadProgressChangedEventArgs> progress)
		{
			cancellationToken.Register(() => _webClient.CancelAsync());

			var cookie = Guid.NewGuid();
			var tcs = new TaskCompletionSource<object>();

			DownloadProgressChangedEventHandler progressHandler = (o, e) =>
			{
				if (!Equals(e.UserState, cookie))
					return;

				progress.Report(e);
			};
			if (progress != null)
				_webClient.DownloadProgressChanged += progressHandler;

			AsyncCompletedEventHandler completedHandler = null;
			completedHandler = (o, e) =>
			{
				if (!Equals(e.UserState, cookie))
					return;

				_webClient.DownloadProgressChanged -= progressHandler;
				_webClient.DownloadFileCompleted -= completedHandler;

				if (e.Cancelled)
					tcs.SetCanceled();
				else if (e.Error != null)
					tcs.SetException(e.Error);
				else
					tcs.SetResult(null);
			};
			_webClient.DownloadFileCompleted += completedHandler;

			_webClient.DownloadFileAsync(address, fileName, cookie);
			return tcs.Task;
		}

		#endregion

		private readonly WebClient _webClient;
	}
}