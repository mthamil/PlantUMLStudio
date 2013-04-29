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
