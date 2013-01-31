using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.ViewModel;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.Configuration
{
	public class SettingsPropagatorTests
	{
		public SettingsPropagatorTests()
		{
			propagator = new SettingsPropagator(settings.Object, new Lazy<IDiagramManager>(() => diagramManager.Object));
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

		private readonly SettingsPropagator propagator;

		private readonly Mock<ISettings> settings = new Mock<ISettings>();
		private readonly Mock<IDiagramManager> diagramManager = new Mock<IDiagramManager> { DefaultValue = DefaultValue.Empty };
	}
}