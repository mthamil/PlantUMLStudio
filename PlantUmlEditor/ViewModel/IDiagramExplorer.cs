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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PlantUmlEditor.Core;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Provides browsing and previewing of diagram files.
	/// </summary>
	public interface IDiagramExplorer
	{
		/// <summary>
		/// The current explorer directory.
		/// </summary>
		DirectoryInfo DiagramLocation { get; }

		/// <summary>
		/// The currently available diagrams.
		/// </summary>
		ICollection<PreviewDiagramViewModel> PreviewDiagrams { get; }

		/// <summary>
		/// Asynchronously requests loading of a diagram located at the given URI.
		/// </summary>
		/// <param name="diagramPath">The path to the diagram file</param>
		/// <returns>A Task that can be used to wait on the operation</returns>
		Task<Diagram> OpenDiagramAsync(Uri diagramPath);

		/// <summary>
		/// Event raised when a preview diagram should be opened for editing.
		/// </summary>
		event EventHandler<OpenPreviewRequestedEventArgs> OpenPreviewRequested;

		/// <summary>
		/// Event raised when a diagram has been deleted.
		/// </summary>
		event EventHandler<DiagramDeletedEventArgs> DiagramDeleted;
	}
}