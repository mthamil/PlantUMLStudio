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
		/// Creates an already completed task from an existing result value.
		/// </summary>
		/// <typeparam name="TResult">The type of the result</typeparam>
		/// <param name="result">The existing result</param>
		/// <returns>A completed task</returns>
		public static Task<TResult> FromResult<TResult>(TResult result)
		{
			var taskSource = new TaskCompletionSource<TResult>();
			taskSource.SetResult(result);
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