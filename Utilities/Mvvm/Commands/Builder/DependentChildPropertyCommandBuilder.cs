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

namespace Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that allows specification of a child property that a parent property's
	/// value depends on.
	/// </summary>
	/// <typeparam name="TParent">The type of the parent object</typeparam>
	/// <typeparam name="TChild">The type of the child objects that the parent depends on</typeparam>
	public class DependentChildPropertyCommandBuilder<TParent, TChild> where TChild : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="DependentChildPropertyCommandBuilder{TParent,TChild}"/>.
		/// </summary>
		/// <param name="parent">The parent object</param>
		/// <param name="collectionGetter">Function that retrieves the collection whose items determine whether a command can execute</param>
		/// <param name="parentProperty">The property on the parent object that depends on the children</param>
		public DependentChildPropertyCommandBuilder(
			TParent parent, 
			Func<IEnumerable<TChild>> collectionGetter,
			Expression<Func<TParent, bool>> parentProperty)
		{
			_parent = parent;
			_collectionGetter = collectionGetter;
			_parentProperty = parentProperty;
		}

		/// <summary>
		/// Specifies the child property that the parent property's value depends on.
		/// </summary>
		/// <param name="childProperty">A child property that the parent property's value depends on</param>
		/// <returns>A builder that allows specification of the command operation</returns>
		public ICommandCompleter DependsOn(Expression<Func<TChild, bool>> childProperty)
		{
			var parentPropertyGetter = _parentProperty.Compile();
			Func<bool> canExecute = () => parentPropertyGetter(_parent);

			return new ChildBoundCommandCompleter<TParent, TChild>(_parent, _collectionGetter, childProperty, canExecute);
		}

		private readonly TParent _parent;
		private readonly Func<IEnumerable<TChild>> _collectionGetter;
		private readonly Expression<Func<TParent, bool>> _parentProperty;
	}
}