using PlantUmlStudio.Core;
using PlantUmlStudio.ViewModel;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.ViewModel
{
	public class PreviewDiagramViewModelTests
	{
		[Theory]
		[InlineData("", "")]
		[InlineData("iuhikcvjnkcvjn sd", "iuhikcvjnkcvjn sd")]
		[InlineData("@startuml", "\n@startuml\n")]
		[InlineData("@startuml\njsdjksdjks", "\n@startuml\njsdjksdjks\n")]
		[InlineData("@startuml\njsdjksdjks\nsvdsvrvdsdfv\ndvervbreer\nsdvsvsvsv", "\n@startuml\njsdjksdjks\nsvdsvrvdsdfv\ndvervbreer\nsdvsvsvsv\n")]
		[InlineData("@startuml\njsdjksdjks\nsvdsvrvdsdfv\ndvervbreer\nsdvsvsvsv", "\n@startuml\njsdjksdjks\nsvdsvrvdsdfv\ndvervbreer\nsdvsvsvsv\ndfdfvdfdfdf\n")]
		[InlineData("@startuml\njsdjksdjks\nsvdsvrvdsdfv\ndvervbreer\nsdvsvsvsv", "\n@startuml\njsdjksdjks\nsvdsvrvdsdfv\ndvervbreer\nsdvsvsvsv\ndfdfvdfdfdf\nvddvvrvrvevrv\n")]
		public void Test_CodePreview(string expected, string input)
		{
			// Arrange.
			var diagram = new Diagram
			{
				Content = input
			};
			var preview = new PreviewDiagramViewModel(diagram);

			// Act.
			var actual = preview.CodePreview;

			// Assert.
			Assert.Equal(expected, actual);
		}
	}
}