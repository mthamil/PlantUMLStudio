using Utilities.Mvvm.Commands;
using Xunit;

namespace Unit.Tests.Utilities.Mvvm.Commands
{
	public class RelayCommandTests
	{
		[Fact]
		public void Test_CanExecute_NoPredicate()
		{
			// Arrange.
			var command = new RelayCommand<bool>(b => { });

			// Act.
			bool actual = command.CanExecute(false);

			// Assert.
			Assert.True(actual);
		}

		[Fact]
		public void Test_CanExecute_WrongType()
		{
			// Arrange.
			var command = new RelayCommand<bool>(b => { }, b => b);

			// Act.
			bool actual = command.CanExecute("parameter");

			// Assert.
			Assert.False(actual);
		}

		[Fact]
		public void Test_CanExecute()
		{
			// Arrange.
			var command = new RelayCommand<bool>(b => { }, b => b);

			// Act.
			bool actual = command.CanExecute(true);

			// Assert.
			Assert.True(actual);
		}
	}
}