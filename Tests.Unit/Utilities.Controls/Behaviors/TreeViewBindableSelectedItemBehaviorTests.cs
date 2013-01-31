using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities.Controls.Behaviors;
using Utilities.PropertyChanged;
using Xunit;

namespace Tests.Unit.Utilities.Controls.Behaviors
{
	public class TreeViewBindableSelectedItemBehaviorTests
	{
		[Fact]
		public void Test_BindableSelectedItem_View_To_ViewModel()
		{
			// Arrange.
			var selectedItemWatcher = new SelectedItemWatcher();
			var child = new TestViewModel();
			selectedItemWatcher.Children.Add(child);

			var childView = new TreeViewItem { DataContext = child };
			var isSelectedBinding = new Binding("IsSelected") { Mode = BindingMode.TwoWay };
			childView.SetBinding(TreeViewItem.IsSelectedProperty, isSelectedBinding);

			var treeView = new TreeView { DataContext = selectedItemWatcher };
			treeView.Items.Add(childView);

			var selectedNodeBinding = new Binding("SelectedItem")
			{
				Source = selectedItemWatcher,
				Mode = BindingMode.TwoWay
			};
			treeView.SetBinding(TreeViewBindableSelectedItemBehavior.BindableSelectedItemProperty, selectedNodeBinding);
			TreeViewBindableSelectedItemBehavior.SetBindableSelectedItem(treeView, new object());	// Initialize the property.

			// Act.
			childView.IsSelected = true;

			// Assert.
			Assert.True(child.IsSelected);
			Assert.Equal(childView, selectedItemWatcher.SelectedItem);	// SelectedItem is a TreeViewItem because the 
																		// TreeView's ItemsSource isn't set.  Couldn't get 
																		// items to generate in the test.

			Assert.Equal(childView, TreeViewBindableSelectedItemBehavior.GetBindableSelectedItem(treeView));
		}

		public class TestViewModel : PropertyChangedNotifier
		{
			public TestViewModel()
			{
				_isSelected = Property.New(this, p => p.IsSelected, OnPropertyChanged);
			}

			public bool IsSelected
			{
				get { return _isSelected.Value; }
				set { _isSelected.Value = value; }
			}

			private readonly Property<bool> _isSelected;
		}

		public class SelectedItemWatcher : PropertyChangedNotifier
		{
			public SelectedItemWatcher()
			{
				_selectedItem = Property.New(this, p => p.SelectedItem, OnPropertyChanged);
				_children = Property.New(this, p => p.Children, OnPropertyChanged);
				_children.Value = new ObservableCollection<TestViewModel>();
			}

			public object SelectedItem
			{
				get { return _selectedItem.Value; }
				set { _selectedItem.Value = value; }
			}
			private readonly Property<object> _selectedItem;

			public ICollection<TestViewModel> Children
			{
				get { return _children.Value; }
			}
			private readonly Property<ICollection<TestViewModel>> _children;
		}
	}
}
