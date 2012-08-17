using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.Core.InputOutput;
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

			progress.Setup(p => p.New(It.IsAny<bool>())).Returns(() => new Mock<IProgress<ProgressUpdate>>().Object);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_SuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int, int>>>()))
				.Returns(Tasks.FromResult<IEnumerable<Diagram>>(new List<Diagram>
				{
					new Diagram { Content = "Diagram 1"},
					new Diagram { Content = "Diagram 2" }
				}));

			var explorer = CreateExplorer();

			// Act.
			explorer.DiagramLocation = diagramLocation;
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Equal(2, explorer.PreviewDiagrams.Count);
			AssertThat.SequenceEqual(new [] { "Diagram 1", "Diagram 2" }, explorer.PreviewDiagrams.Select(d => d.Diagram.Content));
			Assert.Equal(diagramLocation.FullName, settings.Object.LastDiagramLocation.FullName);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_UnsuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int, int>>>()))
				.Returns(Tasks.FromException<IEnumerable<Diagram>, AggregateException>(new AggregateException()));

			var explorer = CreateExplorer();

			// Act.
			explorer.DiagramLocation = diagramLocation;
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Empty(explorer.PreviewDiagrams);
			Assert.Equal(diagramLocation.FullName, settings.Object.LastDiagramLocation.FullName);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_False()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int, int>>>()))
				.Returns(Tasks.FromResult(Enumerable.Empty<Diagram>()));

			var explorer = CreateExplorer();

			// Act.
			bool isValid = explorer.IsDiagramLocationValid;

			// Assert.
			Assert.False(isValid);
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.IsAny<DirectoryInfo>(), 
				It.IsAny<IProgress<Tuple<int, int>>>()), Times.Never());
			settings.VerifySet(s => s.LastDiagramLocation = It.IsAny<DirectoryInfo>(), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand()
		{
			// Arrange.
			string newDiagramFilePath = Path.Combine(diagramLocation.FullName, "new-diagram.puml");

			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int, int>>>()))
				.Returns(Tasks.FromResult<IEnumerable<Diagram>>(new List<Diagram> 
				{ 
					new Diagram 
					{ 
						File = new FileInfo(newDiagramFilePath), 
						Content = "New Diagram" 
					} 
				}));

			var explorer = CreateExplorer();
			explorer.DiagramLocation = diagramLocation;
			explorer.NewDiagramTemplate = "New Diagram";

			NewDiagramCreatedEventArgs newDiagramArgs = null;
			explorer.NewDiagramCreated += (o, e) => newDiagramArgs = e;

			// Act.
			explorer.AddNewDiagramCommand.Execute(new Uri(newDiagramFilePath));

			// Assert.
			Assert.Single(explorer.PreviewDiagrams);
			Assert.Equal(newDiagramFilePath, explorer.PreviewDiagrams.Single().Diagram.File.FullName);
			Assert.Equal("New Diagram", explorer.PreviewDiagrams.Single().Diagram.Content);

			Assert.NotNull(newDiagramArgs);
			Assert.Equal(explorer.PreviewDiagrams.Single(), newDiagramArgs.NewDiagramPreview);
			Assert.Equal(explorer.PreviewDiagrams.Single(), explorer.CurrentPreviewDiagram);

			diagramIO.Verify(dio => dio.SaveAsync(
				It.Is<Diagram>(d => d.Content == "New Diagram" && d.File.FullName == newDiagramFilePath), 
				false));

			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.Is<DirectoryInfo>(d => d.FullName == diagramLocation.FullName), 
				It.IsAny<IProgress<Tuple<int, int>>>()), Times.AtLeastOnce());
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand_FileNameWithoutExtension()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int, int>>>()))
				.Returns(Tasks.FromResult(Enumerable.Empty<Diagram>()));

			var explorer = CreateExplorer();
			explorer.DiagramLocation = diagramLocation;
			explorer.NewDiagramTemplate = "New Diagram";

			// Act.
			explorer.AddNewDiagramCommand.Execute(new Uri(diagramLocation.FullName + @"\test-diagram"));

			// Assert.
			diagramIO.Verify(io => io.SaveAsync(
				It.Is<Diagram>(d => d.File.FullName == diagramLocation.FullName + @"\test-diagram.puml"), 
				It.IsAny<bool>()));
		}

		private DiagramExplorerViewModel CreateExplorer()
		{
			return new DiagramExplorerViewModel(progress.Object, diagramIO.Object, d => new PreviewDiagramViewModel(d), settings.Object);
		}

		private readonly Mock<IProgressRegistration> progress = new Mock<IProgressRegistration>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();
		private readonly Mock<ISettings> settings = new Mock<ISettings>();

		private static readonly DirectoryInfo diagramLocation = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
	}
}