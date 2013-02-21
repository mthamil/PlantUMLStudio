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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using Utilities.Reflection;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose ability to execute depends on the value of a property from each of a collection of multiple
	/// objects. 
	/// </summary>
	/// <typeparam name="TCollectionSource">The type of object that owns the collection of objects, typically the class that instantiates the command</typeparam>
	/// <typeparam name="TPropertySource">The type of object within the collection that provides the property a command is dependent on</typeparam>
	/// <typeparam name="TCollection">The type of collection containing the objects a command is dependent on</typeparam>
	public abstract class ChildPropertyBoundCommandBase<TCollectionSource, TPropertySource, TCollection> : ICommand
		where TPropertySource : INotifyPropertyChanged
		where TCollection : IEnumerable<TPropertySource>
	{
		/// <summary>
		/// Initializes a new command.
		/// </summary>
		/// <param name="parent">An object that provides the collection of objects the command depends on</param>
		/// <param name="collectionExpression">The property on the parent object that provides the collection</param>
		/// <param name="childPropertyExpression">The child object property that that determines whether the command can execute</param>
		/// <param name="execute">The operation to execute</param>
		protected ChildPropertyBoundCommandBase(TCollectionSource parent, Expression<Func<TCollectionSource, TCollection>> collectionExpression, 
			Expression<Func<TPropertySource, bool>> childPropertyExpression, Action<object> execute)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			if (collectionExpression == null)
				throw new ArgumentNullException("collectionExpression");

			if (childPropertyExpression == null)
				throw new ArgumentNullException("childPropertyExpression");

			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;

			var collectionFunc = collectionExpression.Compile();
			_collectionGetter = () => collectionFunc(parent);

			var collection = GetCollection();

			var notifyingCollection = collection as INotifyCollectionChanged;
			if (notifyingCollection != null)
				WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>.AddHandler(notifyingCollection, "CollectionChanged", collection_CollectionChanged);

			foreach (var existingChild in collection)
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(existingChild, "PropertyChanged", item_PropertyChanged);

			var childProperty = Reflect.PropertyOf(typeof(TPropertySource), childPropertyExpression);
			_childPropertyName = childProperty.Name;
		}

		#region ICommand Members

		/// <see cref="ICommand.CanExecute"/>
		public bool CanExecute(object parameter)
		{
			return CanExecutePredicate();
		}

		/// <see cref="ICommand.Execute"/>
		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		/// <see cref="ICommand.CanExecuteChanged"/>
		public event EventHandler CanExecuteChanged;

		private void OnCanExecuteChanged()
		{
			var localEvent = CanExecuteChanged;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		#endregion ICommand Members

		protected TCollection GetCollection()
		{
			return _collectionGetter();
		}

		protected Func<bool> CanExecutePredicate { get; set; }

		void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (var removedItem in e.OldItems.Cast<TPropertySource>())
					WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(removedItem, "PropertyChanged", item_PropertyChanged);
			}

			if (e.NewItems != null)
			{
				foreach (var newItem in e.NewItems.Cast<TPropertySource>())
					WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(newItem, "PropertyChanged", item_PropertyChanged);
			}
		}

		void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == _childPropertyName)
			{
				if (_collectionGetter().Contains((TPropertySource)sender))
					OnCanExecuteChanged();
			}
		}

		private readonly Action<object> _execute;
		private readonly string _childPropertyName;
		private readonly Func<TCollection> _collectionGetter;
	}
}