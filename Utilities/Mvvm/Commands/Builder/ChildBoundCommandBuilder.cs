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
using Utilities.Reflection;

namespace Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that aids in creating commands who abilituy to execute depends on properties of a child collection's
	/// objects.
	/// </summary>
	/// <typeparam name="TParent">The type of parent that owns the child collection</typeparam>
	/// <typeparam name="TChild">The type of object whose properties the parent depends on</typeparam>
	public class ChildBoundCommandBuilder<TParent, TChild>
		where TChild : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="ChildBoundCommandBuilder{TParent,TChild}"/>.
		/// </summary>
		/// <param name="parent">The parent object</param>
		/// <param name="collection">An expression referencing a child collection</param>
		public ChildBoundCommandBuilder(TParent parent, Expression<Func<TParent, IEnumerable<TChild>>> collection)
		{
			_parent = parent;

			var collectionFunc = collection.Compile();
			_collectionGetter = () => collectionFunc(parent);
		}

		/// <summary>
		/// Defines a condition using a property of the child objects that, when true, allows a command to execute.
		/// </summary>
		/// <param name="aggregatePredicate">
		/// The condition that determines whether a command can execute. This predicate
		/// must make use of a property of a child object.
		/// </param>
		/// <returns>A builder that allows specification of the command operation</returns>
		public ICommandCompleter When(Expression<Func<IEnumerable<TChild>, bool>> aggregatePredicate)
		{
			if (aggregatePredicate == null)
				throw new ArgumentNullException("aggregatePredicate");

			var propertyAggregationFunc = aggregatePredicate.Compile();
			Func<bool> canExecute = () => propertyAggregationFunc(_collectionGetter());

			return new ChildBoundCommandCompleter<TParent, TChild>(_parent, _collectionGetter, FindChildProperty(aggregatePredicate), canExecute);
		}

		/// <summary>
		/// Specifies that a parent property of the object that owns a command determines whether it can execute.
		/// </summary>
		/// <param name="parentProperty">The property that determines whether a command can execute</param>
		/// <returns>A builder that enables setting the child property the parent property's value depends on</returns>
		public DependentChildPropertyCommandBuilder<TParent, TChild> Where(Expression<Func<TParent, bool>> parentProperty)
		{
			if (parentProperty == null)
				throw new ArgumentNullException("parentProperty");

			if (Reflect.PropertyOf(parentProperty).SetMethod != null)
				throw new ArgumentException("Parent property must not have a setter.");

			return new DependentChildPropertyCommandBuilder<TParent, TChild>(_parent, _collectionGetter, parentProperty);
		}

		/// <summary>
		/// Attempts to extract a child property expression from a collection predicate expression.
		/// </summary>
		private static Expression<Func<TPropertySource, bool>> FindChildProperty<TPropertySource>(Expression<Func<IEnumerable<TPropertySource>, bool>> aggregatePredicate)
		{
			var methodCall = (MethodCallExpression)aggregatePredicate.Body;
			var childPropertyExpression = methodCall.Arguments.OfType<Expression<Func<TPropertySource, bool>>>().Single();
			return childPropertyExpression;
		}

		private readonly TParent _parent;
		private readonly Func<IEnumerable<TChild>> _collectionGetter;
	}
}