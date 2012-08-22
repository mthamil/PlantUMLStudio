using System;
using System.IO;

namespace PlantUmlEditor.Core.InputOutput
{
	/// <summary>
	/// Information about when a new diagram file is detected.
	/// </summary>
	public class DiagramFileAddedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="newDiagramFile">The new diagram file</param>
		public DiagramFileAddedEventArgs(FileInfo newDiagramFile)
		{
			NewDiagramFile = newDiagramFile;
		}

		/// <summary>
		/// The new diagram file.
		/// </summary>
		public FileInfo NewDiagramFile { get; private set; }
	}
}