using System.IO;

namespace PlantUmlEditor.Configuration
{
	/// <summary>
	/// Interface for application settings.
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// The GraphViz executable.
		/// </summary>
		FileInfo GraphVizExecutable { get; set; }

		/// <summary>
		/// The last directory diagrams were loaded from.
		/// </summary>
		DirectoryInfo LastDiagramLocation { get; set; }

		/// <summary>
		/// Stores the current settings. 
		/// </summary>
		void Save();
	}
}