using System.Windows;
using Utilities.Controls.Commands;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel.Commands
{
	public class CloseWindowCommandTests
	{
		[Fact]
		public void Test_CanExecute()
		{
			// Act.
			var canExecute = _command.CanExecute(null);

			// Assert.
			Assert.True(canExecute);
		}

		[Fact]
		public void Test_Execute()
		{
			// Arrange.
			var window = new Window();

			bool closed = false;
			window.Closed += (o, e) => closed = true;

			// Act.
			_command.Execute(window);

			// Assert.
			Assert.True(closed);
		}

		private readonly CloseWindowCommand _command = new CloseWindowCommand();
	}
}