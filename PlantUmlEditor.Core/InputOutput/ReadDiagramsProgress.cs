using Utilities;

namespace PlantUmlEditor.Core.InputOutput
{
	/// <summary>
	/// Contains information about progress made in reading diagrams.
	/// </summary>
	public class ReadDiagramsProgress
	{
		/// <summary>
		/// Creates new progress data.
		/// </summary>
		/// <param name="processedDiagramCount">The number of diagrams read so far</param>
		/// <param name="totalDiagramCount">The total number of diagrams to read</param>
		/// <param name="diagram">The last read diagram</param>
		public ReadDiagramsProgress(int processedDiagramCount, int totalDiagramCount, Option<Diagram> diagram)
		{
			Diagram = diagram;
			TotalDiagramCount = totalDiagramCount;
			ProcessedDiagramCount = processedDiagramCount;
		}

		/// <summary>
		/// The number of diagrams read so far.
		/// </summary>
		public int ProcessedDiagramCount { get; private set; }

		/// <summary>
		/// The total number of diagrams to read.
		/// </summary>
		public int TotalDiagramCount { get; private set; }

		/// <summary>
		/// The last read diagram.
		/// </summary>
		public Option<Diagram> Diagram { get; private set; }
	}
}