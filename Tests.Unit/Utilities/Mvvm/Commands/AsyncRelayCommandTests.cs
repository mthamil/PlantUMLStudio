using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Mvvm.Commands;
using Xunit;

namespace Tests.Unit.Utilities.Mvvm.Commands
{
	public class AsyncRelayCommandTests : IDisposable
	{
		[Fact]
		public void Test_WithParameter_CanExecute_NoPredicate()
		{
			// Arrange.
			var command = new AsyncRelayCommand<bool>(DelayAndSet);

			// Act.
			bool actual = command.CanExecute(false);

			// Assert.
			Assert.True(actual);
		}

		[Fact]
		public void Test_WithParameter_CanExecute_WrongType()
		{
			// Arrange.
			var command = new AsyncRelayCommand<bool>(DelayAndSet, b => b);

			// Act.
			bool actual = command.CanExecute("parameter");

			// Assert.
			Assert.False(actual);
		}

		[Fact]
		public void Test_WithParameter_CanExecute()
		{
			// Arrange.
			var command = new AsyncRelayCommand<bool>(DelayAndSet, b => b);

			// Act.
			bool actual = command.CanExecute(true);

			// Assert.
			Assert.True(actual);
		}

		[Fact]
		public void Test_WithParameter_Execute()
		{
			// Arrange.
			var command = new AsyncRelayCommand<bool>(DelayAndSet);

			// Act.
			command.Execute(true);

			// Assert.
			resetEvent.Wait();
			Assert.True(resetEvent.IsSet);
		}

		[Fact]
		public void Test_WithoutParameter_CanExecute_NoPredicate()
		{
			// Arrange.
			var command = new AsyncRelayCommand(DelayAndSet);

			// Act.
			bool actual = command.CanExecute(false);

			// Assert.
			Assert.True(actual);
		}

		[Fact]
		public void Test_WithoutParameter_CanExecute()
		{
			// Arrange.
			var command = new AsyncRelayCommand(DelayAndSet, () => false);

			// Act.
			bool actual = command.CanExecute(true);

			// Assert.
			Assert.False(actual);
		}

		[Fact]
		public void Test_WithoutParameter_Execute()
		{
			// Arrange.
			var command = new AsyncRelayCommand(DelayAndSet);

			// Act.
			command.Execute(true);

			// Assert.
			resetEvent.Wait();
			Assert.True(resetEvent.IsSet);
		}

		private Task DelayAndSet()
		{
			return DelayAndSet(false);
		}

		private async Task DelayAndSet(bool parameter)
		{
			await Task.Delay(TimeSpan.FromMilliseconds(100));
			resetEvent.Set();
		}

		public void Dispose()
		{
			resetEvent.Dispose();
		}

		private readonly ManualResetEventSlim resetEvent = new ManualResetEventSlim();
	}
}