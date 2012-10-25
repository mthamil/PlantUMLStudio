using PlantUmlEditor.ViewModel.Notifications;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel.Notifications
{
	public class ProgressNotificationTests
	{
		[Fact]
		public void Test_Severity()
		{
			// Arrange.
			var notification = new ProgressNotification();

			// Act.
			var severity = notification.Severity;

			// Assert.
			Assert.Equal(Severity.Informational, severity);
		}  
	}
}