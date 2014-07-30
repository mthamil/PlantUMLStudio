using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Moq;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.Imaging;
using Tests.Integration.Properties;
using Utilities.Chronology;
using Xunit;

namespace Tests.Integration.PlantUmlStudio.Core
{
	public class PlantUmlTests
	{
		[Fact]
		public async Task Test_CompileDiagramFileAsync()
		{
			// Arrange.
			var renderers = new Mock<IIndex<ImageFormat, IDiagramRenderer>>();
			renderers.Setup(r => r[ImageFormat.PNG]).Returns(new BitmapRenderer());

			var plantUml = new PlantUml(new Mock<IClock>().Object, renderers.Object, new HttpClient())
			{
				PlantUmlJar = new FileInfo(Settings.Default.PlantUmlLocation),
				GraphVizExecutable = new FileInfo(Settings.Default.GraphVizLocation)
			};

			// Act.
			var image = await plantUml.CompileToImageAsync(code, ImageFormat.PNG, CancellationToken.None);

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