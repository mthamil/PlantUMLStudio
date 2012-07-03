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
		/// <param name="taskScheduler">Used to schedule tasks</param>
		public AsyncWebClient(WebClient webClient, TaskScheduler taskScheduler)
		{
			_webClient = webClient;
			_taskScheduler = taskScheduler;
		}

		#region Implementation of IAsyncWebClient

		/// <see cref="IAsyncWebClient.DownloadFileAsync"/>
		public Task DownloadFileAsync(Uri address, string fileName, IProgress<DownloadProgressChangedEventArgs> progress)
		{
			var token = Guid.NewGuid();
			var tcs = new TaskCompletionSource<object>();
			AsyncCompletedEventHandler completedHandler = (o, e) =>
			{
				if (!Equals(e.UserState, token))
					return;

				if (e.Cancelled)
					tcs.SetCanceled();
				else if (e.Error != null)
					tcs.SetException(e.Error);
				else
					tcs.SetResult(null);
			};
			_webClient.DownloadFileCompleted += completedHandler;

			DownloadProgressChangedEventHandler progressHandler = (o, e) =>
			{
				if (!Equals(e.UserState, token))
					return;

				progress.Report(e);
			};
			if (progress != null)
				_webClient.DownloadProgressChanged += progressHandler;

			return Task.Factory.StartNew(() =>
			{
				_webClient.DownloadFileAsync(address, fileName, token);

				return tcs.Task.ContinueWith(t =>
				{
					_webClient.DownloadProgressChanged -= progressHandler;
					_webClient.DownloadFileCompleted -= completedHandler;
				});
			}, CancellationToken.None, TaskCreationOptions.None, _taskScheduler).Unwrap();
		}

		#endregion

		private readonly WebClient _webClient;
		private readonly TaskScheduler _taskScheduler;
	}
}