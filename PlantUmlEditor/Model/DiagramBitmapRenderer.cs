using System.Windows.Media;
using PlantUmlEditor.Converters;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Renders diagrams.
	/// </summary>
	public class DiagramBitmapRenderer : IDiagramRenderer
	{
		/// <see cref="IDiagramRenderer.Render"/>
		public ImageSource Render(DiagramFile diagram)
		{
			return (ImageSource)new UriToCachedImageConverter().Convert(diagram.ImageFilePath, null, null, null);
		}
	}
}
