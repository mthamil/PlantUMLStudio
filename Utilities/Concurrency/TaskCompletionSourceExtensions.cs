using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Provides extension methods for TaskCompletionSource.
	/// </summary>
	public static class TaskCompletionSourceExtensions
	{
		/// <summary>
		/// Attempts to transfer the result of a Task to a TaskCompletionSource.
		/// </summary> 
		/// <typeparam name="TResult">The type of the result</typeparam> 
		/// <param name="completionSource">The TaskCompletionSource to populate</param> 
		/// <param name="task">The task whose completion results should be transfered</param> 
		public static void TrySetFromTask<TResult>(this TaskCompletionSource<TResult> completionSource, Task task)
		{
			switch (task.Status)
			{
				case TaskStatus.RanToCompletion:
					var taskWithResult = task as Task<TResult>;
					completionSource.TrySetResult(taskWithResult != null
												? taskWithResult.Result		// Use the task's result IF it has one.
												: default(TResult));
					break;

				case TaskStatus.Faulted:
					if (task.Exception != null)
						completionSource.TrySetException(task.Exception.InnerExceptions);
					break;

				case TaskStatus.Canceled:
					completionSource.TrySetCanceled();
					break;

				default:
					throw new InvalidOperationException("The task was not completed.");
			}
		}
		
		/// <summary>
		/// Adapts the Event-based Asynchronous Pattern to the 
		/// Task-based Asynchronous Pattern.
		/// </summary>
		/// <typeparam name="TResult">The type of the result</typeparam>
		/// <typeparam name="TAsyncArgs">The type of async event args</typeparam>
		/// <param name="completionSource">The TaskCompletionSource to populate</param>
		/// <param name="asyncArgs">The asynchronous event args to use</param>
		/// <param name="resultSelector">Retrieves the result from the event args</param>
		public static void SetFromEventArgs<TResult, TAsyncArgs>(this TaskCompletionSource<TResult> completionSource, TAsyncArgs asyncArgs, Func<TAsyncArgs, TResult> resultSelector)
			where TAsyncArgs : AsyncCompletedEventArgs
		{
			if (asyncArgs.Cancelled)
				completionSource.SetCanceled();
			else if (asyncArgs.Error != null)
				completionSource.SetException(asyncArgs.Error);
			else
				completionSource.SetResult(resultSelector(asyncArgs));
		}
	}
}