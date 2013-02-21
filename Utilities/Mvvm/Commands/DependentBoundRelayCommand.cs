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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. CanExecute is bound to a property on a parent object while
	/// CanExecuteChanged is bound to each value of a given property of a collection of items.
	/// </summary>
	public class DependentBoundRelayCommand<TCollectionSource, TPropertySource, TCollection> : ChildPropertyBoundCommandBase<TCollectionSource, TPropertySource, TCollection>
		where TPropertySource : INotifyPropertyChanged
		where TCollection : IEnumerable<TPropertySource>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="collectionExpression">The collection whose items trigger a change in whether the command can execute</param>
		/// <param name="parentPropertyExpression">The parent property that determines whether a command can execute</param>
		/// <param name="childPropertyExpression">The child property that triggers a change in <paramref name="parentPropertyExpression"/></param>
		/// <param name="parent">The object that declares the collection property whose items trigger a change in whether the command can execute</param>
		public DependentBoundRelayCommand(Action<object> execute, Expression<Func<TCollectionSource, TCollection>> collectionExpression,
		                                  Expression<Func<TCollectionSource, bool>> parentPropertyExpression, Expression<Func<TPropertySource, bool>> childPropertyExpression, 
										  TCollectionSource parent)
			: base(parent, collectionExpression, childPropertyExpression, execute)
		{
			var parentPropertyGetter = parentPropertyExpression.Compile();
			CanExecutePredicate = () => parentPropertyGetter(parent);
		}
	}
}