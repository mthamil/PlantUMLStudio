using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Collections;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Contains Task utility methods.
	/// </summary>
	public static class Tasks
	{
		/// <summary>
		/// Creates an already successfully completed task with no result value.
		/// </summary>
		/// <returns>A successfully completed task</returns>
		public static Task FromSuccess()
		{
			return completed;
		}
		private static readonly Task completed = Task.FromResult(default(AsyncUnit));

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
			return FromCanceled<AsyncUnit>();
		}

		/// <summary>
		/// Creates an already completed task from an exception.
		/// </summary>
		/// <typeparam name="TResult">The expected result type</typeparam>
		/// <param name="exception">An exception</param>
		/// <returns>A Task that has failed due to the given exception</returns>
		public static Task<TResult> FromException<TResult>(Exception exception)
		{
			var taskSource = new TaskCompletionSource<TResult>();
			taskSource.SetException(exception);
			return taskSource.Task;
		}

		/// <summary>
		/// Creates an already completed task from an exception.
		/// </summary>
		/// <param name="exception">An existing exception</param>
		/// <returns>A Task that has failed due to the given exception</returns>
		public static Task FromException(Exception exception)
		{
			return FromException<AsyncUnit>(exception);
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

		/// <summary>
		/// Creates an already completed task from a collection of exceptions.
		/// </summary>
		/// <typeparam name="TResult">The expected result type</typeparam>
		/// <param name="first">The first exception</param>
		/// <param name="exceptions">Existing exceptions</param>
		/// <returns>A Task that has failed due to the given exceptions</returns>
		public static Task<TResult> FromExceptions<TResult>(Exception first, params Exception[] exceptions)
		{
			var taskSource = new TaskCompletionSource<TResult>();
			taskSource.SetException(first.ToEnumerable().Concat(exceptions));
			return taskSource.Task;
		}

		/// <summary>
		/// Creates an already completed task from a collection of exceptions.
		/// </summary>
		/// <param name="first">The first exception</param>
		/// <param name="exceptions">Existing exceptions</param>
		/// <returns>A Task that has failed due to the given exceptions</returns>
		public static Task FromExceptions(Exception first, params Exception[] exceptions)
		{
			return FromExceptions<AsyncUnit>(first, exceptions);
		}

		/// <summary>
		/// Instead of using System.Object, this type is a clearer indication that a Task does not return a result.
		/// </summary>
		struct AsyncUnit { }
	}
}