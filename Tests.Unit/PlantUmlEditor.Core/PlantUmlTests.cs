using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PlantUmlEditor.Core;
using Utilities.Chronology;
using Xunit;

namespace Tests.Unit.PlantUmlEditor.Core
{
	public class PlantUmlTests
	{
		[Fact]
		public async Task Test_CompileDiagramFileAsync()
		{
			// Arrange.
			var plantUml = new PlantUml(new Mock<IClock>().Object)
			{
				PlantUmlJar = new FileInfo(@"C:\Users\mhamilt\Documents\Visual Studio 2010\Projects\PlantUmlEditor\PlantUmlEditor\bin\Debug\PlantUML\plantuml.jar"),
				GraphVizExecutable = new FileInfo(@"C:\Program Files (x86)\Graphviz2.26.3\bin\dot.exe")
			};

			// Act.
			var image = await plantUml.CompileToImageAsync(code, CancellationToken.None);

			// Assert.
			Assert.NotNull(image);
		}

		private const string code = @"

@startuml class.png

title Class Diagram

package ""Classic Collections"" #DDDDDD
Object <|-- ArrayList
end package

package net.sourceforge.plantuml
Object <|-- Demo1
Demo1 -* Demo2
end package


class C {

}

enum TimeUnit {
  DAYS
  HOURS
  MINUTES
}

@enduml
";
	}

	//using (var filestream = new FileStream(inputFile.FullName.Replace(".puml", ".png"), FileMode.Create))
	//{
	//    var encoder = new PngBitmapEncoder();
	//    encoder.Frames.Add(BitmapFrame.Create(compileTask.Result));
	//    encoder.Save(filestream);
	//}
}