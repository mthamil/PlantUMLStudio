using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.Utilities.Mvvm.Commands
{
	public class AggregateBoundRelayCommandTests
	{
		[Theory]
		[InlineData(true, new [] { true, true })]
		[InlineData(true, new[] { true, false })]
		[InlineData(true, new[] { false, true })]
		[InlineData(false, new[] { false, false })]
		public void Test_CanExecute_Any(bool expected, bool[] values)
		{
			// Arrange.
			var parent = new TestParent();
			foreach (var value in values)
			{
				var child = new TestItem { BoolValue = value };
				parent.Items.Add(child);
			}

			var command = new AggregateBoundRelayCommand<TestParent, TestItem, ObservableCollection<TestItem>>(_ => { },
				p => p.Items, c => c.Any(p => p.BoolValue), parent);

			// Act.
			bool actual = command.CanExecute(null);
			
			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(true, new[] { true, true })]
		[InlineData(false, new[] { true, false })]
		[InlineData(false, new[] { false, true })]
		[InlineData(false, new[] { false, false })]
		public void Test_CanExecute_All(bool expected, bool[] values)
		{
			// Arrange.
			var parent = new TestParent();
			foreach (var value in values)
			{
				var child = new TestItem { BoolValue = value };
				parent.Items.Add(child);
			}

			var command = new AggregateBoundRelayCommand<TestParent, TestItem, ObservableCollection<TestItem>>(_ => { },
				p => p.Items, c => c.All(p => p.BoolValue), parent);

			// Act.
			bool actual = command.CanExecute(null);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Test_CanExecuteChanged()
		{
			// Arrange.
			var parent = new TestParent();
			var child1 = new TestItem();
			var child2 = new TestItem();

			var command = new AggregateBoundRelayCommand<TestParent,TestItem,  ObservableCollection<TestItem>>(_ => { }, 
				p => p.Items, c => c.Any(p => p.BoolValue), parent);

			parent.Items.Add(child1);
			parent.Items.Add(child2);

			// Act/Assert.
			foreach (var child in parent.Items)
			{
				var localChild = child;
				AssertThat.Raises<ICommand>(command, c => c.CanExecuteChanged += null, () => localChild.BoolValue = true);
				AssertThat.DoesNotRaise<ICommand>(command, c => c.CanExecuteChanged += null, () => localChild.BoolValue = true);
			}
		}

		[Fact]
		public void Test_CanExecuteChanged_CollectionCleared()
		{
			// Arrange.
			var parent = new TestParent();
			var child1 = new TestItem();
			var child2 = new TestItem();

			var command = new AggregateBoundRelayCommand<TestParent, TestItem, ObservableCollection<TestItem>>(_ => { },
				p => p.Items, c => c.Any(p => p.BoolValue), parent);

			parent.Items.Add(child1);
			parent.Items.Add(child2);

			// Act/Assert.
			parent.Items.Clear();

			foreach (var child in new [] { child1, child2 })
			{
				var localChild = child;
				AssertThat.DoesNotRaise<ICommand>(command, c => c.CanExecuteChanged += null, () => localChild.BoolValue = true);
			}
		}

		[Fact]
		public void Test_CanExecuteChanged_ItemRemoved()
		{
			// Arrange.
			var parent = new TestParent();
			var child = new TestItem();

			var command = new AggregateBoundRelayCommand<TestParent, TestItem, ObservableCollection<TestItem>>(_ => { },
				p => p.Items, c => c.Any(p => p.BoolValue), parent);

			parent.Items.Add(child);

			// Act/Assert.
			parent.Items.Remove(child);
			AssertThat.DoesNotRaise<ICommand>(command, c => c.CanExecuteChanged += null, () => child.BoolValue = true);
		}

		[Fact]
		public void Test_Execute()
		{
			// Arrange.
			var parent = new TestParent();

			bool executed = false;
			var command = new AggregateBoundRelayCommand<TestParent, TestItem, ObservableCollection<TestItem>>(
				_ => executed = true,
				p => p.Items, c => c.Any(p => p.BoolValue), parent);

			// Act.
			command.Execute(null);

			// Assert.
			Assert.True(executed);
		}

		private class TestParent : PropertyChangedNotifier
		{
			public TestParent()
			{
				_items = Property.New(this, p => p.Items, OnPropertyChanged);
				_items.Value = new ObservableCollection<TestItem>();
			}

			public ObservableCollection<TestItem> Items
			{
				get { return _items.Value; }
			}

			private readonly Property<ObservableCollection<TestItem>> _items;
		}

		private class TestItem : PropertyChangedNotifier
		{
			public TestItem()
			{
				_boolValue = Property.New(this, p => BoolValue, OnPropertyChanged);
			}

			public bool BoolValue
			{
				get { return _boolValue.Value; }
				set { _boolValue.Value = value; }
			}

			private readonly Property<bool> _boolValue;
		}
	}
}
