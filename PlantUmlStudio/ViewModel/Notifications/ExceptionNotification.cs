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
using SharpEssentials.Collections;
using SharpEssentials.Diagnostics;

namespace PlantUmlStudio.ViewModel.Notifications
{
	/// <summary>
	/// A notification backed by an exception.
	/// </summary>
	public class ExceptionNotification : Notification
	{
		/// <summary>
		/// Creates a notification based on an exception.
		/// </summary>
		/// <param name="exception"></param>
		public ExceptionNotification(Exception exception)
		{
			_exception = exception;

			Message = exception.GetExceptionChain().Select(e => e.Message).ToDelimitedString(Environment.NewLine);
			Severity = Severity.Critical;
		}

		private readonly Exception _exception;
	}
}