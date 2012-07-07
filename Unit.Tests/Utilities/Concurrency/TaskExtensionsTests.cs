using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Concurrency;
using Xunit;

namespace Unit.Tests.Utilities.Concurrency
{
	public class TaskExtensionsTests
	{
		[Fact]
		public void Test_Then_WithResults_BothComplete()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => 1m, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() => (int)result + 1, cts.Token, TaskCreationOptions.None, taskScheduler));

			task.Wait();

			// Assert.
			Assert.Equal(2, task.Result);
			Assert.Equal(TaskStatus.RanToCompletion, task.Status);
		}

		[Fact]
		public void Test_Then_WithResults_FirstCompletes_SecondFails()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => 1m, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() => { throw new InvalidOperationException(); }, cts.Token, TaskCreationOptions.None, taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResults_FirstCompletes_SecondFailsCreation()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => 1m, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => (Task<int>)null);


			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResults_FirstCompletes_SecondCanceled()
		{
			// Act.
			Task<int> task =
				Task.Factory.StartNew(() => 1m, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() =>
					{
						cts.Cancel();
						cts.Token.ThrowIfCancellationRequested();
						return 1;
					}, cts.Token, TaskCreationOptions.None, taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResults_FirstFails()
		{
			// Act.
			Task<int> task = 
				Task<decimal>.Factory.StartNew(() => { throw new InvalidOperationException(); }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() => (int)result + 1, cts.Token, TaskCreationOptions.None, taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResults_FirstCanceled()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() =>
				{
					cts.Cancel();
					cts.Token.ThrowIfCancellationRequested();
					return 1m;
				}, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() => (int)result + 1, cts.Token, TaskCreationOptions.None, taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_BothComplete()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler));

			task.Wait();

			// Assert.
			Assert.Equal(TaskStatus.RanToCompletion, task.Status);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstCompletes_SecondFails()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task.Factory.StartNew(() => { throw new InvalidOperationException(); }, cts.Token, TaskCreationOptions.None, taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstCompletes_SecondFailsCreation()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() => 1m, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => (Task)null);


			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstCompletes_SecondCanceled()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() => 1m, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task.Factory.StartNew(() =>
					{
						cts.Cancel();
						cts.Token.ThrowIfCancellationRequested();;
					}, cts.Token, TaskCreationOptions.None, taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstFails()
		{
			// Act.
			Task task = 
				Task<decimal>.Factory.StartNew(() => { throw new InvalidOperationException(); }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstCanceled()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() =>
				{
					cts.Cancel();
					cts.Token.ThrowIfCancellationRequested();
				}, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_BothComplete()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() => 1, cts.Token, TaskCreationOptions.None, taskScheduler));

			task.Wait();

			// Assert.
			Assert.Equal(1, task.Result);
			Assert.Equal(TaskStatus.RanToCompletion, task.Status);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstCompletes_SecondFails()
		{
			// Act.
			Task<int> task =
				Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() => { throw new InvalidOperationException(); }, cts.Token, TaskCreationOptions.None, taskScheduler));

			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstCompletes_SecondFailsCreation()
		{
			// Act.
			Task<int> task =
				Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => (Task<int>)null);

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstCompletes_SecondCanceled()
		{
			// Act.
			Task<int> task =
				Task.Factory.StartNew(() => { }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() =>
					{
						cts.Cancel();
						cts.Token.ThrowIfCancellationRequested();
						return 1;
					}, cts.Token, TaskCreationOptions.None, taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstFails()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => { throw new InvalidOperationException(); }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() => 1, cts.Token, TaskCreationOptions.None, taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstCanceled()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() =>
				{
					cts.Cancel();
					cts.Token.ThrowIfCancellationRequested();
				}, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() => 1, cts.Token, TaskCreationOptions.None, taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithContinuation_BothComplete()
		{
			// Act.
			Task<string> task = 
				Task.Factory.StartNew(() => 1m, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => result.ToString());

			task.Wait();

			// Assert.
			Assert.Equal("1", task.Result);
		}

		[Fact]
		public void Test_Then_WithContinuation_TaskFails()
		{
			// Act.
			Task<string> task =
				Task<int>.Factory.StartNew(() => { throw new InvalidOperationException(); }, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => result.ToString());

			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithContinuation_TaskCancelled()
		{
			// Act.
			Task<string> task = 
				Task<decimal>.Factory.StartNew(() =>
				{
					cts.Cancel();
					cts.Token.ThrowIfCancellationRequested();
					return 1m;
				}, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(result => result.ToString());

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithContinuation_ContinuationFails()
		{
			// Act.
			Task<string> task = 
				Task.Factory.StartNew(() => 1m, cts.Token, TaskCreationOptions.None, taskScheduler)
				.Then(new Func<decimal, string>(result => { throw new InvalidOperationException(); }));

			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		private readonly TaskScheduler taskScheduler = new SynchronousTaskScheduler();
		private readonly CancellationTokenSource cts = new CancellationTokenSource();
	}
}