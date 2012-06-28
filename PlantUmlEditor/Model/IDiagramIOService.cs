using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utilities.Concurrency;

namespace PlantUmlEditor.Model
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
		Task<IList<Diagram>> ReadDiagramsAsync(DirectoryInfo directory, IProgress<Tuple<int?, string>> progress = null);

		/// <summary>
		/// Reads a single diagram from a file.
		/// </summary>
		/// <param name="file">The diagram file to read</param>
		/// <returns>The loaded diagram</returns>
		Diagram Read(FileInfo file);

		/// <summary>
		/// Asynchronously compiles and writes a diagram.
		/// </summary>
		/// <param name="diagram">The diagram to write</param>
		/// <param name="makeBackup">Whether to first back up the existing file before saving</param>
		/// <returns>A task representing the save operation</returns>
		Task SaveAsync(Diagram diagram, bool makeBackup);
	}
}