using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Utilities.Controls.Behaviors;
using Xunit;

namespace Unit.Tests.Utilities.Controls.Behaviors
{
	public class CollectionViewSourceItemFilterTests
	{
		[Fact]
		public void Test_ItemFilterBehavior()
		{
			// Arrange.
			var items = new ObservableCollection<string>();
			var viewSource = new CollectionViewSource
			{
				Source = items
			};

			var filter = CreateFilter();

			CollectionViewSourceItemFilter.SetItemFilter(viewSource, filter);

			// Act: trigger a view refresh (Refresh() wasn't working).
			items.Add("item");

			// Assert.
			Assert.True(filterInvocations[0]);
		}

		[Fact]
		public void Test_ItemFilterBehavior_Replaced()
		{
			// Arrange.
			var items = new ObservableCollection<string>();
			var viewSource = new CollectionViewSource
			{
				Source = items
			};

			var filter1 = CreateFilter();
			var filter2 = CreateFilter();

			CollectionViewSourceItemFilter.SetItemFilter(viewSource, filter1);

			// Act.
			CollectionViewSourceItemFilter.SetItemFilter(viewSource, filter2);
			items.Add("item");	// Trigger a view refresh.

			// Assert.
			Assert.False(filterInvocations[0]);
			Assert.True(filterInvocations[1]);
		}

		[Fact]
		public void Test_ItemFilterBehavior_ReplacedByNull()
		{
			// Arrange.
			var items = new ObservableCollection<string>();
			var viewSource = new CollectionViewSource
			{
				Source = items
			};

			var filter = CreateFilter();

			CollectionViewSourceItemFilter.SetItemFilter(viewSource, filter);

			// Act.
			CollectionViewSourceItemFilter.SetItemFilter(viewSource, null);
			items.Add("item");	// Trigger a view refresh.

			// Assert.
			Assert.False(filterInvocations[0]);
		}

		private Predicate<object> CreateFilter()
		{
			filterInvocations.Add(false);
			int index = filterCounter++;
			return item =>
			{
				filterInvocations[index] = true;
				return true;
			};
		}

		private int filterCounter;
		private readonly IList<bool> filterInvocations = new List<bool>();
	}
}
