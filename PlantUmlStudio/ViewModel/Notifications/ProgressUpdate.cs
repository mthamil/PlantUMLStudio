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
using System.Linq;
using SharpEssentials.Diagnostics;

namespace PlantUmlStudio.ViewModel.Notifications
{
	/// <summary>
	/// Represents a progress update.
	/// </summary>
	public class ProgressUpdate
	{
		/// <summary>
		/// The percentage complete.
		/// </summary>
		public int? PercentComplete { get; set; }

		/// <summary>
		/// The message associated with the update.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Whether progress is finished.
		/// </summary>
		public bool IsFinished { get; set; }

		/// <summary>
		/// Creates a new progress update indicating failed completion due to an exception.
		/// </summary>
		/// <param name="exception">The exception that caused termination of progress</param>
		public static ProgressUpdate Failed(Exception exception)
		{
			string message = String.Join(Environment.NewLine, exception.GetExceptionChain().Select(e => e.Message));
			return new ProgressUpdate { PercentComplete = null, Message = message, IsFinished = true };
		}

		/// <summary>
		/// Creates a new progress update indicating successful completion.
		/// </summary>
		/// <param name="message">The completion message.</param>
		public static ProgressUpdate Completed(string message)
		{
			return new ProgressUpdate { PercentComplete = null, Message = message, IsFinished = true };
		}
	}
}