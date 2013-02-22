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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Interface for an object that aids in creating complex <see cref="ICommand"/>s.
	/// </summary>
	/// <typeparam name="TSource">The type of object for which a command is being built</typeparam>
	public interface ICommandBuilder<TSource> where TSource :INotifyPropertyChanged
	{
		/// <summary>
		/// Indicates that a command is bound to the given boolean property's value to determine
		/// whether it can execute.
		/// </summary>
		/// <param name="predicateProperty">The boolean property that determines whether a command can execute</param>
		/// <returns>A builder for a command bound to a property</returns>
		ICommandCompleter DependsOn(Expression<Func<TSource, bool>> predicateProperty);

		/// <summary>
		/// Indicates that a command is bound to a property of each object of a child collection to determine
		/// whether it can execute.
		/// </summary>
		/// <typeparam name="TChild">The type of child object</typeparam>
		/// <param name="collection">The collection whose items determine whether a command can execute</param>
		/// <returns>A builder for a command bound to a child property</returns>
		ChildBoundCommandBuilder<TSource, TChild> DependsOnCollection<TChild>(Expression<Func<TSource, IEnumerable<TChild>>> collection)
			where TChild : INotifyPropertyChanged;
	}
}