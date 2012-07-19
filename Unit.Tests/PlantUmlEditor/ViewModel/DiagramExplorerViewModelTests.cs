using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Model;
using PlantUmlEditor.ViewModel;
using Utilities.Concurrency;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class DiagramExplorerViewModelTests
	{
		public DiagramExplorerViewModelTests()
		{
			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromSuccess());

			settings.SetupProperty(s => s.LastDiagramLocation);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_SuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult<IEnumerable<Diagram>>(new List<Diagram>
				{
					new Diagram { Content = "Diagram 1"},
					new Diagram { Content = "Diagram 2" }
				}));

			var explorer = CreateExplorer();

			// Act.
			explorer.DiagramLocation = testDiagramFile.Directory;
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Equal(2, explorer.PreviewDiagrams.Count);
			AssertThat.SequenceEqual(new [] { "Diagram 1", "Diagram 2" }, explorer.PreviewDiagrams.Select(d => d.Diagram.Content));
			Assert.Equal(testDiagramFile.Directory.FullName, settings.Object.LastDiagramLocation.FullName);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_UnsuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromException<IEnumerable<Diagram>, AggregateException>(new AggregateException()));

			var explorer = CreateExplorer();

			// Act.
			explorer.DiagramLocation = testDiagramFile.Directory;
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Empty(explorer.PreviewDiagrams);
			Assert.Equal(testDiagramFile.Directory.FullName, settings.Object.LastDiagramLocation.FullName);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_False()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult(Enumerable.Empty<Diagram>()));

			var explorer = CreateExplorer();

			// Act.
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.False(isValid);
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.IsAny<DirectoryInfo>(), 
				It.IsAny<IProgress<Tuple<int?, string>>>()), Times.Never());
			settings.VerifySet(s => s.LastDiagramLocation = It.IsAny<DirectoryInfo>(), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult<IEnumerable<Diagram>>(new List<Diagram> { new Diagram { File = testDiagramFile, Content = "New Diagram" } }));

			var explorer = CreateExplorer();
			explorer.DiagramLocation = testDiagramFile.Directory;
			explorer.NewDiagramTemplate = "New Diagram";

			NewDiagramCreatedEventArgs newDiagramArgs = null;
			explorer.NewDiagramCreated += (o, e) => newDiagramArgs = e;

			// Act.
			explorer.AddNewDiagramCommand.Execute(new Uri(testDiagramFile.FullName));

			// Assert.
			Assert.Single(explorer.PreviewDiagrams);
			Assert.Equal(testDiagramFile.FullName, explorer.PreviewDiagrams.Single().Diagram.File.FullName);
			Assert.Equal("New Diagram", explorer.PreviewDiagrams.Single().Diagram.Content);

			Assert.NotNull(newDiagramArgs);
			Assert.Equal(explorer.PreviewDiagrams.Single(), newDiagramArgs.NewDiagramPreview);
			Assert.Equal(explorer.PreviewDiagrams.Single(), explorer.CurrentPreviewDiagram);

			diagramIO.Verify(dio => dio.SaveAsync(
				It.Is<Diagram>(d => d.Content == "New Diagram" && d.File.FullName == testDiagramFile.FullName), 
				false));

			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.Is<DirectoryInfo>(d => d.FullName == testDiagramFile.Directory.FullName), 
				It.IsAny<IProgress<Tuple<int?, string>>>()), Times.AtLeastOnce());
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand_FileNameWithoutExtension()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult(Enumerable.Empty<Diagram>()));

			var explorer = CreateExplorer();
			explorer.DiagramLocation = testDiagramFile.Directory;
			explorer.NewDiagramTemplate = "New Diagram";

			// Act.
			explorer.AddNewDiagramCommand.Execute(new Uri(testDiagramFile.Directory.FullName + @"\test-diagram"));

			// Assert.
			diagramIO.Verify(io => io.SaveAsync(
				It.Is<Diagram>(d => d.File.FullName == testDiagramFile.Directory.FullName + @"\test-diagram.puml"), 
				It.IsAny<bool>()));
		}

		private DiagramExplorerViewModel CreateExplorer()
		{
			return new DiagramExplorerViewModel(progress.Object, diagramIO.Object, d => new PreviewDiagramViewModel(d), settings.Object);
		}

		private readonly Mock<IProgressViewModel> progress = new Mock<IProgressViewModel>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();
		private readonly Mock<ISettings> settings = new Mock<ISettings>();

		private static readonly FileInfo testDiagramFile = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestDiagrams\class.puml"));
	}
}