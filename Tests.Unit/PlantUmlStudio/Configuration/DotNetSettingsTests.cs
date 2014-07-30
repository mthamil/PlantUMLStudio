using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using PlantUmlStudio.Configuration;
using PlantUmlStudio.Properties;
using Utilities.Collections;
using Utilities.InputOutput;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PlantUmlStudio.Configuration
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
			settings.ShowLineNumbers = true;
			settings.EnableVirtualSpace = true;
			settings.EnableWordWrap = true;
			settings.EmptySelectionCopiesEntireLine = true;
			settings.AllowScrollingBelowContent = true;

			// Act.
			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Assert.
			Assert.Equal(settings.GraphVizLocation, appSettings.GraphVizExecutable.FullName);
			Assert.Equal(Path.GetFullPath(settings.PlantUmlLocation), appSettings.PlantUmlJar.FullName);
			Assert.Equal(Path.GetFullPath(settings.PlantUmlHighlightingDefinition), appSettings.PlantUmlHighlightingDefinition.FullName);

			Assert.Equal(settings.GraphVizLocalVersionPattern, appSettings.GraphVizLocalVersionPattern.ToString());
            Assert.Equal(settings.GraphVizRemoteVersionPattern, appSettings.GraphVizRemoteVersionPattern.ToString());

            Assert.Equal(settings.GraphVizVersionSource, appSettings.GraphVizVersionSource);
            Assert.Equal(settings.GraphVizDownloadLocation, appSettings.GraphVizDownloadLocation);

			Assert.Equal(settings.PlantUmlLocalVersionPattern, appSettings.PlantUmlLocalVersionPattern.ToString());
			Assert.Equal(settings.PlantUmlRemoteVersionPattern, appSettings.PlantUmlRemoteVersionPattern.ToString());

			Assert.Equal(settings.PlantUmlVersionSource, appSettings.PlantUmlVersionSource);
			Assert.Equal(settings.PlantUmlDownloadLocation, appSettings.PlantUmlDownloadLocation);

			Assert.Equal(settings.PlantUmlFileExtension, appSettings.DiagramFileExtension);

			Assert.Equal(settings.RememberOpenFiles, appSettings.RememberOpenFiles);
			Assert.Equal(settings.MaximumRecentFiles, appSettings.MaximumRecentFiles);
			Assert.Equal(settings.AutoSaveEnabled, appSettings.AutoSaveEnabled);
			Assert.Equal(settings.AutoSaveInterval, appSettings.AutoSaveInterval);

			Assert.Equal(settings.HighlightCurrentLine, appSettings.HighlightCurrentLine);
			Assert.Equal(settings.ShowLineNumbers, appSettings.ShowLineNumbers);
			Assert.Equal(settings.EnableVirtualSpace, appSettings.EnableVirtualSpace);
			Assert.Equal(settings.EnableWordWrap, appSettings.EnableWordWrap);
			Assert.Equal(settings.EmptySelectionCopiesEntireLine, appSettings.EmptySelectionCopiesEntireLine);
			Assert.Equal(settings.AllowScrollingBelowContent, appSettings.AllowScrollingBelowContent);
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
				HighlightCurrentLine = false,
				ShowLineNumbers = false,
				EnableVirtualSpace = true,
				EnableWordWrap = true,
				EmptySelectionCopiesEntireLine = false,
				AllowScrollingBelowContent = true
			};

			appSettings.RecentFiles.AddRange(new FileInfo(@"C:\recentFile1"), new FileInfo(@"C:\recentFile2"));

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
			Assert.Equal(false, settings.ShowLineNumbers);
			Assert.Equal(true, settings.EnableVirtualSpace);
			Assert.Equal(true, settings.EnableWordWrap);
			Assert.Equal(false, settings.EmptySelectionCopiesEntireLine);
			Assert.Equal(true, settings.AllowScrollingBelowContent);
		}

		[Fact]
		public void Test_Save_OpenFiles_EvenWhen_OpenFilesNotRemembered()
		{
			// Arrange.
			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"))
			{
				RememberOpenFiles = false,
				OpenFiles = new List<FileInfo> { new FileInfo(@"C:\openFile1"), new FileInfo(@"C:\openFile2") },
			};

			// Act.
			appSettings.Save();

			// Assert.
			AssertThat.SequenceEqual(settings.OpenFiles.Cast<string>(), new[] { @"C:\openFile1", @"C:\openFile2" });
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
			Assert.Equal(2, actual.Count);
			AssertThat.SequenceEqual(actual.Select(f => f.FullName), new[] { @"C:\file1", @"C:\file2" });
		}

		[Fact]
		public void Test_OpenFiles_Preserves_InsertionOrder()
		{
			// Arrange.
			settings.OpenFiles = new StringCollection();
			var files = new[] { new FileInfo(@"C:\fileC"), new FileInfo(@"C:\fileA"), new FileInfo(@"C:\fileB") };

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act.
			foreach (var file in files)
				appSettings.OpenFiles.Add(file);

			// Assert.
			AssertThat.SequenceEqual(appSettings.OpenFiles, files, FileInfoPathEqualityComparer.Instance);
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
			Assert.Equal(2, actual.Count);
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

		[Fact]
		public void Test_ShowLineNumbers_Changes()
		{
			// Arrange.
			settings.ShowLineNumbers = false;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.ShowLineNumbers,
				() => appSettings.ShowLineNumbers = true);

			Assert.True(appSettings.ShowLineNumbers);
		}

		[Fact]
		public void Test_EnableVirtualSpace_Changes()
		{
			// Arrange.
			settings.EnableVirtualSpace = false;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.EnableVirtualSpace,
				() => appSettings.EnableVirtualSpace = true);

			Assert.True(appSettings.EnableVirtualSpace);
		}

		[Fact]
		public void Test_EnableWordWrap_Changes()
		{
			// Arrange.
			settings.EnableWordWrap = false;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.EnableWordWrap,
				() => appSettings.EnableWordWrap = true);

			Assert.True(appSettings.EnableWordWrap);
		}

		[Fact]
		public void Test_EmptySelectionCopiesEntireLine_Changes()
		{
			// Arrange.
			settings.EmptySelectionCopiesEntireLine = false;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.EmptySelectionCopiesEntireLine,
				() => appSettings.EmptySelectionCopiesEntireLine = true);

			Assert.True(appSettings.EmptySelectionCopiesEntireLine);
		}

		[Fact]
		public void Test_AllowScrollingBelowContent_Changes()
		{
			// Arrange.
			settings.AllowScrollingBelowContent = false;

			var appSettings = new DotNetSettings(settings, new DirectoryInfo(@"C:\"));

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.AllowScrollingBelowContent,
				() => appSettings.AllowScrollingBelowContent = true);

			Assert.True(appSettings.AllowScrollingBelowContent);
		}

		private readonly Settings settings = new Settings
		{
			GraphVizLocation = @"C:\dot.exe" // doesn't have a default value
		};
	}
}