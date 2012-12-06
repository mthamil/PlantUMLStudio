//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Concurrency.Processes
{
	/// <summary>
	/// Interface for an object that executes asynchronous operations through external Processes.
	/// </summary>
	public interface IProcessTaskAdapter
	{
		/// <summary>
		/// Executes a Process that does not return results or take input.
		/// </summary>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="cancellationToken">Allows termination of the process</param>
		/// <returns>A Task representing the Process</returns>
		Task StartNew(ProcessStartInfo processInfo, CancellationToken cancellationToken);

		/// <summary>
		/// Executes a Process that takes data written to its input stream
		/// and returns data read from its output stream.
		/// </summary>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="input">The data to write to the Process's input stream</param>
		/// <param name="cancellationToken">Allows termination of the process</param>
		/// <returns> A Task that, when completed successfully, contains a Process's output and error streams</returns>
		Task<Tuple<Stream, Stream>> StartNew(ProcessStartInfo processInfo, Stream input, CancellationToken cancellationToken);
	}
}