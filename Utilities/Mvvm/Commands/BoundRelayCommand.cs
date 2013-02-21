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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. CanExecute and CanExecuteChanged are bound to 
	/// a property of another object.
	/// </summary>
	public class BoundRelayCommand : ICommand
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="propertyDeclarer">The object that declares the property that triggers a change in command execution status</param>
		/// <param name="propertyName">The name of the bound property</param>
		/// <param name="execute">The operation to execute</param>
		/// <param name="canExecute">Function that determines whether a command can be executed</param>
		public BoundRelayCommand(INotifyPropertyChanged propertyDeclarer, string propertyName, Action<object> execute, Func<bool> canExecute)
		{
			if (propertyDeclarer == null)
				throw new ArgumentNullException("propertyDeclarer");

			if (propertyName == null)
				throw new ArgumentNullException("propertyName");

			if (execute == null)
				throw new ArgumentNullException("execute");

			if (canExecute == null)
				throw new ArgumentNullException("canExecute");

			_execute = execute;
			_canExecute = canExecute;
			_propertyName = propertyName;

			propertyDeclarer.PropertyChanged += propertyDeclarer_PropertyChanged;
		}

		#region ICommand Members

		/// <see cref="ICommand.CanExecute"/>
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute();
		}

		/// <see cref="ICommand.Execute"/>
		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		/// <see cref="ICommand.CanExecuteChanged"/>
		public event EventHandler CanExecuteChanged;

		private void OnCanExecuteChanged()
		{
			var localEvent = CanExecuteChanged;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		#endregion

		void propertyDeclarer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == _propertyName)
				OnCanExecuteChanged();
		}

		private readonly Action<object> _execute;
		private readonly Func<bool> _canExecute;
		private readonly string _propertyName;
	}
}