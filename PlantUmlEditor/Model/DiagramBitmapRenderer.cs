using System.Windows.Media;
using PlantUmlEditor.Core;
using Utilities.Controls.Converters;

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
