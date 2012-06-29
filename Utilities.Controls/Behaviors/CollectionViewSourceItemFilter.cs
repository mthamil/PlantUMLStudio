using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Allows binding a CollectionViewSource's Filter property.
	/// </summary>
	public static class CollectionViewSourceItemFilter
	{
		/// <summary>
		/// Gets the property value.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
		public static Predicate<object> GetItemFilter(CollectionViewSource collectionViewSource)
		{
			return (Predicate<object>)collectionViewSource.GetValue(ItemFilterProperty);
		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		public static void SetItemFilter(CollectionViewSource collectionViewSource, Predicate<object> value)
		{
			collectionViewSource.SetValue(ItemFilterProperty, value);
		}

		/// <summary>
		/// The ItemFilter dependency property.
		/// </summary>
		public static readonly DependencyProperty ItemFilterProperty =
			DependencyProperty.RegisterAttached(
			"ItemFilter",
			typeof(Predicate<object>),
			typeof(CollectionViewSourceItemFilter),
			new UIPropertyMetadata(null, OnItemFilterChanged));

		private static void OnItemFilterChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			var collectionViewSource = depObj as CollectionViewSource;
			if (collectionViewSource == null)
				return;

			if (!Equals(e.NewValue, e.OldValue))
			{
				var newFilter = (Predicate<object>)e.NewValue;

				// Remove any previous filter.
				ItemFilterBehavior oldBehavior;
				if (behaviors.TryGetValue(collectionViewSource, out oldBehavior))
				{
					oldBehavior.Unregister();
					behaviors.Remove(collectionViewSource);
				}

				if (newFilter != null)
					behaviors.Add(collectionViewSource, new ItemFilterBehavior(collectionViewSource, newFilter));
			}
		}

		private class ItemFilterBehavior
		{
			public ItemFilterBehavior(CollectionViewSource collectionViewSource, Predicate<object> filter)
			{
				_collectionViewSource = collectionViewSource;
				_filter = filter;
				_collectionViewSource.Filter += collectionViewSource_Filter;
			}

			void collectionViewSource_Filter(object sender, FilterEventArgs e)
			{
				e.Accepted = _filter(e.Item);
			}

			public void Unregister()
			{
				_collectionViewSource.Filter -= collectionViewSource_Filter;
			}

			private readonly CollectionViewSource _collectionViewSource;
			private readonly Predicate<object> _filter;
		}

		private static readonly IDictionary<CollectionViewSource, ItemFilterBehavior> behaviors = new ConcurrentDictionary<CollectionViewSource, ItemFilterBehavior>();
	}
}