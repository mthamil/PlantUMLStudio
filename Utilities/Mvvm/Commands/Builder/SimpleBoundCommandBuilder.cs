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
using System.Linq.Expressions;
using System.Windows.Input;
using Utilities.Reflection;

namespace Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that aids in creating a <see cref="ICommand"/> whose ability to execute
	/// is determined by a property.
	/// </summary>
	/// <typeparam name="TSource">The type of object for which a command is being built</typeparam>
	public class SimpleBoundCommandBuilder<TSource> : ICommandCompleter where TSource : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="SimpleBoundCommandBuilder{TSource}"/>.
		/// </summary>
		/// <param name="source">The object that declares the property the command is bound to</param>
		/// <param name="predicateProperty">The boolean property that determines whether a command can execute</param>
		public SimpleBoundCommandBuilder(TSource source, Expression<Func<TSource, bool>> predicateProperty)
		{
			_source = source;
			_predicateProperty = predicateProperty;
		}

		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The parameterless operation to be executed</param>
		/// <returns>A new command</returns>
		public ICommand Executes(Action operation)
		{
			return Executes(_ => operation());
		}

		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The operation to be executed</param>
		/// <returns>A new command</returns>
		public ICommand Executes(Action<object> operation)
		{
			if (operation == null)
				throw new ArgumentNullException("operation");

			var property = Reflect.PropertyOf(typeof(TSource), _predicateProperty);
			Func<TSource, bool> func = _predicateProperty.Compile();
			Func<bool> canExecute = () => func(_source);

			return new BoundRelayCommand(_source, property.Name, operation, canExecute);
		}

		private readonly TSource _source;
		private readonly Expression<Func<TSource, bool>> _predicateProperty;
	}
}