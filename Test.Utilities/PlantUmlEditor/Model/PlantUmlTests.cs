using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PlantUmlEditor.Model;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.Model
{
	public class PlantUmlTests
	{
		[Fact]
		public void Test_CompileDiagramFile()
		{
			// Arrange.
			var plantUml = new PlantUml(TaskScheduler.Default)
			{
				PlantUmlJar = new FileInfo(@"C:\Users\mhamilt\Documents\Visual Studio 2010\Projects\PlantUmlEditor\PlantUmlEditor\bin\Debug\Thirdparty\plantuml.jar"),
				GraphVizExecutable = new FileInfo(@"C:\Program Files (x86)\Graphviz2.26.3\bin\dot.exe")
			};

			var cts = new CancellationTokenSource();

			var inputFile = new FileInfo(@"C:\Users\mhamilt\Documents\PlantUmlEditor\samples\sample class.puml");

			// Act.
			Task<BitmapSource> compileTask = plantUml.CompileToImage(code, cts.Token);
			//cts.Cancel();
			compileTask.Wait();

			using (var filestream = new FileStream(inputFile.FullName.Replace(".puml", ".png"), FileMode.Create))
			{
				var encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(compileTask.Result));
				encoder.Save(filestream);
			}
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
}