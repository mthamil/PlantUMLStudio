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
	}
}