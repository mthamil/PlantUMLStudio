using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.ViewModel;
using Xunit;

namespace Tests.Unit.PlantUmlEditor.Configuration
{
	public class SettingsPropagatorTests
	{
		public SettingsPropagatorTests()
		{
			propagator = new SettingsPropagator(settings.Object, diagramManager.Object);
		}

		[Fact]
		public void Test_AutoSaveEnabled_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = new List<Mock<IDiagramEditor>>();
			for (int i = 0; i < 2; i++)
			{
				var editor = new Mock<IDiagramEditor> { DefaultValue = DefaultValue.Empty };
				editor.SetupProperty(e => e.AutoSave, false);
				editors.Add(editor);
			}

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
			              .Returns(editors.Select(e => e.Object).ToList());

			settings.SetupProperty(s => s.AutoSaveEnabled, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("AutoSaveEnabled"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.Object.AutoSave);
		}

		[Fact]
		public void Test_AutoSaveInterval_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = new List<Mock<IDiagramEditor>>();
			for (int i = 0; i < 2; i++)
			{
				var editor = new Mock<IDiagramEditor> { DefaultValue = DefaultValue.Empty };
				editor.SetupProperty(e => e.AutoSaveInterval, TimeSpan.FromSeconds(15));
				editors.Add(editor);
			}

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
			              .Returns(editors.Select(e => e.Object).ToList());

			settings.SetupProperty(s => s.AutoSaveInterval, TimeSpan.FromSeconds(30));

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("AutoSaveInterval"));

			// Assert.
			foreach (var editor in editors)
				Assert.Equal(TimeSpan.FromSeconds(30), editor.Object.AutoSaveInterval);
		}

		[Fact]
		public void Test_HighlightCurrentLine_Changes_UpdateDiagramEditors()
		{
			// Arrange.
			var editors = new List<Mock<IDiagramEditor>>();
			for (int i = 0; i < 2; i++)
			{
				var editor = new Mock<IDiagramEditor> { DefaultValue = DefaultValue.Empty };
				var codeEditor = new Mock<ICodeEditor> { DefaultValue = DefaultValue.Empty };
				editor.SetupGet(e => e.CodeEditor).Returns(codeEditor.Object);
				codeEditor.SetupProperty(e => e.HighlightCurrentLine, false);
				editors.Add(editor);
			}

			diagramManager.SetupGet(dm => dm.OpenDiagrams)
			              .Returns(editors.Select(e => e.Object).ToList());

			settings.SetupProperty(s => s.HighlightCurrentLine, true);

			// Act.
			settings.Raise(s => s.PropertyChanged += null, new PropertyChangedEventArgs("HighlightCurrentLine"));

			// Assert.
			foreach (var editor in editors)
				Assert.True(editor.Object.CodeEditor.HighlightCurrentLine);
		}

		[Fact]
		public void Test_ClosedDiagram_AddedToRecentFiles()
		{
			// Arrange.
			var diagram = new Diagram { File = new FileInfo(@"C:\file") };

			settings.SetupGet(s => s.RecentFiles)
			        .Returns(new List<FileInfo>());

			// Act.
			diagramManager.Raise(dm => dm.DiagramClosed += null, new DiagramClosedEventArgs(diagram));

			// Assert.
			Assert.Single(settings.Object.RecentFiles, diagram.File);
		}

		private readonly SettingsPropagator propagator;

		private readonly Mock<ISettings> settings = new Mock<ISettings> { DefaultValue = DefaultValue.Empty };
		private readonly Mock<IDiagramManager> diagramManager = new Mock<IDiagramManager> { DefaultValue = DefaultValue.Empty };
	}
}