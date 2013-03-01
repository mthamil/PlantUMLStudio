//  PlantUML Editor
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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Concurrency.Processes
{
	/// <summary>
	/// Provides methods that enable using a Process as a Task.
	/// </summary>
	public static class ProcessTaskExtensions
	{
		/// <summary>
		/// Creates a Task representing an external background process that takes a stream
		/// as input and returns its output.
		/// </summary>
		/// <param name="taskFactory">Creates tasks</param>
		/// <param name="executable">The name of the executable to run</param>
		/// <param name="arguments">Any arguments to the process</param>
		/// <param name="input">The process input stream</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task<Tuple<Stream, Stream>> FromProcess(this TaskFactory taskFactory, string executable, string arguments, Stream input, CancellationToken cancellationToken = default(CancellationToken))
		{
			return taskFactory.FromProcess(new ProcessStartInfo
			{
				FileName = executable,
				Arguments = arguments,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}, input, cancellationToken);
		}

		/// <summary>
		/// Creates a Task representing an external process that takes a stream
		/// as input and returns its output.
		/// </summary>
		/// <param name="taskFactory">Creates tasks</param>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="input">The process input stream</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task<Tuple<Stream, Stream>> FromProcess(this TaskFactory taskFactory, ProcessStartInfo processInfo, Stream input, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ProcessAdapterFactory(taskFactory).StartNew(processInfo, input, cancellationToken);
		}

		/// <summary>
		/// Creates a Task representing an external process that takes
		/// no input and returns no output.
		/// </summary>
		/// <param name="taskFactory">Creates tasks</param>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task FromProcess(this TaskFactory taskFactory, ProcessStartInfo processInfo, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ProcessAdapterFactory(taskFactory).StartNew(processInfo, cancellationToken);
		}

		internal static Func<TaskFactory, IProcessTaskAdapter> ProcessAdapterFactory = tf => new ProcessTaskAdapter(tf, TaskScheduler.Default);
	}
}