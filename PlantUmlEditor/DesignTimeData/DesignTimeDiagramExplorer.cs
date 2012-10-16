//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
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