using System;
using System.Threading.Tasks;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Provides extension methods for the Task type.
	/// </summary>
	public static class TaskExtensions
	{
		/// <summary>
		/// Schedules a task to be executed after completion of this task while propagating
		/// its result, error, or cancellation states.
		/// </summary>
		/// <typeparam name="T1">The result type of this Task</typeparam>
		/// <typeparam name="T2">The result type of the second Task</typeparam>
		/// <param name="first">The Task to execute first</param>
		/// <param name="next">The Task to execute after completion of the first</param>
		/// <returns>A Task representing completion of both Tasks</returns>
		public static Task<T2> Then<T1, T2>(this Task<T1> first, Func<T1, Task<T2>> next)
		{
			if (first == null) 
				throw new ArgumentNullException("first");
			if (next == null) 
				throw new ArgumentNullException("next");

			var tcs = new TaskCompletionSource<T2>();
			first.ContinueWith(delegate
			{
				if (first.IsFaulted)
				{
					tcs.TrySetException(first.Exception.InnerExceptions);
				}
				else if (first.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					try
					{
						var nextTask = next(first.Result);
						if (nextTask == null)
						{
							tcs.TrySetCanceled();
						}
						else nextTask.ContinueWith(delegate
						{
							TrySetFromTask(tcs, nextTask);
						}, TaskContinuationOptions.ExecuteSynchronously);
					}
					catch (Exception e) 
					{ 
						tcs.TrySetException(e); 
					}
				}
			}, TaskContinuationOptions.ExecuteSynchronously);
			return tcs.Task;
		}

		/// <summary>
		/// Schedules a continuation to be executed after completion of this task while propagating
		/// its result, error, or cancellation states.
		/// </summary>
		/// <typeparam name="T1">The result type of this Task</typeparam>
		/// <typeparam name="T2">The result type of the second Task</typeparam>
		/// <param name="first">The Task to execute first</param>
		/// <param name="continuation">A function to execute after completion of the first Task</param>
		/// <returns>A Task representing completion of both Tasks</returns>
		public static Task<T2> Then<T1, T2>(this Task<T1> first, Func<T1, T2> continuation)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (continuation == null)
				throw new ArgumentNullException("continuation");

			var tcs = new TaskCompletionSource<T2>();
			first.ContinueWith(delegate
			{
				if (first.IsFaulted)
				{
					tcs.TrySetException(first.Exception.InnerExceptions);
				}
				else if (first.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					try
					{
						var finalResult = continuation(first.Result);
						tcs.TrySetResult(finalResult);
					}
					catch (Exception e)
					{
						tcs.TrySetException(e);
					}
				}
			}, TaskContinuationOptions.ExecuteSynchronously);
			return tcs.Task;
		}

		/// <summary>
		/// Schedules a task to be executed after completion of this task while propagating
		/// its error or cancellation states.
		/// </summary>
		/// <typeparam name="T2">The result type of the second Task</typeparam>
		/// <param name="first">The Task to execute first</param>
		/// <param name="next">The Task to execute after completion of the first</param>
		/// <returns>A Task representing completion of both Tasks</returns>
		public static Task<T2> Then<T2>(this Task first, Func<Task<T2>> next)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (next == null)
				throw new ArgumentNullException("next");

			var tcs = new TaskCompletionSource<T2>();
			first.ContinueWith(delegate
			{
				if (first.IsFaulted)
				{
					tcs.TrySetException(first.Exception.InnerExceptions);
				}
				else if (first.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					try
					{
						var nextTask = next();
						if (nextTask == null)
						{
							tcs.TrySetCanceled();
						}
						else nextTask.ContinueWith(delegate
						{
							TrySetFromTask(tcs, nextTask);
						}, TaskContinuationOptions.ExecuteSynchronously);
					}
					catch (Exception e)
					{
						tcs.TrySetException(e);
					}
				}
			}, TaskContinuationOptions.ExecuteSynchronously);
			return tcs.Task;
		}

		/// <summary>
		/// Schedules a task to be executed after completion of this task while propagating
		/// its error or cancellation states.
		/// </summary>
		/// <param name="first">The Task to execute first</param>
		/// <param name="next">The Task to execute after completion of the first</param>
		/// <returns>A Task representing completion of both Tasks</returns>
		public static Task Then(this Task first, Func<Task> next)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (next == null)
				throw new ArgumentNullException("next");

			var tcs = new TaskCompletionSource<object>();
			first.ContinueWith(delegate
			{
				if (first.IsFaulted)
				{
					tcs.TrySetException(first.Exception.InnerExceptions);
				}
				else if (first.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					try
					{
						var nextTask = next();
						if (nextTask == null)
						{
							tcs.TrySetCanceled();
						}
						else nextTask.ContinueWith(delegate
						{
							TrySetFromTask(tcs, nextTask);
						}, TaskContinuationOptions.ExecuteSynchronously);
					}
					catch (Exception e)
					{
						tcs.TrySetException(e);
					}
				}
			}, TaskContinuationOptions.ExecuteSynchronously);
			return tcs.Task;
		}

		/// <summary>
		/// Attempts to transfer the result of a Task to a TaskCompletionSource.
		/// </summary> 
        /// <typeparam name="TResult">The type of the result</typeparam> 
        /// <param name="resultSetter">The TaskCompletionSource to populate</param> 
        /// <param name="task">The task whose completion results should be transfered</param> 
		private static void TrySetFromTask<TResult>(TaskCompletionSource<TResult> resultSetter, Task task)
		{
			switch (task.Status)
			{
				case TaskStatus.RanToCompletion:
					var taskWithResult = task as Task<TResult>;
					resultSetter.TrySetResult(taskWithResult != null ?
						taskWithResult.Result :	// use the task's result IF it has one
						default(TResult));
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