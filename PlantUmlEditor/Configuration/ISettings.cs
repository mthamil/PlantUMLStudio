using System;
using System.IO;
using System.Text.RegularExpressions;

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
		/// The location where information about the latest PlantUML version can be found.
		/// </summary>
		Uri PlantUmlVersionSource { get; }

		/// <summary>
		/// The pattern used to find the latest PlantUML version at PlantUMLVersionSource.
		/// </summary>
		Regex PlantUmlRemoteVersionPattern { get; }

		/// <summary>
		/// The pattern used to find the current PlantUML version from PlantUML itself.
		/// </summary>
		Regex PlantUmlLocalVersionPattern { get; }

		/// <summary>
		/// The file extension to use for diagrams.
		/// </summary>
		string DiagramFileExtension { get; }

		/// <summary>
		/// The file containing syntax highlighting rules for PlantUML diagrams.
		/// </summary>
		FileInfo PlantUmlHighlightingDefinition { get; }

		/// <summary>
		/// Stores the current settings. 
		/// </summary>
		void Save();
	}
}