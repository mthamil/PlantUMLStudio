//  PlantUML Editor
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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PlantUmlEditor.Core.Dependencies;
using PlantUmlEditor.Core.Dependencies.Update;
using Utilities;
using Utilities.Concurrency.Processes;
using Utilities.InputOutput;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Provides an interface to GraphViz.
	/// </summary>
	public class GraphViz : IExternalComponent
	{
		#region Implementation of IComponentUpdateChecker

		/// <see cref="IComponentUpdateChecker.HasUpdateAsync"/>
		public Task<Option<string>> HasUpdateAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(Option<string>.None());
		}

		/// <see cref="IComponentUpdateChecker.DownloadLatestAsync"/>
		public Task DownloadLatestAsync(CancellationToken cancellationToken, IProgress<DownloadProgressChangedEventArgs> progress = null)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Implementation of IExternalComponent

		/// <see cref="IExternalComponent.Name"/>
		public string Name { get { return GraphVizExecutable.Name; } }

		/// <see cref="IExternalComponent.GetCurrentVersionAsync"/>
		public async Task<string> GetCurrentVersionAsync()
		{
			var result = await Task.Factory.FromProcess(
				executable: GraphVizExecutable.FullName,
				arguments: "-V",
				input: Stream.Null
			).ConfigureAwait(false);

			// For some reason output is written to standard error.
			var output = Encoding.Default.GetString(
				await result.Error.Async().ReadAllBytesAsync(CancellationToken.None).ConfigureAwait(false));
			var match = LocalVersionMatchingPattern.Match(output);
			return match.Groups["version"].Value;
		}

		#endregion

		/// <summary>
		/// The location of the GraphViz executable.
		/// </summary>
		public FileInfo GraphVizExecutable { get; set; }

		/// <summary>
		/// Pattern used to extract the current version.
		/// </summary>
		public Regex LocalVersionMatchingPattern { get; set; }
	}
}