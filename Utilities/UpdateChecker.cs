using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Globalization;

namespace Utilities
{
    public class UpdateChecker
    {

        public static Action<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        public static Action<System.ComponentModel.AsyncCompletedEventArgs> DownloadCompleted;
        public static string DownloadedLocation;

        public static bool HasUpdate(string versionUrl, string currentVersion)
        {
            using (WebClient client = new WebClient())
            {
                var serverVersionBytes = client.DownloadData(versionUrl);
                var serverVersion = Encoding.Unicode.GetString(Encoding.Convert(
                    Encoding.UTF8, 
                    Encoding.Unicode, serverVersionBytes));
                return string.Compare(serverVersion, currentVersion, true, CultureInfo.InvariantCulture) != 0;
            }
            //HttpWebRequest request = WebRequest.Create(downloadUrl) as HttpWebRequest;
            //using (var response = request.GetResponse())
            //{
            //    var lastModifiedDate = default(DateTime);
            //    if (DateTime.TryParse(response.Headers["Last-Modified"], out lastModifiedDate))
            //    {
            //        var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //        var localFileDateTime = File.GetLastWriteTime(path);

            //        return (localFileDateTime < lastModifiedDate.AddDays(-1));
            //    }
            //}
        }

        public static void DownloadLatestUpdate(string downloadUrl, string localPath)
        {
            DownloadedLocation = localPath;
            using (WebClient client = new WebClient())
            {
                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileAsync(new Uri(downloadUrl), localPath);
            }
        }

        static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged(e);
        }

        static void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            DownloadCompleted(e);
        }
    }
}
