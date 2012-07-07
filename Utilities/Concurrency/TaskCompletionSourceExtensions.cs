using System;
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
		/// <param name="resultSetter">The TaskCompletionSource to populate</param> 
		/// <param name="task">The task whose completion results should be transfered</param> 
		public static void TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task task)
		{
			switch (task.Status)
			{
				case TaskStatus.RanToCompletion:
					var taskWithResult = task as Task<TResult>;
					resultSetter.TrySetResult(taskWithResult != null
												? taskWithResult.Result		// Use the task's result IF it has one.
												: default(TResult));
					break;

				case TaskStatus.Faulted:
					if (task.Exception != null)
						resultSetter.TrySetException(task.Exception.InnerExceptions);
					break;

				case TaskStatus.Canceled:
					resultSetter.TrySetCanceled();
					break;

				default:
					throw new InvalidOperationException("The task was not completed.");
			}
		}
	}
}