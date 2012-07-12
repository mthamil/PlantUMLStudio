using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using PlantUmlEditor.Model;
using PlantUmlEditor.ViewModel;
using Utilities.Concurrency;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class PreviewDiagramsViewModelTests
	{
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

			previews = new PreviewDiagramsViewModel(progress.Object, diagramIO.Object, d => new PreviewDiagramViewModel(d))
			{
				DiagramLocation = testDiagramFile.Directory
			};

			// Act.
			bool isValid = previews.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Equal(2, previews.PreviewDiagrams.Count);
			AssertThat.SequenceEqual(new [] { "Diagram 1", "Diagram 2" }, previews.PreviewDiagrams.Select(d => d.Diagram.Content));
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_UnsuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromException<IEnumerable<Diagram>, AggregateException>(new AggregateException()));

			previews = new PreviewDiagramsViewModel(progress.Object, diagramIO.Object, d => new PreviewDiagramViewModel(d))
			{
				DiagramLocation = testDiagramFile.Directory
			};

			// Act.
			bool isValid = previews.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Empty(previews.PreviewDiagrams);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_False()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult(Enumerable.Empty<Diagram>()));

			previews = new PreviewDiagramsViewModel(progress.Object, diagramIO.Object, null);

			// Act.
			bool isValid = previews.IsDiagramLocationValid;

			// Assert.
			Assert.False(isValid);
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.IsAny<DirectoryInfo>(), 
				It.IsAny<IProgress<Tuple<int?, string>>>()), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromResult<object>(null));

			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult<IEnumerable<Diagram>>(new List<Diagram> { new Diagram { File = testDiagramFile, Content = "New Diagram" } }));

			previews = new PreviewDiagramsViewModel(progress.Object, diagramIO.Object, d => new PreviewDiagramViewModel(d))
			{
				DiagramLocation = testDiagramFile.Directory,
				NewDiagramTemplate = "New Diagram"
			};

			NewDiagramCreatedEventArgs newDiagramArgs = null;
			previews.NewDiagramCreated += (o, e) => newDiagramArgs = e;

			// Act.
			previews.AddNewDiagramCommand.Execute(new Uri(testDiagramFile.FullName));

			// Assert.
			Assert.Single(previews.PreviewDiagrams);
			Assert.Equal(testDiagramFile.FullName, previews.PreviewDiagrams.Single().Diagram.File.FullName);
			Assert.Equal("New Diagram", previews.PreviewDiagrams.Single().Diagram.Content);

			Assert.NotNull(newDiagramArgs);
			Assert.Equal(previews.PreviewDiagrams.Single(), newDiagramArgs.NewDiagramPreview);
			Assert.Equal(previews.PreviewDiagrams.Single(), previews.CurrentPreviewDiagram);

			diagramIO.Verify(dio => dio.SaveAsync(
				It.Is<Diagram>(d => d.Content == "New Diagram" && d.File.FullName == testDiagramFile.FullName), 
				false));

			diagramIO.Verify(dio => dio.ReadDiagramsAsync(
				It.Is<DirectoryInfo>(d => d.FullName == testDiagramFile.Directory.FullName), 
				It.IsAny<IProgress<Tuple<int?, string>>>()), Times.AtLeastOnce());
		}

		private PreviewDiagramsViewModel previews;

		private readonly Mock<IProgressViewModel> progress = new Mock<IProgressViewModel>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();

		private static readonly FileInfo testDiagramFile = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestDiagrams\class.puml"));
	}
}