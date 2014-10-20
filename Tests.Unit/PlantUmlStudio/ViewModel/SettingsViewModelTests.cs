using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Moq;
using PlantUmlStudio.Configuration;
using PlantUmlStudio.ViewModel;
using SharpEssentials.Testing;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PlantUmlStudio.ViewModel
{
	public class SettingsViewModelTests
	{
		public SettingsViewModelTests()
		{
			settings.SetupAllProperties();

			recentFiles.As<INotifyCollectionChanged>();
			recentFiles.SetupGet(rf => rf.Count)
					   .Returns(1);
			settings.SetupGet(s => s.RecentFiles)
					.Returns(recentFiles.Object);

			viewModel = new SettingsViewModel(settings.Object);
		}

		[Fact]
		public void Test_Initialization()
		{
			// Arrange.
			settings.Object.RememberOpenFiles = true;
			settings.Object.AutoSaveEnabled = true;
			settings.Object.AutoSaveInterval = TimeSpan.FromSeconds(10);
			settings.Object.MaximumRecentFiles = 20;
			settings.Object.HighlightCurrentLine = true;
			settings.Object.ShowLineNumbers = true;
			settings.Object.EnableVirtualSpace = true;
			settings.Object.EnableWordWrap = true;
			settings.Object.EmptySelectionCopiesEntireLine = true;
			settings.Object.AllowScrollingBelowContent = true;

			// Act.
			var settingsViewModel = new SettingsViewModel(settings.Object);

			// Assert.
			Assert.True(settingsViewModel.RememberOpenFiles);
			Assert.True(settingsViewModel.AutoSaveEnabled);
			Assert.Equal(TimeSpan.FromSeconds(10), settingsViewModel.AutoSaveInterval);
			Assert.Equal(20, settingsViewModel.MaximumRecentFiles);
			Assert.True(settingsViewModel.HighlightCurrentLine);
			Assert.True(settingsViewModel.ShowLineNumbers);
			Assert.True(settingsViewModel.EnableVirtualSpace);
			Assert.True(settingsViewModel.EnableWordWrap);
			Assert.True(settingsViewModel.EmptySelectionCopiesEntireLine);
			Assert.True(settingsViewModel.AllowScrollingBelowContent);
		}

		[Fact]
		public void Test_RememberOpenFiles_Changes()
		{
			// Arrange.
			viewModel.RememberOpenFiles = false;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.RememberOpenFiles,
				() => viewModel.RememberOpenFiles = true);

			Assert.True(viewModel.RememberOpenFiles);
		}

		[Fact]
		public void Test_AutoSaveEnabled_Changes()
		{
			// Arrange.
			viewModel.AutoSaveEnabled = false;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.AutoSaveEnabled,
				() => viewModel.AutoSaveEnabled = true);

			Assert.True(viewModel.AutoSaveEnabled);
		}

		[Fact]
		public void Test_AutoSaveInterval_Changes()
		{
			// Arrange.
			viewModel.AutoSaveInterval = TimeSpan.FromSeconds(0);

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.AutoSaveInterval,
				() => viewModel.AutoSaveInterval = TimeSpan.FromSeconds(30));

			Assert.Equal(TimeSpan.FromSeconds(30), viewModel.AutoSaveInterval);
		}

		[Fact]
		public void Test_MaximumRecentFiles_Changes()
		{
			// Arrange.
			viewModel.MaximumRecentFiles = 0;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.MaximumRecentFiles,
				() => viewModel.MaximumRecentFiles = 6);

			Assert.Equal(6, viewModel.MaximumRecentFiles);
		}

		[Fact]
		public void Test_HighlightCurrentLine_Changes()
		{
			// Arrange.
			viewModel.HighlightCurrentLine = false;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.HighlightCurrentLine,
				() => viewModel.HighlightCurrentLine = true);

			Assert.True(viewModel.HighlightCurrentLine);
		}

		[Fact]
		public void Test_ShowLineNumbers_Changes()
		{
			// Arrange.
			viewModel.ShowLineNumbers = false;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.ShowLineNumbers,
				() => viewModel.ShowLineNumbers = true);

			Assert.True(viewModel.ShowLineNumbers);
		}

		[Fact]
		public void Test_EnableVirtualSpace_Changes()
		{
			// Arrange.
			viewModel.EnableVirtualSpace = false;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.EnableVirtualSpace,
				() => viewModel.EnableVirtualSpace = true);

			Assert.True(viewModel.EnableVirtualSpace);
		}

		[Fact]
		public void Test_EnableWordWrap_Changes()
		{
			// Arrange.
			viewModel.EnableWordWrap = false;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.EnableWordWrap,
				() => viewModel.EnableWordWrap = true);

			Assert.True(viewModel.EnableWordWrap);
		}

		[Fact]
		public void Test_EmptySelectionCopiesEntireLine_Changes()
		{
			// Arrange.
			viewModel.EmptySelectionCopiesEntireLine = false;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.EmptySelectionCopiesEntireLine,
				() => viewModel.EmptySelectionCopiesEntireLine = true);

			Assert.True(viewModel.EmptySelectionCopiesEntireLine);
		}

		[Fact]
		public void Test_AllowScrollingBelowContent_Changes()
		{
			// Arrange.
			viewModel.AllowScrollingBelowContent = false;

			// Act/Assert.
			AssertThat.PropertyChanged(viewModel,
				s => s.AllowScrollingBelowContent,
				() => viewModel.AllowScrollingBelowContent = true);

			Assert.True(viewModel.AllowScrollingBelowContent);
		}

		[Theory]
		[InlineData(true, 1)]
		[InlineData(true, 2)]
		[InlineData(false, 0)]
		public void Test_CanClearRecentFiles_Initialization(bool expected, int initialCount)
		{
			// Arrange.
			recentFiles.SetupGet(rf => rf.Count).Returns(initialCount);

			// Act.
			var settingsViewModel = new SettingsViewModel(settings.Object);

			// Assert.
			Assert.Equal(expected, settingsViewModel.CanClearRecentFiles);
		}

		[Theory]
		[InlineData(true,  1, false)]
		[InlineData(true,  2, false)]
		[InlineData(false, 0, false)]
		[InlineData(false, 1, true)]
		public void Test_CanClearRecentFiles_UpdatedWhenCollectionChanges(bool expected, int count, bool alreadyCleared)
		{
			// Arrange.
			recentFiles.SetupGet(rf => rf.Count).Returns(count);

			recentFiles.As<INotifyCollectionChanged>()
			           .Raise(rf => rf.CollectionChanged += null,
			                  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

			if (alreadyCleared)
				viewModel.ClearRecentFiles();

			// Act.
			bool actual = viewModel.CanClearRecentFiles;

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Test_ClearRecentFilesCommand_OnlyQueuesUpClearAction()
		{
			// Act.
			viewModel.ClearRecentFilesCommand.Execute(null);

			// Assert.
			recentFiles.Verify(rf => rf.Clear(), Times.Never());
			Assert.False(viewModel.CanClearRecentFiles);
		}

		[Fact]
		public void Test_CanSave_DefaultIsTrue()
		{
			// Arrange.
			const bool expected = true;

			// Act.
			bool actual = viewModel.CanSave;

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Test_CanSave_NotWhileSaveInProgress()
		{
			// Arrange.
			bool couldSave = true;

			settings.Setup(s => s.Save())
			        .Callback(() => couldSave = viewModel.CanSave);

			// Act.
			viewModel.Save();

			// Assert.
			Assert.False(couldSave);
			Assert.True(viewModel.CanSave);
		}

		[Fact]
		public void Test_SaveCommand()
		{
			// Arrange.
			settings.SetupGet(s => s.RecentFiles)
					.Returns(new[] { "file1", "file2" }.Select(f => new FileInfo(f)).ToList());

			viewModel.RememberOpenFiles = true;
			viewModel.AutoSaveEnabled = true;
			viewModel.AutoSaveInterval = TimeSpan.FromSeconds(45);
			viewModel.MaximumRecentFiles = 12;
			viewModel.ClearRecentFiles();
			viewModel.HighlightCurrentLine = true;
			viewModel.ShowLineNumbers = true;
			viewModel.EnableVirtualSpace = true;
			viewModel.EnableWordWrap = true;
			viewModel.EmptySelectionCopiesEntireLine = true;
			viewModel.AllowScrollingBelowContent = true;

			// Act.
			viewModel.SaveCommand.Execute(null);

			// Assert.
			settings.Verify(s => s.Save());
			Assert.True(settings.Object.RememberOpenFiles);
			Assert.True(settings.Object.AutoSaveEnabled);
			Assert.Equal(TimeSpan.FromSeconds(45), settings.Object.AutoSaveInterval);
			Assert.Equal(12, settings.Object.MaximumRecentFiles);
			Assert.Empty(settings.Object.RecentFiles);
			Assert.True(settings.Object.HighlightCurrentLine);
			Assert.True(settings.Object.ShowLineNumbers);
			Assert.True(settings.Object.EnableVirtualSpace);
			Assert.True(settings.Object.EnableWordWrap);
			Assert.True(settings.Object.EmptySelectionCopiesEntireLine);
			Assert.True(settings.Object.AllowScrollingBelowContent);
		}

		[Fact]
		public void Test_SaveCompleted()
		{
			// Act.
			bool? beforeSave = viewModel.SaveCompleted;
			viewModel.Save();
			bool? afterSave = viewModel.SaveCompleted;

			// Assert.
			Assert.Null(beforeSave);
			Assert.True(afterSave.HasValue);
			Assert.True(afterSave.Value);
		}

		[Fact]
		public void Test_Save_UnsubscribesFromCollectionChanges()
		{
			// Arrange.
			recentFiles.SetupGet(rf => rf.Count)
			           .Returns(0);

			// Act.
			viewModel.Save();

			// Assert.
			AssertThat.PropertyDoesNotChange(viewModel, 
				p => p.CanClearRecentFiles, 
				() => recentFiles.As<INotifyCollectionChanged>()
						.Raise(rf => rf.CollectionChanged += null, 
							new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
		}

		private readonly SettingsViewModel viewModel;

		private readonly Mock<ISettings> settings = new Mock<ISettings> { DefaultValue = DefaultValue.Empty };
		private readonly Mock<ICollection<FileInfo>> recentFiles = new Mock<ICollection<FileInfo>>(); 
	}
}