using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PlantUmlStudio.Core.Dependencies;
using PlantUmlStudio.Core.Security;
using PlantUmlStudio.ViewModel;
using SharpEssentials;
using SharpEssentials.Testing;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.ViewModel
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
			var viewModel = new ComponentViewModel(component.Object, securityService.Object);

			// Assert.
			Assert.Equal("Name", viewModel.Name);
		} 

		[Fact]
		[Synchronous]
		public async Task Test_LoadAsync()
		{
			// Arrange.
			var component = new Mock<IExternalComponent>();
			component.Setup(c => c.GetCurrentVersionAsync(It.IsAny<CancellationToken>()))
			         .Returns(Task.FromResult("Version"));

			component.Setup(c => c.HasUpdateAsync(It.IsAny<CancellationToken>()))
			         .Returns(Task.FromResult(Option.Some("NewerVersion")));

			securityService.Setup(ss => ss.HasAdminPriviledges()).Returns(true);

			var viewModel = new ComponentViewModel(component.Object, securityService.Object);

			// Act.
			await viewModel.LoadAsync();

			// Assert.
			Assert.Equal("Version", viewModel.CurrentVersion);
			Assert.True(viewModel.HasAvailableUpdate.HasValue);
			Assert.True(viewModel.HasAvailableUpdate.Value);
			Assert.True(viewModel.CanUpdate);
			Assert.Equal("NewerVersion", viewModel.LatestVersion);
		}

		[Fact]
        [Synchronous]
        public async Task Test_UpdateCommand()
		{
			// Arrange.
			var component = new Mock<IExternalComponent>();
			component.Setup(c => c.DownloadLatestAsync(It.IsAny<IProgress<DownloadProgressChangedEventArgs>>(), It.IsAny<CancellationToken>()))
			         .Returns(Task.CompletedTask);

			var viewModel = new ComponentViewModel(component.Object, securityService.Object);

			// Act.
			await viewModel.UpdateCommand.ExecuteAsync(null);

			// Assert.
			Assert.True(viewModel.UpdateCompleted);
		}

		[Theory]
		[Synchronous]
		[InlineData(true, "someVersion", true)]
		[InlineData(false, null, true)]
		[InlineData(false, "someVersion", false)]
		[InlineData(false, null, false)]
		public async Task Test_CanUpdate(bool expected, string latestVersion, bool hasPermission)
		{
			// Arrange.
			var component = new Mock<IExternalComponent>();
            component.Setup(c => c.GetCurrentVersionAsync(It.IsAny<CancellationToken>()))
			         .Returns(Task.FromResult("Version"));

			component.Setup(c => c.HasUpdateAsync(It.IsAny<CancellationToken>()))
			         .Returns(Task.FromResult(Option.From(latestVersion)));

			securityService.Setup(ss => ss.HasAdminPriviledges()).Returns(hasPermission);

			var viewModel = new ComponentViewModel(component.Object, securityService.Object);
			await viewModel.LoadAsync();

			// Act.
			bool actual = viewModel.CanUpdate;

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly Mock<ISecurityService> securityService = new Mock<ISecurityService>();
	}
}