using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Properties;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PlantUmlEditor.Configuration
{
	public class DotNetSettingsTests
	{
		[Fact]
		public void Test_Initialization()
		{
			// Arrange.
			settings.RememberOpenFiles = true;
			settings.MaximumRecentFiles = 15;
			settings.AutoSaveEnabled = true;
			settings.AutoSaveInterval = TimeSpan.FromSeconds(15);
			settings.HighlightCurrentLine = true;

			// Act.
			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Assert.
			Assert.Equal(settings.GraphVizLocation, appSettings.GraphVizExecutable.FullName);
			Assert.Equal(Path.GetFullPath(settings.PlantUmlLocation), appSettings.PlantUmlJar.FullName);
			Assert.Equal(Path.GetFullPath(settings.PlantUmlHighlightingDefinition), appSettings.PlantUmlHighlightingDefinition.FullName);

			Assert.Equal(settings.GraphVizLocalVersionPattern, appSettings.GraphVizLocalVersionPattern.ToString());
			Assert.Equal(settings.PlantUmlLocalVersionPattern, appSettings.PlantUmlLocalVersionPattern.ToString());
			Assert.Equal(settings.PlantUmlRemoteVersionPattern, appSettings.PlantUmlRemoteVersionPattern.ToString());

			Assert.Equal(settings.PlantUmlVersionSource, appSettings.PlantUmlVersionSource);
			Assert.Equal(settings.DownloadUrl, appSettings.PlantUmlDownloadLocation);

			Assert.Equal(settings.PlantUmlFileExtension, appSettings.DiagramFileExtension);

			Assert.Equal(settings.RememberOpenFiles, appSettings.RememberOpenFiles);
			Assert.Equal(settings.MaximumRecentFiles, appSettings.MaximumRecentFiles);
			Assert.Equal(settings.AutoSaveEnabled, appSettings.AutoSaveEnabled);
			Assert.Equal(settings.AutoSaveInterval, appSettings.AutoSaveInterval);

			Assert.Equal(settings.HighlightCurrentLine, appSettings.HighlightCurrentLine);
		}

		[Fact]
		public void Test_Save()
		{
			// Arrange.
			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"))
			{
				RememberOpenFiles = true,
				OpenFiles = new List<FileInfo> { new FileInfo(@"C:\openFile1"), new FileInfo(@"C:\openFile2") },
				MaximumRecentFiles = 20,
				AutoSaveEnabled = true,
				AutoSaveInterval = TimeSpan.FromSeconds(15),
				HighlightCurrentLine = false
			};

			appSettings.RecentFiles.Add(new FileInfo(@"C:\recentFile1"));
			appSettings.RecentFiles.Add(new FileInfo(@"C:\recentFile2"));

			// Act.
			appSettings.Save();

			// Assert.
			Assert.Equal(true, settings.RememberOpenFiles);
			AssertThat.SequenceEqual(settings.OpenFiles.Cast<string>(), new[] { @"C:\openFile1", @"C:\openFile2" });
			Assert.Equal(20, settings.MaximumRecentFiles);
			AssertThat.SequenceEqual(settings.RecentFiles.Cast<string>(), new[] { @"C:\recentFile2", @"C:\recentFile1" });
			Assert.Equal(true, settings.AutoSaveEnabled);
			Assert.Equal(TimeSpan.FromSeconds(15), settings.AutoSaveInterval);
			Assert.Equal(false, settings.HighlightCurrentLine);
		}

		[Theory]
		[InlineData(@"C:\", "")]
		[InlineData(@"C:\", null)]
		[InlineData(@"C:\Diagrams", @"C:\Diagrams")]
		public void Test_LastDiagramLocation_Initialization(string expected, string lastPath)
		{
			// Arrange.
			settings.LastPath = lastPath;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act.
			var actual = appSettings.LastDiagramLocation.FullName;

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Test_OpenFiles_Initialization_WhenNull()
		{
			// Arrange.
			settings.OpenFiles = null;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act.
			var actual = appSettings.OpenFiles;

			// Assert.
			Assert.Empty(actual);
		}

		[Fact]
		public void Test_OpenFiles_Initialization_WhenEmpty()
		{
			// Arrange.
			settings.OpenFiles = new StringCollection();

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act.
			var actual = appSettings.OpenFiles;

			// Assert.
			Assert.Empty(actual);
		}

		[Fact]
		public void Test_OpenFiles_Initialization_WhenPopulated()
		{
			// Arrange.
			settings.OpenFiles = new StringCollection();
			settings.OpenFiles.AddRange(new [] { @"C:\file1", @"C:\file2" });

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act.
			var actual = appSettings.OpenFiles;

			// Assert.
			Assert.NotEmpty(actual);
			Assert.Equal(2, actual.Count());
			AssertThat.SequenceEqual(actual.Select(f => f.FullName), new[] { @"C:\file1", @"C:\file2" });
		}

		[Fact]
		public void Test_RecentFiles_Initialization_WhenNull()
		{
			// Arrange.
			settings.RecentFiles = null;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act.
			var actual = appSettings.RecentFiles;

			// Assert.
			Assert.Empty(actual);
		}

		[Fact]
		public void Test_RecentFiles_Initialization_WhenEmpty()
		{
			// Arrange.
			settings.RecentFiles = new StringCollection();

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act.
			var actual = appSettings.RecentFiles;

			// Assert.
			Assert.Empty(actual);
		}

		[Fact]
		public void Test_RecentFiles_Initialization_WhenPopulated()
		{
			// Arrange.
			settings.RecentFiles = new StringCollection();
			settings.RecentFiles.AddRange(new[] { @"C:\file1", @"C:\file2" });

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act.
			var actual = appSettings.RecentFiles;

			// Assert.
			Assert.NotEmpty(actual);
			Assert.Equal(2, actual.Count());
			AssertThat.SequenceEqual(actual.Select(f => f.FullName), new[] { @"C:\file1", @"C:\file2" });
		}

		[Fact]
		public void Test_LastDiagramLocation_Changes()
		{
			// Arrange.
			settings.LastPath = @"C:\Initial";

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings, 
				s => s.LastDiagramLocation, 
				() => appSettings.LastDiagramLocation = new DirectoryInfo(@"C:\New"));

			Assert.Equal(@"C:\New", appSettings.LastDiagramLocation.FullName);
		}

		[Fact]
		public void Test_RememberOpenFiles_Changes()
		{
			// Arrange.
			settings.RememberOpenFiles = true;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.RememberOpenFiles,
				() => appSettings.RememberOpenFiles = false);

			Assert.False(appSettings.RememberOpenFiles);
		}

		[Fact]
		public void Test_OpenFiles_Changes()
		{
			// Arrange.
			settings.OpenFiles = new StringCollection { @"C:\file1" };

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.OpenFiles,
				() => appSettings.OpenFiles = new [] { new FileInfo(@"C:\file1"), new FileInfo(@"C:\file2") });

			AssertThat.SequenceEqual(new[] { @"C:\file1", @"C:\file2" }, appSettings.OpenFiles.Select(f => f.FullName));
		}

		[Fact]
		public void Test_MaximumRecentFiles_Changes()
		{
			// Arrange.
			settings.MaximumRecentFiles = 5;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.MaximumRecentFiles,
				() => appSettings.MaximumRecentFiles = 15);

			Assert.Equal(15, appSettings.MaximumRecentFiles);
		}

		[Fact]
		public void Test_AutoSaveEnabled_Changes()
		{
			// Arrange.
			settings.AutoSaveEnabled = false;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.AutoSaveEnabled,
				() => appSettings.AutoSaveEnabled = true);

			Assert.True(appSettings.AutoSaveEnabled);
		}

		[Fact]
		public void Test_AutoSaveInterval_Changes()
		{
			// Arrange.
			settings.AutoSaveInterval = TimeSpan.FromSeconds(0);

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.AutoSaveInterval,
				() => appSettings.AutoSaveInterval = TimeSpan.FromSeconds(30));

			Assert.Equal(TimeSpan.FromSeconds(30), appSettings.AutoSaveInterval);
		}

		[Fact]
		public void Test_HighlightCurrentLine_Changes()
		{
			// Arrange.
			settings.HighlightCurrentLine = false;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.HighlightCurrentLine,
				() => appSettings.HighlightCurrentLine = true);

			Assert.True(appSettings.HighlightCurrentLine);
		}

		private readonly Settings settings = new Settings
		{
			GraphVizLocation = @"C:\dot.exe" // doesn't have a default value
		};
	}
}