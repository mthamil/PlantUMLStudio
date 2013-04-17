using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.ViewModel;
using Utilities.InputOutput;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PlantUmlEditor.Configuration
{
	public class SettingsPropagatorTests
	{
		public SettingsPropagatorTests()
		{
			settings.SetupGet(s => s.RecentFiles).Returns(new List<FileInfo>());
			settings.SetupGet(s => s.OpenFiles).Returns(new List<FileInfo>());
			propagator = new SettingsPropagator(settings.Object, diagramManager.Object);		
		}

		[Fact]
		public void Test_AutoSaveEnabled_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = Mocks.Of<IDiagramEditor>()
			                   .Where(e => e.AutoSave == false)
			                   .Take(2).ToList();

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
			              .Returns(editors);

			settings.SetupProperty(s => s.AutoSaveEnabled, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("AutoSaveEnabled"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.AutoSave);
		}

		[Fact]
		public void Test_AutoSaveInterval_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = Mocks.Of<IDiagramEditor>()
							   .Where(e => e.AutoSaveInterval == TimeSpan.FromSeconds(15))
							   .Take(2).ToList();

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
			              .Returns(editors);

			settings.SetupProperty(s => s.AutoSaveInterval, TimeSpan.FromSeconds(30));

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("AutoSaveInterval"));

			// Assert.
			foreach (var editor in editors)
				Assert.Equal(TimeSpan.FromSeconds(30), editor.AutoSaveInterval);
		}

		[Fact]
		public void Test_HighlightCurrentLine_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = Mocks.Of<IDiagramEditor>()
			                   .Where(e => e.CodeEditor.Options.HighlightCurrentLine == false)
			                   .Take(2).ToList();

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
			              .Returns(editors);

			settings.SetupProperty(s => s.HighlightCurrentLine, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("HighlightCurrentLine"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.CodeEditor.Options.HighlightCurrentLine);
		}

		[Fact]
		public void Test_ShowLineNumbers_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = Mocks.Of<IDiagramEditor>()
							   .Where(e => e.CodeEditor.Options.ShowLineNumbers == false)
							   .Take(2).ToList();

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
						  .Returns(editors);

			settings.SetupProperty(s => s.ShowLineNumbers, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("ShowLineNumbers"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.CodeEditor.Options.ShowLineNumbers);
		}

		[Fact]
		public void Test_EnableVirtualSpace_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = Mocks.Of<IDiagramEditor>()
							   .Where(e => e.CodeEditor.Options.EnableVirtualSpace == false)
							   .Take(2).ToList();

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
						  .Returns(editors);

			settings.SetupProperty(s => s.EnableVirtualSpace, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("EnableVirtualSpace"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.CodeEditor.Options.EnableVirtualSpace);
		}

		[Fact]
		public void Test_EnableWordWrap_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = Mocks.Of<IDiagramEditor>()
							   .Where(e => e.CodeEditor.Options.EnableWordWrap == false)
							   .Take(2).ToList();

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
						  .Returns(editors);

			settings.SetupProperty(s => s.EnableWordWrap, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("EnableWordWrap"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.CodeEditor.Options.EnableWordWrap);
		}

		[Fact]
		public void Test_EmptySelectionCopiesEntireLine_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = Mocks.Of<IDiagramEditor>()
							   .Where(e => e.CodeEditor.Options.EmptySelectionCopiesEntireLine == false)
							   .Take(2).ToList();

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
						  .Returns(editors);

			settings.SetupProperty(s => s.EmptySelectionCopiesEntireLine, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("EmptySelectionCopiesEntireLine"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.CodeEditor.Options.EmptySelectionCopiesEntireLine);
		}

		[Fact]
		public void Test_AllowScrollingBelowContent_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = Mocks.Of<IDiagramEditor>()
							   .Where(e => e.CodeEditor.Options.AllowScrollingBelowContent == false)
							   .Take(2).ToList();

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
						  .Returns(editors);

			settings.SetupProperty(s => s.AllowScrollingBelowContent, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("AllowScrollingBelowContent"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.CodeEditor.Options.AllowScrollingBelowContent);
		}

		[Fact]
		public void Test_ClosedDiagram_AddedToRecentFiles()
		{
			// Arrange.
			var diagram = new Diagram { File = new FileInfo(@"C:\file") };

			// Act.
			diagramManager.Raise(dm => dm.DiagramClosed += null, new DiagramClosedEventArgs(diagram));

			// Assert.
			Assert.Single(settings.Object.RecentFiles);
			Assert.Equal(settings.Object.RecentFiles.Single(), diagram.File, FileInfoPathEqualityComparer.Instance);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void Test_OpenedDiagram_AddedToOpenFiles_When_OpenFilesRemembered(bool rememberOpenFiles)
		{
			// Arrange.
			var diagram = new Diagram { File = new FileInfo(@"C:\file") };

			settings.SetupGet(s => s.RememberOpenFiles).Returns(rememberOpenFiles);

			// Act.
			diagramManager.Raise(dm => dm.DiagramOpened += null, new DiagramOpenedEventArgs(diagram));

			// Assert.
			Assert.Single(settings.Object.OpenFiles);
			Assert.Equal(settings.Object.OpenFiles.Single(), diagram.File, FileInfoPathEqualityComparer.Instance);
		}

		[Fact]
		public void Test_OpenedDiagram_NotAddedToOpenFiles_When_FileAlreadyExists()
		{
			// Arrange.
			var diagram = new Diagram { File = new FileInfo(@"C:\file") };

			settings.Object.OpenFiles.Add(new FileInfo(@"C:\file"));

			// Act.
			diagramManager.Raise(dm => dm.DiagramOpened += null, new DiagramOpenedEventArgs(diagram));

			// Assert.
			Assert.Single(settings.Object.OpenFiles);
			Assert.Equal(settings.Object.OpenFiles.Single(), diagram.File, FileInfoPathEqualityComparer.Instance);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void Test_ClosedDiagram_RemovedFromOpenFiles(bool rememberOpenFiles)
		{
			// Arrange.
			var diagram = new Diagram { File = new FileInfo(@"C:\file") };

			settings.SetupGet(s => s.RememberOpenFiles).Returns(rememberOpenFiles);
			settings.Object.OpenFiles.Add(new FileInfo(@"C:\file"));

			// Act.
			diagramManager.Raise(dm => dm.DiagramClosed += null, new DiagramClosedEventArgs(diagram));

			// Assert.
			Assert.Empty(settings.Object.OpenFiles);
		}

		[Fact]
		public void Test_Multiple_Diagrams_Opened()
		{
			// Arrange.
			settings.SetupGet(s => s.RememberOpenFiles).Returns(true);

			var files = new[] { new FileInfo("fileC"), new FileInfo("fileA"), new FileInfo("fileB") };

			// Act.
			foreach (var file in files)
			{
				diagramManager.Raise(dm => dm.DiagramOpened += null,
					new DiagramOpenedEventArgs(new Diagram { File = file }));
			}

			// Assert.
			AssertThat.SequenceEqual(settings.Object.OpenFiles, files, FileInfoPathEqualityComparer.Instance);
		}

		[Fact]
		public void Test_DiagramManager_Closing_Saves_Settings()
		{
			// Act.
			diagramManager.Raise(dm => dm.Closing += null, EventArgs.Empty);

			// Assert.
			settings.Verify(s => s.Save());
		}

		private readonly SettingsPropagator propagator;

		private readonly Mock<ISettings> settings = new Mock<ISettings> { DefaultValue = DefaultValue.Empty };
		private readonly Mock<IDiagramManager> diagramManager = new Mock<IDiagramManager> { DefaultValue = DefaultValue.Empty };
	}
}