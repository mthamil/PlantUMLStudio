using System.Windows.Media;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Interface for an object that renders diagram files.
	/// </summary>
	public interface IDiagramRenderer
	{
		/// <summary>
		/// Renders a diagram file to an image.
		/// </summary>
		/// <param name="diagram">The diagram to render</param>
		/// <returns>A rendered image</returns>
		ImageSource Render(DiagramFile diagram);
	}
}