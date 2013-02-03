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
using System.Collections;
using System.Collections.Specialized;
using System.Windows;

namespace Utilities.Collections
{
	/// <summary>
	/// Synchronizes two collections that implement INotifyCollectionChanged.
	/// </summary>
	public class CollectionMirror : DisposableBase, IWeakEventListener
	{
		/// <summary>
		/// Initializes a new collection mirror.
		/// </summary>
		/// <param name="source">The source collection</param>
		/// <param name="target">The target collection</param>
		public CollectionMirror(IList source, IList target)
			: this(source, target, obj => obj, obj => obj) { }

		/// <summary>
		/// Initializes a new collection mirror with transformations.
		/// </summary>
		/// <param name="source">The source collection</param>
		/// <param name="target">The target collection</param>
		/// <param name="sourceToTarget">A mapping from source items to target items</param>
		/// <param name="targetToSource">A mapping from target items to source items</param>
		public CollectionMirror(IList source, IList target, Func<object, object> sourceToTarget, Func<object, object> targetToSource)
		{
			_source = source;
			_target = target;
			_sourceToTarget = sourceToTarget;
			_targetToSource = targetToSource;

			Synchronize(_source, _target, _sourceToTarget);
			Subscribe(_source);
		}

		private void Unsubscribe(ICollection collection)
		{
			var notifierCollection = collection as INotifyCollectionChanged;
			if (notifierCollection != null)
				CollectionChangedEventManager.RemoveListener(notifierCollection, this);
		}

		private void Subscribe(ICollection collection)
		{
			var notifierCollection = collection as INotifyCollectionChanged;
			if (notifierCollection != null)
				CollectionChangedEventManager.AddListener(notifierCollection, this);
		}

		/// <summary>
		/// Ensures that the target collection matches the source collection.
		/// </summary>
		/// <remarks>The current implementation first clears the target, removing all existing items</remarks>
		private void Synchronize(IList source, IList target, Func<object, object> mapping)
		{
			using (SuppressChangeEvents(target))
			{
				target.Clear();
				foreach (var item in source)
					target.Add(mapping(item));
			}
		}

		private void PropagateAdd(IList collection, Func<object, object> mapping, NotifyCollectionChangedEventArgs args)
		{
			using (SuppressChangeEvents(collection))
			{
				for (int i = 0; i < args.NewItems.Count; i++)
				{
					int insertionIndex = i + args.NewStartingIndex;
					var newItem = args.NewItems[i];

					if (insertionIndex > collection.Count)
						collection.Add(mapping(newItem));
					else
						collection.Insert(insertionIndex, mapping(newItem));
				}
			}
		}

		private void PropagateRemove(IList collection, NotifyCollectionChangedEventArgs args)
		{
			using (SuppressChangeEvents(collection))
			{
				for (int i = args.OldItems.Count - 1; i >= 0; i--)
				{
					int removalIndex = i + args.OldStartingIndex;
					collection.RemoveAt(removalIndex);
				}
			}
		}

		private void PropagateReplace(IList collection, Func<object, object> mapping, NotifyCollectionChangedEventArgs args)
		{
			using (SuppressChangeEvents(collection))
			{
				for (int i = 0; i < args.NewItems.Count; i++)
				{
					int replacementIndex = i + args.NewStartingIndex;
					collection[replacementIndex] = mapping(args.NewItems[replacementIndex]);
				}
			}
		}

		private void PropagateMove(IList collection, Func<object, object> mapping, NotifyCollectionChangedEventArgs args)
		{
			PropagateRemove(collection, args);
			PropagateAdd(collection, mapping, args);
		}

		private void PropagateReset(IList collection)
		{
			var source = collection == _source ? _target : _source;
			var mapping = collection == source ? _sourceToTarget : _targetToSource;
			Synchronize(source, collection, mapping);
		}

		#region Implementation of IWeakEventListener

		/// <see cref="IWeakEventListener.ReceiveWeakEvent"/>
		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			var changeArgs = (NotifyCollectionChangedEventArgs)e;
			var changedCollection = (IList)sender;

			var collectionToUpdate = changedCollection == _source ? _target : _source;
			var mapping = changedCollection == _source ? _sourceToTarget : _targetToSource;

			switch (changeArgs.Action)
			{
				case NotifyCollectionChangedAction.Add:
					PropagateAdd(collectionToUpdate, mapping, changeArgs);
					break;
				case NotifyCollectionChangedAction.Remove:
					PropagateRemove(collectionToUpdate, changeArgs);
					break;
				case NotifyCollectionChangedAction.Replace:
					PropagateReplace(collectionToUpdate, mapping, changeArgs);
					break;
				case NotifyCollectionChangedAction.Move:
					PropagateMove(collectionToUpdate, mapping, changeArgs);
					break;
				case NotifyCollectionChangedAction.Reset:
					PropagateReset(collectionToUpdate);
					break;
			}

			return true;
		}

		#endregion

		#region Implementation of DisposableBase

		/// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing()
		{
			Unsubscribe(_source);
			Unsubscribe(_target);
		}

		#endregion

		private readonly IList _source;
		private readonly IList _target;
		private readonly Func<object, object> _sourceToTarget;
		private readonly Func<object, object> _targetToSource;

		/// <summary>
		/// Temporarily suppresses handling of change events for a collection.
		/// </summary>
		private IDisposable SuppressChangeEvents(ICollection collection)
		{
			return new ChangeEventSuppressionToken(collection, this);
		}

		private class ChangeEventSuppressionToken : IDisposable
		{
			public ChangeEventSuppressionToken(ICollection collection, CollectionMirror listener)
			{
				_collection = collection;
				_listener = listener;

				_listener.Unsubscribe(_collection);
			}

			#region Implementation of IDisposable

			public void Dispose()
			{
				_listener.Subscribe(_collection);
			}

			#endregion

			private readonly ICollection _collection;
			private readonly CollectionMirror _listener;
		}
	}
}