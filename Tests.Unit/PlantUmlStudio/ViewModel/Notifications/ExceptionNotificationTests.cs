using System;
using PlantUmlStudio.ViewModel.Notifications;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.ViewModel.Notifications
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