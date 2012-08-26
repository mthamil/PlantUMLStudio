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
	public abstract class ChildPropertyBoundCommandBase<TCollectionSource, TPropertySource, TCollection> : ICommand
		where TPropertySource : INotifyPropertyChanged
		where TCollection : IEnumerable<TPropertySource>
	{
		protected ChildPropertyBoundCommandBase(Action<object> execute,
		                                  Expression<Func<TCollectionSource, TCollection>> collectionExpression,
		                                  Expression<Func<TPropertySource, bool>> childPropertyExpression, TCollectionSource parent)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			if (collectionExpression == null)
				throw new ArgumentNullException("collectionExpression");

			if (childPropertyExpression == null)
				throw new ArgumentNullException("childPropertyExpression");

			if (parent == null)
				throw new ArgumentNullException("parent");

			_execute = execute;

			var collectionFunc = collectionExpression.Compile();
			_collectionGetter = () => collectionFunc(parent);

			var collection = GetCollection();
			WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>.AddHandler((INotifyCollectionChanged)collection, "CollectionChanged", collection_CollectionChanged);
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