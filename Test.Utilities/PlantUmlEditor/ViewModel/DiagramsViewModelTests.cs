using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using PlantUmlEditor.Model;
using PlantUmlEditor.ViewModel;
using Utilities.Concurrency;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class DiagramsViewModelTests
	{
		public DiagramsViewModelTests()
		{
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

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, null, d => new DiagramViewModel(d))
			{
				DiagramLocation = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
			};

			// Act.
			bool isValid = diagrams.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Equal(2, diagrams.Diagrams.Count);
			AssertThat.SequenceEqual(new [] { "Diagram 1", "Diagram 2" }, diagrams.Diagrams.Select(d => d.Diagram.Content));
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_UnsuccessfulLoad()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromException<IEnumerable<Diagram>, AggregateException>(new AggregateException()));

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, null, d => new DiagramViewModel(d))
			{
				DiagramLocation = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
			};

			// Act.
			bool isValid = diagrams.IsDiagramLocationValid;

			// Assert.
			Assert.True(isValid);
			Assert.Empty(diagrams.Diagrams);
		}

		[Fact]
		[Synchronous]
		public void Test_IsDiagramLocationValid_False()
		{
			// Arrange.
			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult(Enumerable.Empty<Diagram>()));

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, null, null);

			// Act.
			bool isValid = diagrams.IsDiagramLocationValid;

			// Assert.
			Assert.False(isValid);
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()), Times.Never());
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramCommand()
		{
			// Arrange.
			var diagram = new DiagramViewModel(new Diagram { DiagramFilePath = "DiagramPath" });
			var editor = new Mock<IDiagramEditor>();

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, null);

			// Act.
			diagrams.OpenDiagramCommand.Execute(diagram);

			// Assert.
			Assert.Single(diagrams.OpenDiagrams);
			Assert.Equal(editor.Object, diagrams.OpenDiagram);
		}

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramCommand_DiagramAlreadyOpenedOnce()
		{
			// Arrange.
			var diagram = new DiagramViewModel(new Diagram { DiagramFilePath = "DiagramPath" });
			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.DiagramViewModel).Returns(diagram);

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, null, null);
			diagrams.OpenDiagrams.Add(editor.Object);

			// Act.
			diagrams.OpenDiagramCommand.Execute(diagram);

			// Assert.
			Assert.Single(diagrams.OpenDiagrams);
			Assert.Equal(editor.Object, diagrams.OpenDiagram);
		}

		private DiagramsViewModel diagrams;

		private readonly Mock<IProgressViewModel> progress = new Mock<IProgressViewModel>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();
	}
}