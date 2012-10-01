using System;
using System.Diagnostics;
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
			var result = await new ProcessStartInfo
			{
				FileName = GraphVizExecutable.FullName,
				Arguments = "-V",
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}.ToTask(new MemoryStream(), CancellationToken.None).ConfigureAwait(false);

			// For some reason output is written to standard error.
			var output = Encoding.Default.GetString(
				await result.Item2.Async().ReadAllBytesAsync(CancellationToken.None).ConfigureAwait(false));
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