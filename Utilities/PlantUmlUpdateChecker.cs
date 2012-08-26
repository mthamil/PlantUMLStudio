using System;
using System.IO;
using System.Text;
using System.Net;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Chronology;
using Utilities.InputOutput;
using Utilities.Net;

namespace Utilities
{
	/// <summary>
	/// Checks for and downloads updates of PlantUML.
	/// </summary>
    public class PlantUmlUpdateChecker
    {
		public PlantUmlUpdateChecker(IClock clock)
		{
			_clock = clock;
		}

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
    	public bool HasUpdate(Uri versionUrl, string currentVersion)
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
        public async Task DownloadLatestAsync(CancellationToken cancellationToken, IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
			if (LocalVersion.Exists)
			{
				var backupFile = new FileInfo(String.Format("{0}_{1:yyyyMMdd_HHmmss}.bak", LocalVersion.FullName, DateTime.Now));
				await LocalVersion.CopyToAsync(backupFile, true);
			}

			var temp = new FileInfo(LocalVersion.FullName + ".tmp");

			using (var webClient = new WebClient())
			{
				await webClient.Async().DownloadFileAsync(DownloadUrl, temp.FullName, cancellationToken, progress);
				LocalVersion.Delete();
				await temp.CopyToAsync(LocalVersion, false);
				temp.Delete();
			}
        }

		private readonly IClock _clock;
    }
}
