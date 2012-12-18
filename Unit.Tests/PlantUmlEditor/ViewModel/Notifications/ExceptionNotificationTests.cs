using System;
using PlantUmlEditor.ViewModel.Notifications;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel.Notifications
{
	public class ExceptionNotificationTests
	{
		[Fact]
		public void Test_Properties()
		{
			// Arrange.
			var notification = new ExceptionNotification(new InvalidOperationException("message"));

			// Act.
			var message = notification.Message;
			var severity = notification.Severity;

			// Assert.
			Assert.Equal("message", message);
			Assert.Equal(Severity.Critical, severity);
		}

		[Fact]
		public void Test_InnerExceptions()
		{
			// Arrange.
			var notification = new ExceptionNotification(
				new InvalidOperationException("message1", 
					new InvalidOperationException("message2",
						new InvalidOperationException("message3"))));

			// Act.
			var message = notification.Message;
			var severity = notification.Severity;

			// Assert.
			Assert.Equal(String.Format("message1{0}message2{0}message3", Environment.NewLine), message);
			Assert.Equal(Severity.Critical, severity);
		} 
	}
}