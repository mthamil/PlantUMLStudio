using PlantUmlEditor.ViewModel.Notifications;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel.Notifications
{
	public class NotificationTests
	{
		[Fact]
		public void Test_Properties()
		{
			// Arrange.
			var notification = new Notification("message");

			// Act.
			var message = notification.Message;
			var severity = notification.Severity;

			// Assert.
			Assert.Equal("message", message);
			Assert.Equal(Severity.Informational, severity);
		} 
	}
}