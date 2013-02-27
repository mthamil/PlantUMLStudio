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
	/// Base class for commands whose ability to execute is bound to a property on some object.
	/// </summary>
	public abstract class BoundRelayCommandBase : ICommand
	{
		protected BoundRelayCommandBase(INotifyPropertyChanged propertyDeclarer, string propertyName, Func<bool> canExecute)
		{
			if (propertyDeclarer == null)
				throw new ArgumentNullException("propertyDeclarer");

			if (propertyName == null)
				throw new ArgumentNullException("propertyName");

			if (canExecute == null)
				throw new ArgumentNullException("canExecute");

			_propertyName = propertyName;
			_canExecute = canExecute;

			propertyDeclarer.PropertyChanged += propertyDeclarer_PropertyChanged;
		}

		/// <see cref="ICommand.CanExecute"/>
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute();
		}

		/// <see cref="ICommand.Execute"/>
		public abstract void Execute(object parameter);

		/// <see cref="ICommand.CanExecuteChanged"/>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
		/// </summary>
		protected void OnCanExecuteChanged()
		{
			var localEvent = CanExecuteChanged;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		private void propertyDeclarer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == _propertyName)
				OnCanExecuteChanged();
		}

		private readonly Func<bool> _canExecute;
		private readonly string _propertyName;
	}
}