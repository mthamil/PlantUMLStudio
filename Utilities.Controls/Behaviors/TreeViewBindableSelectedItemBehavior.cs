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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Since the TreeView's SelectedItem property is readonly, it is not bindable.
	/// This attached behavior provides a selected item property that is bindable.
	/// </summary>
	public static class TreeViewBindableSelectedItemBehavior
	{
		/// <summary>
		/// Gets the selected item.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TreeView))]
		public static object GetBindableSelectedItem(TreeView treeView)
		{
			return treeView.GetValue(BindableSelectedItemProperty);
		}

		/// <summary>
		/// Sets the selected item.
		/// </summary>
		public static void SetBindableSelectedItem(TreeView treeView, object value)
		{
			treeView.SetValue(BindableSelectedItemProperty, value);
		}

		/// <summary>
		/// The BindableSelectedItem dependency property.
		/// </summary>
		public static readonly DependencyProperty BindableSelectedItemProperty =
			DependencyProperty.RegisterAttached(
			"BindableSelectedItem",
			typeof(object),
			typeof(TreeViewBindableSelectedItemBehavior),
			new UIPropertyMetadata(null, OnBindableSelectedItemChanged));

		private static void OnBindableSelectedItemChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			TreeView treeView = depObj as TreeView;
			if (treeView == null)
				return;

			var selectedItem = e.NewValue;
			if (selectedItem == null)
				return;

			// The first time the property is changed on a given TreeView, wire up the SelectedItemChanged event handler.
			if (!behaviors.ContainsKey(depObj))
				behaviors.Add(depObj, new TreeViewSelectedItemBehavior(treeView));

			TreeViewSelectedItemBehavior treeViewBehavior = behaviors[depObj];
			treeViewBehavior.ChangeSelectedItem(selectedItem);
		}

		private class TreeViewSelectedItemBehavior
		{
			public TreeViewSelectedItemBehavior(TreeView treeView)
			{
				_treeView = treeView;
				treeView.SelectedItemChanged += (sender, e) => SetBindableSelectedItem(treeView, e.NewValue);
			}

			internal void ChangeSelectedItem(object newlySelectedItem)
			{
				var item = (TreeViewItem)_treeView.ItemContainerGenerator.ContainerFromItem(newlySelectedItem);
				if (item != null && !item.IsSelected)
					item.IsSelected = true;
			}

			private readonly TreeView _treeView;
		}

		private static readonly IDictionary<DependencyObject, TreeViewSelectedItemBehavior> behaviors = new Dictionary<DependencyObject, TreeViewSelectedItemBehavior>();
	}
}