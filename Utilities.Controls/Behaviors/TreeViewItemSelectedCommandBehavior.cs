using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Attached behavior that executes a command when a TreeViewItem is selected.
	/// </summary>
	public static class TreeViewItemSelectedCommandBehavior
	{
		/// <summary>
		/// Gets the selected command for a TreeViewItem.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
		public static ICommand GetSelectedCommand(TreeViewItem treeViewItem)
		{
			return (ICommand)treeViewItem.GetValue(SelectedCommandProperty);
		}

		/// <summary>
		/// Sets the selected command for a TreeViewItem.
		/// </summary>
		public static void SetSelectedCommand(TreeViewItem treeViewItem, ICommand value)
		{
			treeViewItem.SetValue(SelectedCommandProperty, value);
		}

		/// <summary>
		/// The command property.
		/// </summary>
		public static readonly DependencyProperty SelectedCommandProperty =
			DependencyProperty.RegisterAttached(
			"SelectedCommand",
			typeof(ICommand),
			typeof(TreeViewItemSelectedCommandBehavior),
			new UIPropertyMetadata(null, OnSelectedCommandChanged));

		static void OnSelectedCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			TreeViewItem treeViewItem = depObj as TreeViewItem;
			if (treeViewItem == null)
				return;

			var newCommand = e.NewValue as ICommand;
			if (newCommand == null)
				return;

			if ((e.NewValue != null) && (e.OldValue == null))
			{
				treeViewItem.Selected += treeViewItem_Selected;
			}
			else if ((e.NewValue == null) && (e.OldValue != null))
			{
				treeViewItem.Selected -= treeViewItem_Selected;
			}
		}

		static void treeViewItem_Selected(object sender, RoutedEventArgs e)
		{
			// Only react to the Selected event raised by the TreeViewItem
			// whose IsSelected property was modified. Ignore all ancestors
			// who are merely reporting that a descendant's Selected fired.
			if (!ReferenceEquals(sender, e.OriginalSource))
				return;

			var treeViewItem = e.OriginalSource as TreeViewItem;
			if (treeViewItem != null)
			{
				var command = GetSelectedCommand(treeViewItem);
				if (command.CanExecute(treeViewItem.DataContext))	// The command parameter is the current binding.
				    command.Execute(treeViewItem.DataContext);
			}
		}
	}
}
