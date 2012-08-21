using System;
using PlantUmlEditor.Core;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Event args containing information about when a diagram's underlying file has been deleted.
	/// </summary>
	public class DiagramDeletedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="deletedDiagram">The deleted diagram</param>
		public DiagramDeletedEventArgs(Diagram deletedDiagram)
		{
			DeletedDiagram = deletedDiagram;
		}

		/// <summary>
		/// The deleted diagram.
		/// </summary>
		public Diagram DeletedDiagram { get; private set; }
	}
}