using System;
using System.IO;

namespace PlantUmlEditor.Core.InputOutput
{
	/// <summary>
	/// Information about when a diagram deletion is detected.
	/// </summary>
	public class DiagramDeletedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="deletedDiagramFile">The deleted diagram file</param>
		public DiagramDeletedEventArgs(FileInfo deletedDiagramFile)
		{
			DeletedDiagramFile = deletedDiagramFile;
		}

		/// <summary>
		/// The deleted diagram.
		/// </summary>
		public FileInfo DeletedDiagramFile { get; private set; }
	}
}