using System;
using Moq;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.ViewModel;
using Xunit;

namespace Tests.Unit.PlantUmlEditor.ViewModel
{
	public class SettingsViewModelTests
	{
		public SettingsViewModelTests()
		{
			settings.SetupAllProperties();
			viewModel = new SettingsViewModel(settings.Object);
		}

		[Fact]
		public void Test_Initialization()
		{
			// Arrange.
			settings.Object.RememberOpenFiles = true;
			settings.Object.AutoSaveEnabled = true;
			settings.Object.AutoSaveInterval = TimeSpan.FromSeconds(10);

			// Act.
			var settingsViewModel = new SettingsViewModel(settings.Object);

			// Assert.
			Assert.True(settingsViewModel.RememberOpenFiles);
			Assert.True(settingsViewModel.AutoSaveEnabled);
			Assert.Equal(TimeSpan.FromSeconds(10), settingsViewModel.AutoSaveInterval);
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
			        .Callback(() => 
						couldSave = viewModel.CanSave);

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
			viewModel.RememberOpenFiles = true;
			viewModel.AutoSaveEnabled = true;
			viewModel.AutoSaveInterval = TimeSpan.FromSeconds(45);

			// Act.
			viewModel.SaveCommand.Execute(null);

			// Assert.
			settings.Verify(s => s.Save());
			Assert.True(settings.Object.RememberOpenFiles);
			Assert.True(settings.Object.AutoSaveEnabled);
			Assert.Equal(TimeSpan.FromSeconds(45), settings.Object.AutoSaveInterval);
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

		private readonly SettingsViewModel viewModel;

		private readonly Mock<ISettings> settings = new Mock<ISettings> { DefaultValue = DefaultValue.Empty };
	}
}