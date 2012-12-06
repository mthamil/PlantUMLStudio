//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace PlantUmlEditor.Core.Dependencies.Update
{
	/// <summary>
	/// Checks for and downloads updates of third party components.
	/// </summary>
	public interface IComponentUpdateChecker
	{
		/// <summary>
		/// Determines whether an update is available for a dependency.
		/// </summary>
		/// <param name="cancellationToken">An optional token that can cancel checking for updates</param>
		/// <returns>If an update is available, returns the new version</returns>
		Task<Option<string>> HasUpdateAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieves a dependency's latest version.
		/// </summary>
		/// <param name="cancellationToken">An optional token that can cancel download of an update</param>
		/// <param name="progress">An optional object that can report progress on an update download</param>
		Task DownloadLatestAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null);
	}
}