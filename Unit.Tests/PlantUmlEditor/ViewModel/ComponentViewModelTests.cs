using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PlantUmlEditor.Core.Dependencies;
using PlantUmlEditor.ViewModel;
using Utilities;
using Utilities.Concurrency;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class ComponentViewModelTests
	{
		[Fact]
		public void Test_Construction()
		{
			// Arrange.
			var component = new Mock<IExternalComponent>();
			component.SetupGet(c => c.Name).Returns("Name");
			
			// Act.
			var viewModel = new ComponentViewModel(component.Object);

			// Assert.
			Assert.Equal("Name", viewModel.Name);
		} 

		[Fact]
		[Synchronous]
		public async Task Test_LoadAsync()
		{
			// Arrange.
			var component = new Mock<IExternalComponent>();
			component.Setup(c => c.GetCurrentVersionAsync())
				.Returns(Task.FromResult("Version"));

			component.Setup(c => c.HasUpdateAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(Option<string>.Some("NewerVersion")));

			var viewModel = new ComponentViewModel(component.Object);

			// Act.
			await viewModel.LoadAsync();

			// Assert.
			Assert.Equal("Version", viewModel.CurrentVersion);
			Assert.True(viewModel.HasUpdate.HasValue);
			Assert.True(viewModel.HasUpdate.Value);
			Assert.Equal("NewerVersion", viewModel.LatestVersion);
		}

		[Fact]
		[Synchronous]
		public void Test_UpdateCommand()
		{
			// Arrange.
			var component = new Mock<IExternalComponent>();
			component.Setup(c => c.DownloadLatestAsync(It.IsAny<CancellationToken>(), It.IsAny<IProgress<DownloadProgressChangedEventArgs>>()))
				.Returns(Tasks.FromSuccess());

			var viewModel = new ComponentViewModel(component.Object);

			// Act.
			viewModel.UpdateCommand.Execute(null);

			// Assert.
			Assert.True(viewModel.UpdateCompleted);
		} 
	}
}