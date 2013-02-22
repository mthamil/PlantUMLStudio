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
using System.Windows.Input;
using Utilities.Reflection;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// Contains factory methods for commands.
	/// </summary>
	public static class Command
	{
		/// <summary>
		/// Creates a command whose ability to execute is bound to a property's value.
		/// </summary>
		/// <param name="propertyDeclarer">The object that declares the property that triggers a change in command execution status</param>
		/// <param name="predicateProperty">An expression referencing a boolean property that determines whether the command can be executed</param>
		/// <param name="operation">The operation to execute</param>
		public static ICommand Bound<TSource>(TSource propertyDeclarer, Expression<Func<TSource, bool>> predicateProperty, Action operation)
			where TSource : INotifyPropertyChanged
		{
			return Bound(propertyDeclarer, predicateProperty, _ => operation());
		}

		/// <summary>
		/// Creates a command whose ability to execute is bound to a property's value.
		/// </summary>
		/// <param name="propertyDeclarer">The object that declares the property that triggers a change in command execution status</param>
		/// <param name="predicateProperty">An expression referencing a boolean property that determines whether the command can be executed</param>
		/// <param name="operation">The operation to execute</param>
		public static ICommand Bound<TSource>(TSource propertyDeclarer, Expression<Func<TSource, bool>> predicateProperty, Action<object> operation)
			where TSource : INotifyPropertyChanged
		{
			if (propertyDeclarer == null)
				throw new ArgumentNullException("propertyDeclarer");

			if (operation == null)
				throw new ArgumentNullException("operation");

			if (predicateProperty == null)
				throw new ArgumentNullException("predicateProperty");

			var property = Reflect.PropertyOf(typeof(TSource), predicateProperty);
			Func<TSource, bool> func = predicateProperty.Compile();
			Func<bool> canExecute = () => func(propertyDeclarer);

			return new BoundRelayCommand(propertyDeclarer, property.Name, operation, canExecute);
		}

		/// <summary>
		/// Creates a command whose ability to execute depends on the properties of multiple objects.
		/// </summary>
		/// <param name="parent">An object that declares the collection property whose items trigger a change in whether the command can execute</param>
		/// <param name="collection">A collection whose items trigger a change in whether the command can execute</param>
		/// <param name="aggregatePredicate">
		///     A predicate that aggregates the values of a property from each child item.
		///     This predicate MUST make use of a boolean property of the child type.
		/// </param>
		/// <param name="operation">The operation to execute</param>
		public static ICommand BoundAggregate<TCollectionSource, TPropertySource>(TCollectionSource parent,
		                                                                          Expression<Func<TCollectionSource, IEnumerable<TPropertySource>>> collection,
		                                                                          Expression<Func<IEnumerable<TPropertySource>, bool>> aggregatePredicate,
		                                                                          Action<object> operation)
			where TPropertySource :INotifyPropertyChanged
		{
			if (aggregatePredicate == null)
				throw new ArgumentNullException("aggregatePredicate");

			ChildPropertyBoundCommand<TCollectionSource, TPropertySource> command = null;

			var propertyAggregationFunc = aggregatePredicate.Compile();
			Func<bool> canExecute = () => propertyAggregationFunc(command.Collection);

			command = new ChildPropertyBoundCommand<TCollectionSource, TPropertySource>(
				parent, collection, FindChildProperty(aggregatePredicate), canExecute,  operation);
			return command;
		}

		private static Expression<Func<TPropertySource, bool>> FindChildProperty<TPropertySource>(Expression<Func<IEnumerable<TPropertySource>, bool>> propertyAggregationExpression)
		{
			var methodCall = (MethodCallExpression)propertyAggregationExpression.Body;
			var childPropertyExpression = methodCall.Arguments.OfType<Expression<Func<TPropertySource, bool>>>().Single();
			return childPropertyExpression;
		}

		/// <summary>
		/// Creates a command whose CanExecute is bound to a property on a parent object while
		/// CanExecuteChanged is bound to each value of a given property of a collection of items.
		/// </summary>
		/// <param name="parent">An object that declares the collection property whose items trigger a change in whether the command can execute</param>
		/// <param name="collection">A collection whose items trigger a change in whether the command can execute</param>
		/// <param name="parentProperty">The parent property that determines whether a command can execute</param>
		/// <param name="childProperty">The child property that triggers a change in <paramref name="parentProperty"/></param>
		/// <param name="operation">The operation to execute</param>
		public static ICommand BoundDependent<TCollectionSource, TPropertySource>(TCollectionSource parent,
		                                                                          Expression<Func<TCollectionSource, IEnumerable<TPropertySource>>> collection,
		                                                                          Expression<Func<TCollectionSource, bool>> parentProperty,
		                                                                          Expression<Func<TPropertySource, bool>> childProperty, 
																				  Action<object> operation)
			where TPropertySource : INotifyPropertyChanged
		{
			var parentPropertyGetter = parentProperty.Compile();
			Func<bool> canExecute = () => parentPropertyGetter(parent);

			return new ChildPropertyBoundCommand<TCollectionSource, TPropertySource>(
				parent, collection, childProperty, canExecute, operation);
		}
	}
}