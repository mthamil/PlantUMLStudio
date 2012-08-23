using System;
using System.Threading.Tasks;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Contains Task utility methods.
	/// </summary>
	public static class Tasks
	{
		/// <summary>
		/// Creates an already completed task with no result value.
		/// </summary>
		/// <returns>A successfully completed task</returns>
		public static Task FromSuccess()
		{
			return completed;
		}
		private static readonly Task completed = Task.FromResult<object>(null);

		/// <summary>
		/// Creates an already canceled task.
		/// </summary>
		/// <typeparam name="TResult">The type of the result the task was supposed to return</typeparam>
		/// <returns>A canceled task</returns>
		public static Task<TResult> FromCanceled<TResult>()
		{
			var taskSource = new TaskCompletionSource<TResult>();
			taskSource.SetCanceled();
			return taskSource.Task;
		}

		/// <summary>
		/// Creates an already canceled task.
		/// </summary>
		/// <returns>A canceled task</returns>
		public static Task FromCanceled()
		{
			var taskSource = new TaskCompletionSource<object>();
			taskSource.SetCanceled();
			return taskSource.Task;
		}

		/// <summary>
		/// Creates an already completed task from an exception.
		/// </summary>
		/// <typeparam name="TResult">The expected result type</typeparam>
		/// <typeparam name="TException">The type of exception that was "thrown"</typeparam>
		/// <param name="exception">An exception</param>
		/// <param name="result">A value of the expected result type that may be supplied to help type inference.
		/// This value will not be used otherwise.</param>
		/// <returns>A Task that has failed due to the given exception</returns>
		public static Task<TResult> FromException<TResult, TException>(TException exception, TResult result = default(TResult))
			where TException : Exception
		{
			var taskSource = new TaskCompletionSource<TResult>();
			taskSource.SetException(exception);
			return taskSource.Task;
		}

		/// <summary>
		/// Creates an already completed task from an exception.
		/// </summary>
		/// <typeparam name="TException">The type of exception that was "thrown"</typeparam>
		/// <param name="exception">An existing exception</param>
		/// <returns>A Task that has failed due to the given exception</returns>
		public static Task FromException<TException>(TException exception)
			where TException : Exception
		{
			var taskSource = new TaskCompletionSource<object>();
			taskSource.SetException(exception);
			return taskSource.Task;
		}

		/// <summary>
		/// Creates an already completed task from an exception.
		/// </summary>
		/// <typeparam name="TException">The type of exception that was "thrown"</typeparam>
		/// <returns>A Task that has failed due to the given exception</returns>
		public static Task FromException<TException>()
			where TException : Exception, new()
		{
			return FromException(new TException());
		}
	}
}