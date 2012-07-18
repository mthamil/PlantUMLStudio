using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Utilities.Concurrency.Processes;
using Utilities.Concurrency;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Provides an interface to PlantUML.
	/// </summary>
	public class PlantUml : IDiagramCompiler
	{
		/// <see cref="IDiagramCompiler.CompileToImage"/>
		public Task<BitmapSource> CompileToImage(string diagramCode, CancellationToken cancellationToken)
		{
			return new ProcessStartInfo
			{
				FileName = "java",
				Arguments = String.Format(@"-jar ""{0}"" -quiet -graphvizdot ""{1}"" -pipe", PlantUmlJar.FullName, GraphVizExecutable.FullName),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}.ToTask(new MemoryStream(Encoding.Default.GetBytes(diagramCode)), cancellationToken)
			.Then(stream => 
				{
					var bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.StreamSource = stream;
					bitmap.EndInit();
					bitmap.Freeze();
					return (BitmapSource)bitmap;
				});
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

		/// <summary>
		/// The location of the GraphViz executable.
		/// </summary>
		public FileInfo GraphVizExecutable { get; set; }

		/// <summary>
		/// The location of the plantuml.jar file.
		/// </summary>
		public FileInfo PlantUmlJar { get; set; }
	}
}