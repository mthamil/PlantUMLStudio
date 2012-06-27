namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Interface for an object that compiles diagrams.
	/// </summary>
	public interface IDiagramCompiler
	{
		/// <summary>
		/// Compiles a diagram's code into a visual representation.
		/// </summary>
		/// <param name="diagram">The diagram to compile</param>
		void Compile(Diagram diagram);
	}
}