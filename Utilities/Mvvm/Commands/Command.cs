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
		/// <param name="canExecuteProperty">An expression referencing a boolean property that determines whether the command can be executed</param>
		/// <param name="execute">The operation to execute</param>
		public static ICommand Bound<TSource>(TSource propertyDeclarer, Expression<Func<TSource, bool>> canExecuteProperty, Action execute)
			where TSource : INotifyPropertyChanged
		{
			return Bound(propertyDeclarer, canExecuteProperty, _ => execute());
		}

		/// <summary>
		/// Creates a command whose ability to execute is bound to a property's value.
		/// </summary>
		/// <param name="propertyDeclarer">The object that declares the property that triggers a change in command execution status</param>
		/// <param name="canExecuteProperty">An expression referencing a boolean property that determines whether the command can be executed</param>
		/// <param name="execute">The operation to execute</param>
		public static ICommand Bound<TSource>(TSource propertyDeclarer, Expression<Func<TSource, bool>> canExecuteProperty, Action<object> execute)
			where TSource : INotifyPropertyChanged
		{
			if (propertyDeclarer == null)
				throw new ArgumentNullException("propertyDeclarer");

			if (execute == null)
				throw new ArgumentNullException("execute");

			if (canExecuteProperty == null)
				throw new ArgumentNullException("canExecuteProperty");

			var property = Reflect.PropertyOf(typeof(TSource), canExecuteProperty);
			Func<TSource, bool> func = canExecuteProperty.Compile();
			Func<bool> canExecute = () => func(propertyDeclarer);

			return new BoundRelayCommand(propertyDeclarer, property.Name, execute, canExecute);
		}

		/// <summary>
		/// Creates a command whose ability to execute depends on the properties of multiple objects.
		/// </summary>
		/// <param name="parent">An object that declares the collection property whose items trigger a change in whether the command can execute</param>
		/// <param name="collectionExpression">A collection whose items trigger a change in whether the command can execute</param>
		/// <param name="propertyAggregationExpression">
		///     A predicate that aggregates the values of a property from each child item.
		///     This predicate MUST make use of a boolean property of the child type.
		/// </param>
		/// <param name="execute">The operation to execute</param>
		public static ICommand BoundAggregate<TCollectionSource, TPropertySource>(TCollectionSource parent,
			Expression<Func<TCollectionSource, IEnumerable<TPropertySource>>> collectionExpression,
			Expression<Func<IEnumerable<TPropertySource>, bool>> propertyAggregationExpression, 
			Action<object> execute)
				where TPropertySource : INotifyPropertyChanged
		{
			if (propertyAggregationExpression == null)
				throw new ArgumentNullException("propertyAggregationExpression");

			return new AggregateBoundRelayCommand<TCollectionSource, TPropertySource, IEnumerable<TPropertySource>>(
				parent, collectionExpression, propertyAggregationExpression, execute);
		}
	}
}