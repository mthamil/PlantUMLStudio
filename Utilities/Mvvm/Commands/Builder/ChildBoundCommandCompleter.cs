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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that completes construction of a command that depends on a child collection.
	/// </summary>
	/// <typeparam name="TParent">The type of parent object</typeparam>
	/// <typeparam name="TChild">The type of child object the parent depends on</typeparam>
	public class ChildBoundCommandCompleter<TParent, TChild> : ICommandCompleter where TChild : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="ChildBoundCommandCompleter{TParent,TChild}"/>.
		/// </summary>
		/// <param name="parent">The parent object</param>
		/// <param name="collectionGetter">Function that retrieves the collection whose items determine whether a command can execute</param>
		/// <param name="childProperty">A child property that the parent is somehow dependent upon for determining whether a command can execute</param>
		/// <param name="canExecute">The actual predicate that determines whether a command can execute</param>
		public ChildBoundCommandCompleter(
			TParent parent, 
			Func<IEnumerable<TChild>> collectionGetter, 
			Expression<Func<TChild, bool>> childProperty,
			Func<bool> canExecute)
		{
			_parent = parent;
			_collectionGetter = collectionGetter;
			_childProperty = childProperty;
			_canExecute = canExecute;
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
			return new ChildPropertyBoundCommand<TParent, TChild>(
				_parent, _collectionGetter, _childProperty, _canExecute, operation);
		}

		private readonly TParent _parent;
		private readonly Func<IEnumerable<TChild>> _collectionGetter;
		private readonly Expression<Func<TChild, bool>> _childProperty;
		private readonly Func<bool> _canExecute;
	}
}