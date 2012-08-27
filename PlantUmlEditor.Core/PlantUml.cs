using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Utilities.Concurrency;
using Utilities.Concurrency.Processes;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Provides an interface to PlantUML.
	/// </summary>
	public class PlantUml : IDiagramCompiler
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

		/// <see cref="IDiagramCompiler.GetCurrentVersion"/>
		public async Task<string> GetCurrentVersion()
		{
			// For some reason PlantUML writes this data to the error stream.
			try
			{
				await new ProcessStartInfo
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
			}
			catch (ProcessErrorException ex)
			{
				int versionIndex = ex.Message.IndexOf(versionToken);
				if (versionIndex > -1)
				{
					string version = ex.Message.Substring(versionIndex + versionToken.Length).Trim();
					int versionEndIndex = version.IndexOf(' ');
					version = version.Substring(0, versionEndIndex);
					return version;
				}
				throw;
			}

			return null;
		}

		/// <summary>
		/// The location of the GraphViz executable.
		/// </summary>
		public FileInfo GraphVizExecutable { get; set; }

		/// <summary>
		/// The location of the plantuml.jar file.
		/// </summary>
		public FileInfo PlantUmlJar { get; set; }

		private const string versionToken = "version";
	}
}