//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
		/// Task-based Asynchronous Pattern for asynchronous operations
		/// that do not return a result.
		/// </summary>
		/// <typeparam name="TAsyncArgs">The type of async event args</typeparam>
		/// <param name="completionSource">The TaskCompletionSource to populate</param>
		/// <param name="asyncArgs">The asynchronous event args to use</param>
		public static void TrySetFromEventArgs<TAsyncArgs>(this TaskCompletionSource<object> completionSource, TAsyncArgs asyncArgs)
			where TAsyncArgs : AsyncCompletedEventArgs
		{
			completionSource.TrySetFromEventArgs(asyncArgs, _ => null);
		}
		
		/// <summary>
		/// Adapts the Event-based Asynchronous Pattern to the 
		/// Task-based Asynchronous Pattern for asynchronous operations
		/// that return a result.
		/// </summary>
		/// <typeparam name="TResult">The type of the result</typeparam>
		/// <typeparam name="TAsyncArgs">The type of async event args</typeparam>
		/// <param name="completionSource">The TaskCompletionSource to populate</param>
		/// <param name="asyncArgs">The asynchronous event args to use</param>
		/// <param name="resultSelector">Retrieves the result from the event args</param>
		public static void TrySetFromEventArgs<TResult, TAsyncArgs>(this TaskCompletionSource<TResult> completionSource, TAsyncArgs asyncArgs, Func<TAsyncArgs, TResult> resultSelector)
			where TAsyncArgs : AsyncCompletedEventArgs
		{
			if (asyncArgs.Cancelled)
				completionSource.TrySetCanceled();
			else if (asyncArgs.Error != null)
				completionSource.TrySetException(asyncArgs.Error);
			else
				completionSource.TrySetResult(resultSelector(asyncArgs));
		}
	}
}