using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Action that, in conjunction with the CollectionViewSource.Filter event, 
	/// provides a bindable CollectionViewSource filter.
	/// </summary>
	public class CollectionViewSourceFilter : TriggerAction<CollectionViewSource>
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
			get { return (Predicate<object>)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}

		/// <summary>
		/// The Filter dependency property.
		/// </summary>
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register(
				"Filter",
				typeof(Predicate<object>),
				typeof(CollectionViewSourceFilter),
				new UIPropertyMetadata(null));
	}
}