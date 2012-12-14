using PlantUmlEditor.ViewModel.Notifications;
using Xunit;
using Xunit.Extensions;

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

		[Theory]
		[InlineData("Short message.", "Short message.")]
		[InlineData("Message containing new", "Message containing new\r\nline.")]
		[InlineData("Verrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrry",
					"Verrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrry long message")]
		[InlineData("Verrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrry",
					"Verrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrry long new\r\nline message")]
		[InlineData("Verrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr",
					"Verrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr\r\nrrrrrrrrrrrrrrrrrrrrrry long new line message")]
		public void Test_Summary(string expected, string input)
		{
			// Act.
			var notification = new Notification(input);

			// Assert.
			Assert.Equal(expected, notification.Summary);
			Assert.Equal(notification.Summary != notification.Message, notification.HasMoreInfo);
		}
	}
}