//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
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
		/// The pattern used to find the current GraphViz version from GraphViz itself.
		/// </summary>
		Regex GraphVizLocalVersionPattern { get; }

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