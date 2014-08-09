//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PlantUmlStudio.Core.Dependencies;
using PlantUmlStudio.Core.Dependencies.Update;
using Utilities.Chronology;
using Utilities.Concurrency.Processes;
using Utilities.InputOutput;

namespace PlantUmlStudio.Core
{
	/// <summary>
	/// Provides an interface to GraphViz.
	/// </summary>
	public class GraphViz : ComponentUpdateChecker, IExternalComponent
	{
	    /// <summary>
		/// Initializes a new instance of <see cref="GraphViz"/>.
		/// </summary>
        /// <param name="clock">The system clock</param>
        /// <param name="httpClient">Used for web requests</param>
        public GraphViz(IClock clock, HttpClient httpClient) 
            : base(clock, httpClient)
	    {
		}

		#region Implementation of IExternalComponent

		/// <see cref="IExternalComponent.Name"/>
		public string Name { get { return GraphVizExecutable.Name; } }

		#endregion

        #region Implementation of IComponentUpdateChecker

        /// <see cref="IExternalComponent.GetCurrentVersionAsync"/>
        public override async Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken)
        {
            if (!GraphVizExecutable.Exists)
                throw new FileNotFoundException("Component not found.", GraphVizExecutable.FullName);

            var result = await Task.Factory.FromProcess(
                executable: GraphVizExecutable.FullName,
                arguments: "-V",
                input: Stream.Null,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            // For some reason output is written to standard error.
            var output = Encoding.Default.GetString(
                await result.Error.Async().ReadAllBytesAsync(cancellationToken).ConfigureAwait(false));
            var match = LocalVersionPattern.Match(output);
            return match.Groups["version"].Value;
        }

        /// <see cref="IComponentUpdateChecker.DownloadLatestAsync"/>
        public override Task DownloadLatestAsync(IProgress<DownloadProgressChangedEventArgs> progress, CancellationToken cancellationToken)
        {
            Process.Start(DownloadLocation.ToString());
            return Task.FromResult<object>(null);
        }

        #endregion

		/// <summary>
		/// The location of the GraphViz executable.
		/// </summary>
		public FileInfo GraphVizExecutable { get; set; }

		/// <summary>
		/// Pattern used to extract the current version.
		/// </summary>
		public Regex LocalVersionPattern { get; set; }
	}
}