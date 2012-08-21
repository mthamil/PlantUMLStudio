using System;

namespace PlantUmlEditor.Core.InputOutput
{
	/// <summary>
	/// Information about when a new diagram is detected.
	/// </summary>
	public class DiagramFileAddedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="newDiagram">The new diagram</param>
		public DiagramFileAddedEventArgs(Diagram newDiagram)
		{
			NewDiagram = newDiagram;
		}

		/// <summary>
		/// The new diagram.
		/// </summary>
		public Diagram NewDiagram { get; private set; }
	}
}