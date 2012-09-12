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
		/// Determines whether an update is available for the dependency.
		/// </summary>
		/// <returns>If an update is available, returns the new version</returns>
		Task<Option<string>> HasUpdateAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Retrieves the dependency's latest version.
		/// </summary>
		Task DownloadLatestAsync(CancellationToken cancellationToken, IProgress<DownloadProgressChangedEventArgs> progress = null);
	}
}