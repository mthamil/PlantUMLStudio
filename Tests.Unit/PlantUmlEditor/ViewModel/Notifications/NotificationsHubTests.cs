using System.Linq;
using PlantUmlEditor.ViewModel.Notifications;
using Xunit;

namespace Tests.Unit.PlantUmlEditor.ViewModel.Notifications
{
	public class NotificationsHubTests
	{
		[Fact]
		public void Test_Notify()
		{
			// Arrange.
			var notification = new Notification("Test");

			// Act/Assert.
			AssertThat.PropertyChanged(notifications, p => p.LatestNotification, 
				() => notifications.Notify(notification));

			Assert.Equal(notification, notifications.LatestNotification);
			Assert.Single(notifications.Notifications);
			Assert.Equal(notification, notifications.Notifications.Single());
		}

		[Fact]
		[Synchronous]
		public void Test_StartProgress()
		{
			// Act.
			var progress = notifications.StartProgress(true);
			progress.Report(new ProgressUpdate { Message = "Start", PercentComplete = 0 });

			// Assert.
			Assert.Single(notifications.Notifications);
			Assert.Equal("Start", notifications.Notifications.Single().Message);
		}

		[Fact]
		[Synchronous]
		public void Test_StartProgress_Completed()
		{
			// Act.
			var progress = notifications.StartProgress(true);
			progress.Report(new ProgressUpdate { Message = "Start", PercentComplete = 0 });
			progress.Report(ProgressUpdate.Completed("Finished!"));

			// Assert.
			Assert.Single(notifications.Notifications);
			Assert.Equal("Finished!", notifications.Notifications.Single().Message);
		}

		private readonly NotificationsHub notifications = new NotificationsHub();
	}
}