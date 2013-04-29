//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Manages diagrams open for editing.
	/// </summary>
	public interface IDiagramManager
	{
		/// <summary>
		/// The open diagram currently selected for editing.
		/// </summary>
		IDiagramEditor OpenDiagram { get; set; }

		/// <summary>
		/// The currently open diagrams.
		/// </summary>
		ICollection<IDiagramEditor> OpenDiagrams { get; }

		/// <summary>
		/// Opens a diagram for editing.
		/// </summary>
		/// <param name="diagram">The diagram to open</param>
		void OpenDiagramForEdit(PreviewDiagramViewModel diagram);

		/// <summary>
		/// Event raised when a diagram is opened for editing.
		/// </summary>
		event EventHandler<DiagramOpenedEventArgs> DiagramOpened;

		/// <summary>
		/// Event raised when an open diagram has closed.
		/// </summary>
		event EventHandler<DiagramClosedEventArgs> DiagramClosed;

		/// <summary>
		/// Asynchronously saves all open diagrams.
		/// </summary>
		Task SaveAllAsync();

		/// <summary>
		/// Closes a diagram manager.
		/// </summary>
		void Close();

		/// <summary>
		/// Event raised when a diagram manager is closing.
		/// This occurs before any final cleanup/save operations.
		/// </summary>
		event EventHandler<EventArgs> Closing;
	}
}