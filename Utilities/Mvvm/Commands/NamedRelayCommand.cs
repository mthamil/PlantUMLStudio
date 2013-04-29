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
using System.ComponentModel;
using Utilities.PropertyChanged;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command that has a name.
	/// </summary>
	public class NamedRelayCommand : RelayCommand, INotifyPropertyChanged
	{
		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public NamedRelayCommand(Action execute)
			: this(execute, null) { }

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public NamedRelayCommand(Action execute, Func<bool> canExecute)
			: base(execute, canExecute)
		{
			_name = Property.New(this, p => p.Name, OnPropertyChanged);
		}

		/// <summary>
		/// The name of the command.
		/// </summary>
		public string Name
		{
			get { return _name.Value; }
			set { _name.Value = value; }
		}

		#region Implementation of INotifyPropertyChanged

		/// <see cref="INotifyPropertyChanged.PropertyChanged"/>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			var localEvent = PropertyChanged;
			if (localEvent != null)
				localEvent(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private readonly Property<string> _name;
	}

	/// <summary>
	/// A command with a name.
	/// </summary>
	/// <typeparam name="T">The type of parameter to be passed to the command</typeparam>
	public class NamedRelayCommand<T> : RelayCommand<T>, INotifyPropertyChanged
	{
		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public NamedRelayCommand(Action<T> execute)
			: this(execute, null) { }

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public NamedRelayCommand(Action<T> execute, Predicate<T> canExecute)
			: base(execute, canExecute)
		{
			_name = Property.New(this, p => p.Name, OnPropertyChanged);
		}

		/// <summary>
		/// The name of the command.
		/// </summary>
		public string Name
		{
			get { return _name.Value; }
			set { _name.Value = value; }
		}

		#region Implementation of INotifyPropertyChanged

		/// <see cref="INotifyPropertyChanged.PropertyChanged"/>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			var localEvent = PropertyChanged;
			if (localEvent != null)
				localEvent(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private readonly Property<string> _name;
	}
}