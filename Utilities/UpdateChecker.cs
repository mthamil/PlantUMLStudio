using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Net;
using System.Globalization;
using Utilities.Concurrency;
using Utilities.Net;

namespace Utilities
{
	/// <summary>
	/// Checks for updates.
	/// </summary>
    public class UpdateChecker
    {
		public Uri DownloadUrl { get; set; }

    	public bool HasUpdate(string versionUrl, string currentVersion)
        {
            using (WebClient client = new WebClient())
            {
                var serverVersionBytes = client.DownloadData(versionUrl);
                var serverVersion = Encoding.Unicode.GetString(Encoding.Convert(
                    Encoding.UTF8, 
                    Encoding.Unicode, serverVersionBytes));
                return string.Compare(serverVersion, currentVersion, true, CultureInfo.InvariantCulture) != 0;
            }
        }

		/// <summary>
		/// Retrieves the latest PlantUML version.
		/// </summary>
        public void DownloadLatestPlantUml()
        {
			var localFileDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var existingFileName = @"Thirdparty\" + Path.GetFileName(DownloadUrl.OriginalString);
			var localFilePath = Path.Combine(localFileDirectory, existingFileName);
			var localFile = new FileInfo(localFilePath);
			if (localFile.Exists)
			{
				var backupFilePath = String.Format("{0}_{1:yyyyMMdd_HHmmss}.bak", localFile.FullName, DateTime.Now);
				localFile.CopyTo(backupFilePath, true);
			}

			var webClient = new WebClient();
			var progress = new Progress<DownloadProgressChangedEventArgs>(p =>
			{

			});

			var downloadTask = webClient.Async().DownloadFileAsync(DownloadUrl, localFilePath + ".tmp", progress);
			downloadTask.ContinueWith(t =>
			{
				webClient.Dispose();
			});
        }
    }
}
