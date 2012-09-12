using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Interface for a service that compiles diagrams to images.
	/// </summary>
	public interface IDiagramCompiler
	{
		/// <summary>
		/// Creates an in-memory bitmap image from diagram code.
		/// No external files are created.
		/// </summary>
		/// <param name="diagramCode">The diagram code to compile</param>
		/// <param name="cancellationToken">An optional cancellation token</param>
		/// <returns>A Task representing the compilation operation</returns>
		Task<BitmapSource> CompileToImageAsync(string diagramCode, CancellationToken cancellationToken);

		/// <summary>
		/// Reads the code from a diagram file, compiles it to an image, and saves the output to a file.
		/// </summary>
		/// <param name="diagramFile">The diagram file to compile</param>
		/// <returns>A Task representing the compilation operation</returns>
		Task CompileToFileAsync(FileInfo diagramFile);
	}
}