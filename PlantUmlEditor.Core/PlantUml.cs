using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PlantUmlEditor.Core.Dependencies;
using PlantUmlEditor.Core.Dependencies.Update;
using Utilities;
using Utilities.Chronology;
using Utilities.Concurrency.Processes;
using Utilities.InputOutput;
using Utilities.Net;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Provides an interface to PlantUML.
	/// </summary>
	public class PlantUml : ComponentUpdateChecker, IExternalComponent, IDiagramCompiler
	{
		/// <summary>
		/// Initializes the PlantUML wrapper.
		/// </summary>
		/// <param name="clock">The system clock</param>
		public PlantUml(IClock clock) 
			: base(clock)
		{
		}

		/// <see cref="IDiagramCompiler.CompileToImageAsync"/>
		public async Task<BitmapSource> CompileToImageAsync(string diagramCode, CancellationToken cancellationToken)
		{
			var result = await new ProcessStartInfo
			{
				FileName = "java",
				Arguments = String.Format(@"-jar ""{0}"" -quiet -graphvizdot ""{1}"" -pipe", PlantUmlJar.FullName, GraphVizExecutable.FullName),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}.ToTask(new MemoryStream(Encoding.Default.GetBytes(diagramCode)), cancellationToken).ConfigureAwait(false);

			await HandleErrorStream(result.Item2).ConfigureAwait(false);

			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.StreamSource = result.Item1;
			bitmap.EndInit();
			bitmap.Freeze();
			return (BitmapSource)bitmap;
		}
		
		/// <see cref="IDiagramCompiler.CompileToFileAsync"/>
		public Task CompileToFileAsync(FileInfo diagramFile)
		{
			return new ProcessStartInfo
			{
				FileName = "java",
				Arguments = String.Format(@"-jar ""{0}"" -quiet -graphvizdot ""{1}"" ""{2}""", PlantUmlJar.FullName, GraphVizExecutable.FullName, diagramFile.FullName),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardError = true,
				UseShellExecute = false
			}.ToTask(CancellationToken.None);
		}

		#region Implementation of IExternalComponent

		/// <see cref="IExternalComponent.Name"/>
		public string Name { get { return PlantUmlJar.Name; } }

		/// <see cref="IExternalComponent.GetCurrentVersionAsync"/>
		public async Task<string> GetCurrentVersionAsync()
		{
			var result = await new ProcessStartInfo
			{
				FileName = "java",
				Arguments = String.Format(@"-jar ""{0}"" -version", PlantUmlJar.FullName),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}.ToTask(new MemoryStream(), CancellationToken.None).ConfigureAwait(false);

			await HandleErrorStream(result.Item2).ConfigureAwait(false);

			var output = Encoding.Default.GetString(
				await result.Item1.Async().ReadAllBytesAsync(CancellationToken.None).ConfigureAwait(false));
			var match = LocalVersionMatchingPattern.Match(output);
			return match.Groups["version"].Value;
		}

		#endregion

		#region Implementation of IComponentUpdateChecker

		/// <summary>
		/// The location of the latest PlantUML version number.
		/// </summary>
		public Uri VersionLocation { get; set; }

		/// <summary>
		/// Pattern used to find the latest version.
		/// </summary>
		public Regex RemoteVersionMatchingPattern { get; set; }

		/// <see cref="IComponentUpdateChecker.HasUpdateAsync"/>
		public override async Task<Option<string>> HasUpdateAsync(CancellationToken cancellationToken)
		{
			string currentVersion = await GetCurrentVersionAsync().ConfigureAwait(false);

			// Scrape the PlantUML downloads page for the latest version number.
			using (var client = new WebClient())
			{
				var downloadPage = await client.Async().DownloadStringAsync(VersionLocation, cancellationToken).ConfigureAwait(false);
				var match = RemoteVersionMatchingPattern.Match(downloadPage);
				if (match.Success)
				{
					string remoteVersion = match.Groups["version"].Value;
					bool versionsNotEqual = String.Compare(remoteVersion, currentVersion, true, CultureInfo.InvariantCulture) != 0;
					if (versionsNotEqual)
						return remoteVersion;
				}
			}
			
			return Option<string>.None();
		}

		#endregion

		private static async Task HandleErrorStream(Stream errorStream)
		{
			if (errorStream.Length > 0)
			{
				string errorMessage = Encoding.Default.GetString(
					await errorStream.Async().ReadAllBytesAsync(CancellationToken.None).ConfigureAwait(false));
				throw new PlantUmlException(errorMessage);
			}
		}

		/// <summary>
		/// The location of the GraphViz executable.
		/// </summary>
		public FileInfo GraphVizExecutable { get; set; }

		/// <summary>
		/// The location of the plantuml.jar file.
		/// </summary>
		public FileInfo PlantUmlJar { get; set; }

		/// <summary>
		/// Pattern used to extract the current version.
		/// </summary>
		public Regex LocalVersionMatchingPattern { get; set; }
	}
}