using System;
using System.Threading;
using Utilities.Mvvm.Commands;
using Xunit;

namespace Unit.Tests.Utilities.Mvvm.Commands
{
	public class CancelTaskCommandTests
	{
		[Fact]
		public void Test_Cancel()
		{
			// Arrange.
			var cts = new CancellationTokenSource();
			var command = new CancelTaskCommand(cts);

			EventArgs args = null;
			EventHandler canExecuteChangedHandler = (o, e) => args = e;
			command.CanExecuteChanged += canExecuteChangedHandler;

			// Act.
			command.Execute(null);

			// Assert.
			Assert.True(cts.IsCancellationRequested);
			Assert.False(command.CanExecute(null));
			Assert.NotNull(args);
		}
	}
}