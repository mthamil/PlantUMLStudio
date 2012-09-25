using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Interactivity;
using Utilities.Controls.Behaviors;
using Xunit;

namespace Unit.Tests.Utilities.Controls.Behaviors
{
	public class CollectionViewSourceFilterActionTests
	{
		[Fact]
		public void Test_FilterBehavior()
		{
			// Arrange.
			var items = new ObservableCollection<string>();
			var viewSource = new CollectionViewSource
			{
				Source = items
			};

			var filter = CreateFilter();

			var action = new CollectionViewSourceFilterAction();
			action.Attach(viewSource);
			action.Filter = filter;

			var trigger = new EventTrigger { EventName = "Filter" };
			trigger.Attach(viewSource);
			trigger.Actions.Add(action);

			// Act: trigger a view refresh (Refresh() wasn't working).
			items.Add("item");

			// Assert.
			Assert.True(filterInvocations[0]);
		}

		[Fact]
		public void Test_FilterBehavior_Replaced()
		{
			// Arrange.
			var items = new ObservableCollection<string>();
			var viewSource = new CollectionViewSource
			{
				Source = items
			};

			var filter1 = CreateFilter();
			var filter2 = CreateFilter();

			var action = new CollectionViewSourceFilterAction();
			action.Attach(viewSource);
			action.Filter = filter1;

			var trigger = new EventTrigger { EventName = "Filter" };
			trigger.Attach(viewSource);
			trigger.Actions.Add(action);

			// Act.
			action.Filter = filter2;
			items.Add("item");	// Trigger a view refresh.

			// Assert.
			Assert.False(filterInvocations[0]);
			Assert.True(filterInvocations[1]);
		}

		[Fact]
		public void Test_FilterBehavior_ReplacedByNull()
		{
			// Arrange.
			var items = new ObservableCollection<string>();
			var viewSource = new CollectionViewSource
			{
				Source = items
			};

			var filter = CreateFilter();

			var action = new CollectionViewSourceFilterAction();
			action.Attach(viewSource);
			action.Filter = filter;

			var trigger = new EventTrigger { EventName = "Filter" };
			trigger.Attach(viewSource);
			trigger.Actions.Add(action);

			// Act.
			action.Filter = null;
			items.Add("item");	// Trigger a view refresh.

			// Assert.
			Assert.False(filterInvocations[0]);
		}

		[Fact]
		public void Test_FilterBehavior_Detached()
		{
			// Arrange.
			var items = new ObservableCollection<string>();
			var viewSource = new CollectionViewSource
			{
				Source = items
			};

			var filter = CreateFilter();

			var action = new CollectionViewSourceFilterAction();
			action.Attach(viewSource);
			action.Filter = filter;

			var trigger = new EventTrigger { EventName = "Filter" };
			trigger.Attach(viewSource);
			trigger.Actions.Add(action);

			// Act.
			action.Detach();
			trigger.Detach();
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
