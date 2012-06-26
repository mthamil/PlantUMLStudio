using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PlantUmlEditor.Controls.Behaviors
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