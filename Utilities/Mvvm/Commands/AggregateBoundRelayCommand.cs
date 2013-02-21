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
using System.Linq;
using System.Linq.Expressions;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. CanExecute and CanExecuteChanged are bound to 
	/// each value of a given property of a collection of items.
	/// </summary>
	public class AggregateBoundRelayCommand<TCollectionSource, TPropertySource, TCollection> : ChildPropertyBoundCommandBase<TCollectionSource, TPropertySource, TCollection>
		where TPropertySource : INotifyPropertyChanged
		where TCollection : IEnumerable<TPropertySource>
	{
		/// <summary>
		/// Creates a command whose ability to execute depends on the properties of multiple objects.
		/// </summary>
		/// <param name="parent">The object that declares the collection property whose items trigger a change in whether the command can execute</param>
		/// <param name="collectionExpression">The collection whose items trigger a change in whether the command can execute</param>
		/// <param name="propertyAggregationExpression">
		///     A predicate that aggregates the values of a property from each child item.
		///     This predicate MUST make use of a boolean property of the child type.
		/// </param>
		/// <param name="execute">The execution logic.</param>
		public AggregateBoundRelayCommand(TCollectionSource parent, Expression<Func<TCollectionSource, TCollection>> collectionExpression, 
										  Expression<Func<TCollection, bool>> propertyAggregationExpression, Action<object> execute)
			: base(parent, collectionExpression, GetChildProperty(propertyAggregationExpression), execute)
		{
			if (propertyAggregationExpression == null) 
				throw new ArgumentNullException("propertyAggregationExpression");

			var propertyAggregationFunc = propertyAggregationExpression.Compile();
			CanExecutePredicate = () => propertyAggregationFunc(GetCollection());
		}

		private static Expression<Func<TPropertySource, bool>> GetChildProperty(Expression<Func<TCollection, bool>> propertyAggregationExpression)
		{
			var methodCall = (MethodCallExpression)propertyAggregationExpression.Body;
			var childPropertyExpression = methodCall.Arguments.OfType<Expression<Func<TPropertySource, bool>>>().Single();
			return childPropertyExpression;
		}
	}
}