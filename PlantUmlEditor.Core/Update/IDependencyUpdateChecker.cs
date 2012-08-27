using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PlantUmlEditor.Core.Update
{
	/// <summary>
	/// Checks for and downloads updates of third party dependencies.
	/// </summary>
	public interface IDependencyUpdateChecker
	{
		/// <summary>
		/// Determines whether an update is available.
		/// </summary>
		Task<bool> HasUpdateAsync();

		/// <summary>
		/// Retrieves the latest PlantUML version.
		/// </summary>
		Task DownloadLatestAsync(CancellationToken cancellationToken, IProgress<DownloadProgressChangedEventArgs> progress = null);
	}
}