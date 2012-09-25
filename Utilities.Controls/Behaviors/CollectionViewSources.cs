using System;
using System.Windows;
using System.Windows.Data;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Provides CollectionViewSource attached properties.
	/// </summary>
	public static class CollectionViewSources
	{
		/// <summary>
		/// Gets the property value.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
		public static Predicate<object> GetFilter(CollectionViewSource collectionViewSource)
		{
			return (Predicate<object>)collectionViewSource.GetValue(FilterProperty);
		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		public static void SetFilter(CollectionViewSource collectionViewSource, Predicate<object> value)
		{
			collectionViewSource.SetValue(FilterProperty, value);
		}

		/// <summary>
		/// The Filter attached property.
		/// </summary>
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.RegisterAttached(
				"Filter",
				typeof(Predicate<object>),
				typeof(CollectionViewSources),
				new UIPropertyMetadata(null));
	}
}