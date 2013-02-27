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
using System.Threading.Tasks;
using System.Windows.Input;

namespace Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that enables a simpler syntax for defining asynchronous command operations.
	/// That is, async/await are not necessary and method references for Task-returning methods
	/// can be used.
	/// </summary>
	public class AsyncCommandCompleterWrapper : IAsyncCommandCompleter
	{
		public AsyncCommandCompleterWrapper(ICommandCompleter completer)
		{
			_completer = completer;
		}

		/// <see cref="IAsyncCommandCompleter.Executes(Func{Task})"/>
		public ICommand Executes(Func<Task> operation)
		{
			return _completer.Executes(async () => await operation());
		}

		/// <see cref="IAsyncCommandCompleter.Executes(Func{object,Task})"/>
		public ICommand Executes(Func<object, Task> operation)
		{
			return _completer.Executes(async parameter => await operation(parameter));
		}

		private readonly ICommandCompleter _completer;
	}
}