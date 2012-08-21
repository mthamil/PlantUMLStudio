using System;
using System.Collections.Generic;
using System.IO;
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
		/// <param name="progress">An optional progress reporter</param>
		/// <returns>A task with the loaded diagrams</returns>
		Task<IEnumerable<Diagram>> ReadDiagramsAsync(DirectoryInfo directory, IProgress<Tuple<int, int>> progress = null);

		/// <summary>
		/// Asynchronously reads a single diagram from a file.
		/// </summary>
		/// <param name="file">The diagram file to read</param>
		/// <returns>The loaded diagram</returns>
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
		event EventHandler<DiagramFileAddedEventArgs> DiagramAdded;

		/// <summary>
		/// Raised when a diagram is deleted.
		/// </summary>
		event EventHandler<DiagramFileDeletedEventArgs> DiagramDeleted; 
	}
}