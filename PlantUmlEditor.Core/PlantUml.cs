using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Utilities.Concurrency.Processes;
using Utilities.InputOutput;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Provides an interface to PlantUML.
	/// </summary>
	public class PlantUml : IPlantUml
	{
		/// <see cref="IDiagramCompiler.CompileToImage"/>
		public async Task<BitmapSource> CompileToImage(string diagramCode, CancellationToken cancellationToken)
		{
			var output = await new ProcessStartInfo
			{
				FileName = "java",
				Arguments = String.Format(@"-jar ""{0}"" -quiet -graphvizdot ""{1}"" -pipe", PlantUmlJar.FullName, GraphVizExecutable.FullName),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}.ToTask(new MemoryStream(Encoding.Default.GetBytes(diagramCode)), cancellationToken);

			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.StreamSource = output;
			bitmap.EndInit();
			bitmap.Freeze();
			return (BitmapSource)bitmap;
		}
		
		/// <see cref="IDiagramCompiler.CompileToFile"/>
		public Task CompileToFile(FileInfo diagramFile)
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

		/// <see cref="IPlantUml.GetCurrentVersion"/>
		public async Task<string> GetCurrentVersion()
		{
			var outputStream = await new ProcessStartInfo
			{
				FileName = "java",
				Arguments = String.Format(@"-jar ""{0}"" -version", PlantUmlJar.FullName),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}.ToTask(new MemoryStream(), CancellationToken.None);

			var resultData = await outputStream.Async().ReadAllBytesAsync(CancellationToken.None);
			var output = Encoding.Default.GetString(resultData);
			var match = VersionMatchingPattern.Match(output);
			return match.Groups["version"].Value;
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
		public Regex VersionMatchingPattern { get; set; }
	}
}