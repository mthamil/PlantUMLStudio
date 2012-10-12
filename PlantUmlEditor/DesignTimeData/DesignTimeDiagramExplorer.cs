using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using PlantUmlEditor.Core;
using PlantUmlEditor.ViewModel;

namespace PlantUmlEditor.DesignTimeData
{
	/// <summary>
	/// Used for designer support.
	/// </summary>
	public class DesignTimeDiagramExplorer : DiagramExplorerViewModel
	{
		public DesignTimeDiagramExplorer()
		{
			DiagramLocation = GetDesignTimeDiagramPath();
			foreach (var diagramFile in DiagramLocation.EnumerateFiles("*.puml"))
			{
				var imagePath = Path.Combine(diagramFile.DirectoryName, Path.GetFileNameWithoutExtension(diagramFile.Name) + ".png");
				PreviewDiagrams.Add(
					new PreviewDiagramViewModel(
						new Diagram
						{
							Content = File.ReadAllText(diagramFile.FullName),
							File = diagramFile,
							ImageFilePath = imagePath
						})
					{
						ImagePreview = BitmapFrame.Create(new Uri(imagePath))
					});
			}
		}

		private DirectoryInfo GetDesignTimeDiagramPath([CallerFilePath] string path = "")
		{
			return new DirectoryInfo(Path.GetDirectoryName(path));
		}

		public new DirectoryInfo DiagramLocation { get; set; }
	}
}