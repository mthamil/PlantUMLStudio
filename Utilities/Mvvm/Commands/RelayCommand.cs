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
using System.Diagnostics;
using System.Windows.Input;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. The default return value for the CanExecute
	/// method is 'true'.  This command does not take any parameters.
	/// </summary>
	public class RelayCommand : CommandBase
	{
		/// <summary>
		/// Initializes a new command.
		/// </summary>
		/// <param name="execute">The operation to execute</param>
		/// <param name="canExecute">Function that determines whether a command can be executed</param>
		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#region ICommand Members

		/// <see cref="ICommand.CanExecute"/>
		[DebuggerStepThrough]
		public override bool CanExecute(object parameter)
		{
			if (_canExecute == null)
				return true;

			return _canExecute();
		}

		/// <see cref="ICommand.Execute"/>
		public override void Execute(object parameter)
		{
			_execute();
		}

		#endregion

		readonly Action _execute;
		readonly Func<bool> _canExecute;
	}

	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates.  In order for CanExecute to return
	/// true, the command parameter must of type T.
	/// </summary>
	/// <typeparam name="T">The type of parameter to be passed to the command</typeparam>
	public class RelayCommand<T> : CommandBase
	{
		/// <summary>
		/// Initializes a new command.
		/// </summary>
		/// <param name="execute">The operation to execute</param>
		/// <param name="canExecute">Function that determines whether a command can be executed</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#region ICommand Members

		/// <see cref="ICommand.CanExecute"/>
		[DebuggerStepThrough]
		public override bool CanExecute(object parameter)
		{
			if (_canExecute == null)
				return true;

			if (parameter is T)
				return _canExecute((T)parameter);

			return false;
		}

		/// <see cref="ICommand.Execute"/>
		public override void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		#endregion

		readonly Action<T> _execute;
		readonly Predicate<T> _canExecute;
	}
}
