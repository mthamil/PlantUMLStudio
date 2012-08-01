using System;
using System.IO;
using System.Text;
using System.Net;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Concurrency;
using Utilities.Net;

namespace Utilities
{
	/// <summary>
	/// Checks for and downloads updates of PlantUML.
	/// </summary>
    public class PlantUmlUpdateChecker
    {
		/// <summary>
		/// The location to download updates from.
		/// </summary>
		public Uri DownloadUrl { get; set; }

		/// <summary>
		/// The local location of the file to update.
		/// </summary>
		public FileInfo LocalVersion { get; set; }

		/// <summary>
		/// Whether an update is available.
		/// </summary>
    	public bool HasUpdate(string versionUrl, string currentVersion)
        {
            using (WebClient client = new WebClient())
            {
                var serverVersionBytes = client.DownloadData(versionUrl);
                var serverVersion = Encoding.Unicode.GetString(Encoding.Convert(
                    Encoding.UTF8, 
                    Encoding.Unicode, serverVersionBytes));
                return String.Compare(serverVersion, currentVersion, true, CultureInfo.InvariantCulture) != 0;
            }
        }

		/// <summary>
		/// Retrieves the latest PlantUML version.
		/// </summary>
        public Task DownloadLatestAsync()
        {
			if (LocalVersion.Exists)
			{
				var backupFilePath = String.Format("{0}_{1:yyyyMMdd_HHmmss}.bak", LocalVersion.FullName, DateTime.Now);
				LocalVersion.CopyTo(backupFilePath, true);
			}

			var progress = new Progress<DownloadProgressChangedEventArgs>(p =>
			{

			});

			var temp = new FileInfo(LocalVersion.FullName + ".tmp");

			var webClient = new WebClient();
			return webClient.Async()
				.DownloadFileAsync(DownloadUrl, temp.FullName, CancellationToken.None, progress)
				.Then(() =>
				{
					LocalVersion.Delete();
					temp.CopyTo(LocalVersion.FullName);
					temp.Delete();
				})
				.ContinueWith(t => webClient.Dispose());
        }
    }
}
