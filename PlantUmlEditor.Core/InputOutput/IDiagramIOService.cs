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
using System.Threading;
using System.Threading.Tasks;

namespace PlantUmlEditor.Core.InputOutput
{
	/// <summary>
	/// Provides operations related to diagram reading/writing.
	/// </summary>
	public interface IDiagramIOService
	{
		/// <summary>
		/// Asynchronously loads all diagram files from a directory.
		/// </summary>
		/// <param name="directory">The directory to load from</param>
		/// <param name="cancellationToken">Allows cancellation of diagram reading</param>
		/// <param name="progress">An optional progress reporter</param>
		/// <returns>A task with the loaded diagrams</returns>
		Task<IEnumerable<Diagram>> ReadDiagramsAsync(DirectoryInfo directory, CancellationToken cancellationToken, IProgress<ReadDiagramsProgress> progress = null);

		/// <summary>
		/// Asynchronously reads a single diagram from a file.
		/// </summary>
		/// <param name="file">The diagram file to read</param>
		/// <returns>The loaded diagram or null if a file was not a diagram file</returns>
		Task<Diagram> ReadAsync(FileInfo file);

		/// <summary>
		/// Asynchronously saves a diagram's contents to its file.
		/// </summary>
		/// <param name="diagram">The diagram to write</param>
		/// <param name="makeBackup">Whether to first back up the existing file before saving</param>
		/// <returns>A task representing the save operation</returns>
		Task SaveAsync(Diagram diagram, bool makeBackup);

		/// <summary>
		/// Asynchronously deletes a diagram file.
		/// </summary>
		/// <param name="diagram">The diagram to delete</param>
		/// <returns>A task representing the delete operation</returns>
		Task DeleteAsync(Diagram diagram);

		/// <summary>
		/// Begins monitoring a directory for changes.
		/// </summary>
		/// <param name="directory">The directory to monitor</param>
		void StartMonitoring(DirectoryInfo directory);

		/// <summary>
		/// Stops monitoring file system changes.
		/// </summary>
		void StopMonitoring();

		/// <summary>
		/// Raised when a new diagram is added.
		/// </summary>
		event EventHandler<DiagramFileAddedEventArgs> DiagramFileAdded;

		/// <summary>
		/// Raised when a diagram is deleted.
		/// </summary>
		event EventHandler<DiagramFileDeletedEventArgs> DiagramFileDeleted; 
	}
}