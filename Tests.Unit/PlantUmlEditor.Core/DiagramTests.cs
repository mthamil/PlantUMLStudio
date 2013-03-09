using PlantUmlEditor.Core;
using PlantUmlEditor.Core.Imaging;
using Xunit;

namespace Tests.Unit.PlantUmlEditor.Core
{
	public class DiagramTests
	{
		[Fact]
		public void Test_PNG_ImageFormat_Detection()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act.
			diagram.ImageFilePath = "image.png";

			// Assert.
			Assert.Equal(ImageFormat.PNG, diagram.ImageFormat);
		}

		[Fact]
		public void Test_SVG_ImageFormat_Detection()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act.
			diagram.ImageFilePath = "image.svg";

			// Assert.
			Assert.Equal(ImageFormat.SVG, diagram.ImageFormat);
		}

		[Fact]
		public void Test_ImageFormat_Default()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act.
			diagram.ImageFilePath = "image";

			// Assert.
			Assert.Equal(ImageFormat.PNG, diagram.ImageFormat);
		}

		[Fact]
		public void Test_Content_RaisesPropertyChange()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act/Assert.
			AssertThat.PropertyChanged(diagram, p => p.Content, () => diagram.Content = "blargh");
		}

		[Fact]
		public void Test_ImageFilePath_RaisesPropertyChange()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act/Assert.
			AssertThat.PropertyChanged(diagram, p => p.ImageFilePath, () => diagram.ImageFilePath = "image.png");
		}
	}
}