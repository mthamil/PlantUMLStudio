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
	public class ChildPropertyBoundCommand<TCollectionSource, TPropertySource> : ICommand
		where TPropertySource : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new command whose ability to execute depends on the properties of multiple objects.
		/// </summary>
		/// <param name="parent">An object that provides the collection of objects the command depends on</param>
		/// <param name="collectionGetter">A function that provides the collection</param>
		/// <param name="childPropertyExpression">The child object property that that determines whether the command can execute</param>
		/// <param name="canExecute">Function used to determine whether the command can execute</param>
		/// <param name="execute">The operation to execute</param>
		public ChildPropertyBoundCommand(TCollectionSource parent, Func<IEnumerable<TPropertySource>> collectionGetter, 
			Expression<Func<TPropertySource, bool>> childPropertyExpression, Func<bool> canExecute, Action<object> execute)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			if (collectionGetter == null)
				throw new ArgumentNullException("collectionGetter");

			if (childPropertyExpression == null)
				throw new ArgumentNullException("childPropertyExpression");

			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;

			_collectionGetter = collectionGetter;

			var collection = Collection;

			var notifyingCollection = collection as INotifyCollectionChanged;
			if (notifyingCollection != null)
				WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>.AddHandler(notifyingCollection, "CollectionChanged", collection_CollectionChanged);

			foreach (var existingChild in collection)
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(existingChild, "PropertyChanged", item_PropertyChanged);

			var childProperty = Reflect.PropertyOf(childPropertyExpression);
			_childPropertyName = childProperty.Name;
		}

		#region ICommand Members

		/// <see cref="ICommand.CanExecute"/>
		public bool CanExecute(object parameter)
		{
			return _canExecute();
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

		/// <summary>
		/// Gets the collection of items the command depends on.
		/// </summary>
		internal IEnumerable<TPropertySource> Collection
		{
			get { return _collectionGetter(); }
		}

		void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (var removedItem in e.OldItems.Cast<INotifyPropertyChanged>())
					WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(removedItem, "PropertyChanged", item_PropertyChanged);
			}

			if (e.NewItems != null)
			{
				foreach (var newItem in e.NewItems.Cast<INotifyPropertyChanged>())
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
		private readonly Func<bool> _canExecute;
		private readonly string _childPropertyName;
		private readonly Func<IEnumerable<TPropertySource>> _collectionGetter;
	}
}