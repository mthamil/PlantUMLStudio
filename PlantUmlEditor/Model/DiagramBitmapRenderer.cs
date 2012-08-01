using System.Windows.Media;
using PlantUmlEditor.Converters;
using PlantUmlEditor.Core;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Renders diagrams.
	/// </summary>
	public class DiagramBitmapRenderer : IDiagramRenderer
	{
		/// <see cref="IDiagramRenderer.Render"/>
		public ImageSource Render(Diagram diagram)
		{
			return (ImageSource)new UriToCachedImageConverter().Convert(diagram.ImageFilePath, null, null, null);
		}
	}
}
