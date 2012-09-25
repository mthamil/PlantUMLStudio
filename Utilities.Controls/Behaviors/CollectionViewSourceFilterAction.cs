using System;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Action that, in conjunction with CollectionViewSources.FilterProperty and the CollectionViewSource.Filter event, 
	/// provides a bindable CollectionViewSource filter.
	/// </summary>
	public class CollectionViewSourceFilterAction : TriggerAction<CollectionViewSource>
	{
		/// <see cref="System.Windows.Interactivity.TriggerAction.Invoke"/>
		protected override void Invoke(object parameter)
		{
			var e = (FilterEventArgs)parameter;
			if (Filter != null)
				e.Accepted = Filter(e.Item);
		}

		/// <summary>
		/// Gets or sets the collection filter.
		/// </summary>
		public Predicate<object> Filter
		{
			get { return CollectionViewSources.GetFilter(AssociatedObject); }
			set { CollectionViewSources.SetFilter(AssociatedObject, value); }
		}
	}
}