using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Chronology;
using Utilities.InputOutput;
using Utilities.Net;

namespace PlantUmlEditor.Core.Update
{
	/// <summary>
	/// Checks for and downloads updates of third party dependencies.
	/// </summary>
    public class DependencyUpdateChecker : IDependencyUpdateChecker
	{
		/// <summary>
		/// Initializes a new update checker.
		/// </summary>
		/// <param name="clock">The system clock</param>
		public DependencyUpdateChecker(IClock clock)
		{
			_clock = clock;
		}

		/// <summary>
		/// The location to download updates from.
		/// </summary>
		public Uri RemoteLocation { get; set; }

		/// <summary>
		/// The local location of the file to update.
		/// </summary>
		public FileInfo LocalLocation { get; set; }

		#region IDependencyUpdateChecker Members

		/// <see cref="IDependencyUpdateChecker.HasUpdateAsync"/>
    	public virtual Task<bool> HasUpdateAsync()
        {
			return Task.FromResult(false);
        }

		/// <see cref="IDependencyUpdateChecker.DownloadLatestAsync"/>
        public async Task DownloadLatestAsync(CancellationToken cancellationToken, IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
			if (LocalLocation.Exists)
			{
				// Make a backup in case the new version has issues.
				var backupFile = new FileInfo(String.Format("{0}_{1:yyyyMMdd_HHmmss}.bak", LocalLocation.FullName, _clock.Now));
				await LocalLocation.CopyToAsync(backupFile, true);
			}

			using (var webClient = new WebClient())
			{
				var temp = new FileInfo(LocalLocation.FullName + ".tmp");
				await webClient.Async().DownloadFileAsync(RemoteLocation, temp.FullName, cancellationToken, progress);
				LocalLocation.Delete();
				await temp.CopyToAsync(LocalLocation, false);
				temp.Delete();
			}
        }

		#endregion IDependencyUpdateChecker Members

		private readonly IClock _clock;
    }
}
