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

using System.IO;

namespace Utilities.Concurrency.Processes
{
	/// <summary>
	/// Contains the streams resulting from the execution of an external process.
	/// </summary>
	public class ProcessResult
	{
		/// <summary>
		/// Initializes a new <see cref="ProcessResult"/>.
		/// </summary>
		/// <param name="output">The data read from the output stream</param>
		/// <param name="error">The data read from the error stream</param>
		public ProcessResult(Stream output, Stream error)
		{
			Output = output;
			Error = error;
		}

		/// <summary>
		/// The data read from the output stream.
		/// </summary>
		public Stream Output { get; private set; }

		/// <summary>
		/// The data read from the error stream.
		/// </summary>
		public Stream Error { get; private set; }
	}
}