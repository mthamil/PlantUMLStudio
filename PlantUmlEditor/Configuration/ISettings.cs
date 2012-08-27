using System;
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
		/// The PlantUML jar.
		/// </summary>
		FileInfo PlantUmlJar { get; set; }

		/// <summary>
		/// The last directory diagrams were loaded from.
		/// </summary>
		DirectoryInfo LastDiagramLocation { get; set; }

		/// <summary>
		/// The URL where PlantUML can be downloaded.
		/// </summary>
		Uri PlantUmlDownloadLocation { get; }

		/// <summary>
		/// The file extension to use for diagrams.
		/// </summary>
		string DiagramFileExtension { get; }

		/// <summary>
		/// Stores the current settings. 
		/// </summary>
		void Save();
	}
}