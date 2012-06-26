using System.IO;
using PlantUmlEditor.Model;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Reads diagrams.
	/// </summary>
	public interface IDiagramReader
	{
		/// <summary>
		/// Reads a diagram from a file.
		/// </summary>
		DiagramFile Read(FileInfo file);
	}
}