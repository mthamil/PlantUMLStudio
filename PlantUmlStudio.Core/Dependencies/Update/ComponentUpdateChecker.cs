//  PlantUML Studio
//  Copyright 2014 Matthew Hamilton - matthamilton@live.com
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

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Chronology;
using Utilities.InputOutput;

namespace PlantUmlStudio.Core.Dependencies.Update
{
	/// <summary>
	/// Checks for and downloads updates of third party components.
	/// </summary>
    public class ComponentUpdateChecker : IComponentUpdateChecker
	{
		/// <summary>
		/// Initializes a new <see cref="ComponentUpdateChecker"/>.
		/// </summary>
		/// <param name="clock">The system clock</param>
        /// <param name="httpClient">Used for web requests</param>
		public ComponentUpdateChecker(IClock clock, HttpClient httpClient)
		{
		    _clock = clock;
		    _httpClient = httpClient;
		}

	    /// <summary>
		/// The location to download updates from.
		/// </summary>
		public Uri DownloadLocation { get; set; }

		/// <summary>
		/// The local location of the file to update.
		/// </summary>
		public FileInfo LocalLocation { get; set; }

        /// <summary>
        /// Used for web requests.
        /// </summary>
	    protected HttpClient HttpClient
	    {
	        get { return _httpClient; }
	    }

	    #region IComponentUpdateChecker Members

		/// <see cref="IComponentUpdateChecker.HasUpdateAsync"/>
		public virtual Task<Option<string>> HasUpdateAsync(CancellationToken cancellationToken)
        {
			return Task.FromResult(Option<string>.None());
        }

		/// <see cref="IComponentUpdateChecker.DownloadLatestAsync"/>
        public async Task DownloadLatestAsync(IProgress<DownloadProgressChangedEventArgs> progress, CancellationToken cancellationToken)
        {
			if (LocalLocation.Exists)
			{
				// Make a backup in case the new version has issues.
				var backupFile = new FileInfo(String.Format("{0}_{1:yyyyMMdd_HHmmss}.bak", LocalLocation.FullName, _clock.Now));
				await LocalLocation.CopyToAsync(backupFile, true, cancellationToken).ConfigureAwait(false);
                LocalLocation.Delete();
			}

            var response = await HttpClient.GetAsync(DownloadLocation, cancellationToken).ConfigureAwait(false);
            using (var source = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var destination = LocalLocation.Open(FileMode.OpenOrCreate))
			{
			    await source.CopyToAsync(destination).ConfigureAwait(false);
			}
        }

		#endregion IComponentUpdateChecker Members

		private readonly IClock _clock;
	    private readonly HttpClient _httpClient;
	}
}
