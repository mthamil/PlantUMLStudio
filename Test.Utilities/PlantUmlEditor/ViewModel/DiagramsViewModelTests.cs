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
				DiagramLocation = testDiagramFile.Directory
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
				DiagramLocation = testDiagramFile.Directory
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
			var diagram = new DiagramViewModel(new Diagram { DiagramFilePath = testDiagramFile.FullName });
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
			var diagram = new DiagramViewModel(new Diagram { DiagramFilePath = testDiagramFile.FullName });
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

		[Fact]
		[Synchronous]
		public void Test_OpenDiagramEditorClosed()
		{
			// Arrange.
			var diagram = new DiagramViewModel(new Diagram { DiagramFilePath = testDiagramFile.FullName });
			var editor = new Mock<IDiagramEditor>();
			editor.SetupGet(e => e.DiagramViewModel).Returns(diagram);

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, null);
			diagrams.OpenDiagramCommand.Execute(diagram);

			// Act.
			editor.Raise(e => e.Closed += null, EventArgs.Empty);

			// Assert.
			Assert.Empty(diagrams.OpenDiagrams);
		}

		[Fact]
		[Synchronous]
		public void Test_AddNewDiagramCommand()
		{
			// Arrange.
			var editor = new Mock<IDiagramEditor>();

			diagramIO.Setup(dio => dio.SaveAsync(It.IsAny<Diagram>(), It.IsAny<bool>()))
				.Returns(Tasks.FromResult<object>(null));

			diagramIO.Setup(dio => dio.ReadDiagramsAsync(It.IsAny<DirectoryInfo>(), It.IsAny<IProgress<Tuple<int?, string>>>()))
				.Returns(Tasks.FromResult<IEnumerable<Diagram>>(new List<Diagram> { new Diagram { DiagramFilePath = testDiagramFile.FullName, Content = "New Diagram" } }));

			diagrams = new DiagramsViewModel(progress.Object, diagramIO.Object, d => editor.Object, d => new DiagramViewModel(d))
			{
				DiagramLocation = testDiagramFile.Directory,
				NewDiagramTemplate = "New Diagram"
			};

			// Act.
			diagrams.AddNewDiagramCommand.Execute(new Uri(testDiagramFile.FullName));

			// Assert.
			Assert.Single(diagrams.Diagrams);
			Assert.Equal(testDiagramFile.FullName, diagrams.Diagrams.Single().Diagram.DiagramFilePath);
			Assert.Equal("New Diagram", diagrams.Diagrams.Single().Diagram.Content);

			Assert.Single(diagrams.OpenDiagrams);
			Assert.Equal(diagrams.OpenDiagrams.Single(), diagrams.OpenDiagram);
			Assert.Equal(diagrams.Diagrams.Single(), diagrams.CurrentDiagram);

			diagramIO.Verify(dio => dio.SaveAsync(It.Is<Diagram>(d => d.Content == "New Diagram" && d.DiagramFilePath == testDiagramFile.FullName), false));
			diagramIO.Verify(dio => dio.ReadDiagramsAsync(It.Is<DirectoryInfo>(d => d.FullName == testDiagramFile.Directory.FullName), It.IsAny<IProgress<Tuple<int?, string>>>()), Times.AtLeastOnce());
		}

		private DiagramsViewModel diagrams;

		private readonly Mock<IProgressViewModel> progress = new Mock<IProgressViewModel>();
		private readonly Mock<IDiagramIOService> diagramIO = new Mock<IDiagramIOService>();

		private static readonly FileInfo testDiagramFile = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestDiagrams\class.puml"));
	}
}