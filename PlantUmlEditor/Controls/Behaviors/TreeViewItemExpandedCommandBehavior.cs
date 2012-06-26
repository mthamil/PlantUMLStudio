using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlantUmlEditor.Controls.Behaviors
{
	/// <summary>
	/// Provides an ICommand that executes when a TreeViewItem is expanded.
	/// </summary>
	public static class TreeViewItemExpandedCommandBehavior
	{
		/// <summary>
		/// Gets the expanded command for a TreeViewItem.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
		public static ICommand GetExpandedCommand(TreeViewItem treeViewItem)
		{
			return (ICommand)treeViewItem.GetValue(ExpandedCommandProperty);
		}

		/// <summary>
		/// Sets the expanded command for a TreeViewItem.
		/// </summary>
		public static void SetExpandedCommand(TreeViewItem treeViewItem, ICommand value)
		{
			treeViewItem.SetValue(ExpandedCommandProperty, value);
		}

		/// <summary>
		/// The command property.
		/// </summary>
		public static readonly DependencyProperty ExpandedCommandProperty =
			DependencyProperty.RegisterAttached(
			"ExpandedCommand",
			typeof(ICommand),
			typeof(TreeViewItemExpandedCommandBehavior),
			new UIPropertyMetadata(null, OnExpandedCommandChanged));

		static void OnExpandedCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			TreeViewItem treeViewItem = depObj as TreeViewItem;
			if (treeViewItem == null)
				return;

			var newCommand = e.NewValue as ICommand;
			if (newCommand == null)
				return;

			if ((e.NewValue != null) && (e.OldValue == null))
			{
				treeViewItem.Expanded += treeViewItem_Expanded;
			}
			else if ((e.NewValue == null) && (e.OldValue != null))
			{
				treeViewItem.Expanded -= treeViewItem_Expanded;
			}
		}

		static void treeViewItem_Expanded(object sender, RoutedEventArgs e)
		{
			// Only react to the Selected event raised by the TreeViewItem
			// whose IsSelected property was modified. Ignore all ancestors
			// who are merely reporting that a descendant's Selected fired.
			if (!ReferenceEquals(sender, e.OriginalSource))
				return;

			var treeViewItem = e.OriginalSource as TreeViewItem;
			if (treeViewItem != null)
			{
				var command = GetExpandedCommand(treeViewItem);
				if (command.CanExecute(treeViewItem.DataContext))	// The command parameter is the current binding.
					command.Execute(treeViewItem.DataContext);
			}
		}
	}
}