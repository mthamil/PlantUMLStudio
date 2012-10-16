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
using System.Threading;
using System.Windows.Input;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command that performs cancellation using a CancellationTokenSource.
	/// </summary>
	public class CancelTaskCommand : ICommand
	{
		/// <summary>
		/// Initializes a command with a cancellation token source.
		/// </summary>
		/// <param name="cancellationTokenSource">Object that performs cancellation</param>
		public CancelTaskCommand(CancellationTokenSource cancellationTokenSource)
		{
			_cancellationTokenSource = cancellationTokenSource;
		}

		/// <see cref="ICommand.Execute"/>
		public void Execute(object parameter)
		{
			_cancellationTokenSource.Cancel();
			OnCanExecuteChanged();
		}

		/// <see cref="ICommand.Execute"/>
		public bool CanExecute(object parameter)
		{
			return !_cancellationTokenSource.IsCancellationRequested;
		}

		/// <see cref="ICommand.CanExecuteChanged"/>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				_canExecuteChanged += value;
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				_canExecuteChanged -= value;
				CommandManager.RequerySuggested -= value;
			}
		}

		private void OnCanExecuteChanged()
		{
			if (_canExecuteChanged != null)
				_canExecuteChanged(this, EventArgs.Empty);
		}

		private event EventHandler _canExecuteChanged;

		private readonly CancellationTokenSource _cancellationTokenSource;
	}
}