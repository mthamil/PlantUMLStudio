using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Concurrency;
using Xunit;
using Xunit.Sdk;

namespace Unit.Tests
{
	/// <summary>
	/// Contains tests for custom assertions.
	/// </summary>
	public class AssertThatTests
	{
		[Fact]
		public void Test_SequenceEqual_ShorterThanExpected()
		{
			// Arrange.
			var expected = new List<int> { 1, 2, 3 };
			var actual = new List<int> { 1, 2 };

			// Act/Assert.
			var exception = Assert.Throws<SequenceEqualException<int>>(() =>
				AssertThat.SequenceEqual(expected, actual));

			Assert.Equal("1,2,3", exception.Expected);
			Assert.Equal("1,2", exception.Actual);
			Assert.Equal("SequenceEqualException`1 : SequenceEqual Assertion Failure\r\nExpected: 1,2,3\r\nActual: 1,2", exception.Message);
		}

		[Fact]
		public void Test_SequenceEqual_LongerThanExpected()
		{
			// Arrange.
			var expected = new List<int> { 1, 2 };
			var actual = new List<int> { 1, 2, 3 };

			// Act/Assert.
			var exception = Assert.Throws<SequenceEqualException<int>>(() =>
				AssertThat.SequenceEqual(expected, actual));

			Assert.Equal("1,2", exception.Expected);
			Assert.Equal("1,2,3", exception.Actual);
			Assert.Equal("SequenceEqualException`1 : SequenceEqual Assertion Failure\r\nExpected: 1,2\r\nActual: 1,2,3", exception.Message);
		}

		[Fact]
		public void Test_SequenceEqual_ElementDifference()
		{
			// Arrange.
			var expected = new List<int> { 1, 4, 3 };
			var actual = new List<int> { 1, 2, 3 };

			// Act/Assert.
			var exception = Assert.Throws<SequenceEqualException<int>>(() =>
				AssertThat.SequenceEqual(expected, actual));

			Assert.Equal("1,4", exception.Expected);
			Assert.Equal("1,2", exception.Actual);
			Assert.Equal("SequenceEqualException`1 : SequenceEqual Assertion Failure\r\nExpected: 1,4,...\r\nActual: 1,2,...", exception.Message);
		}

		[Fact]
		public void Test_SequenceEqual_EqualLists()
		{
			// Arrange.
			var expected = new List<int> { 1, 2, 3 };
			var actual = new List<int> { 1, 2, 3 };

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.SequenceEqual(expected, actual));
		}

		[Fact]
		public void Test_SequenceEqual_EqualSequences()
		{
			// Arrange.
			var expected = new List<int> { 1, 2, 3 };
			var actual = Enumerable.Range(1, 3);

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.SequenceEqual(expected, actual));
		}

		[Fact]
		public void Test_SequenceEqual_EmptyExpected()
		{
			// Arrange.
			var expected = Enumerable.Empty<int>();
			var actual = new List<int> { 1, 2, 3 };

			// Act/Assert.
			var exception = Assert.Throws<SequenceEqualException<int>>(() =>
				AssertThat.SequenceEqual(expected, actual));

			Assert.Equal(string.Empty, exception.Expected);
			Assert.Equal("1", exception.Actual);
			Assert.Equal("SequenceEqualException`1 : SequenceEqual Assertion Failure\r\nExpected: Empty Sequence\r\nActual: 1,...", exception.Message);
		}

		[Fact]
		public void Test_SequenceEqual_EmptyActual()
		{
			// Arrange.
			var expected = new List<int> { 1, 2, 3 };
			var actual = Enumerable.Empty<int>();

			// Act/Assert.
			var exception = Assert.Throws<SequenceEqualException<int>>(() =>
				AssertThat.SequenceEqual(expected, actual));

			Assert.Equal("1", exception.Expected);
			Assert.Equal(string.Empty, exception.Actual);
			Assert.Equal("SequenceEqualException`1 : SequenceEqual Assertion Failure\r\nExpected: 1,...\r\nActual: Empty Sequence", exception.Message);
		}

		[Fact]
		public void Test_Raises_WithEventName_Success()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.Raises(test, "TestEvent", () => test.OnTestEvent(4)));

			Assert.True(test.EventRaised);
		}

		[Fact]
		public void Test_Raises_WithEventName_Failure()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.Throws<RaisesException>(() =>
				AssertThat.Raises(test, "TestEvent", () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		[Fact]
		public void Test_Raises_WithEventSubscriber_Success()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.Raises(test, t => t.TestEvent += null, () => test.OnTestEvent(4)));

			Assert.True(test.EventRaised);
		}

		[Fact]
		public void Test_Raises_WithEventSubscriber_Failure()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.Throws<RaisesException>(() =>
				AssertThat.Raises(test, t => t.TestEvent += null, () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		[Fact]
		public void Test_DoesNotRaise_WithEventName_Success()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.Throws<RaisesException>(() =>
				AssertThat.DoesNotRaise(test, "TestEvent", () => test.OnTestEvent(4)));

			Assert.True(test.EventRaised);
		}

		[Fact]
		public void Test_DoesNotRaise_WithEventName_Failure()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.DoesNotRaise(test, "TestEvent", () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		[Fact]
		public void Test_DoesNotRaise_WithEventSubscriber_Success()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.Throws<RaisesException>(() =>
				AssertThat.DoesNotRaise(test, t => t.TestEvent += null, () => test.OnTestEvent(4)));

			Assert.True(test.EventRaised);
		}

		[Fact]
		public void Test_DoesNotRaise_WithEventSubscriber_Failure()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.DoesNotRaise(test, t => t.TestEvent += null, () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		[Fact]
		public void Test_RaisesWithEventArgs_WithEventName_Success()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			TestEventArgs args = null;
			Assert.DoesNotThrow(() =>
				args = AssertThat.RaisesWithEventArgs<TestEventArgs>(test, "TestEvent", () => test.OnTestEvent(4)));

			Assert.Equal(4, args.Value);
			Assert.True(test.EventRaised);
		}

		[Fact]
		public void Test_RaisesWithEventArgs_WithEventName_EventNotRaised()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.Throws<RaisesException>(() =>
				AssertThat.RaisesWithEventArgs<TestEventArgs>(test, "TestEvent", () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		[Fact]
		public void Test_RaisesWithEventArgs_WithEventSubscriber_Success()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			TestEventArgs args = null;
			Assert.DoesNotThrow(() =>
				args = AssertThat.RaisesWithEventArgs<IEventTest, TestEventArgs>(test, t => t.TestEvent += null, () => test.OnTestEvent(4)));

			Assert.Equal(4, args.Value);
			Assert.True(test.EventRaised);
		}

		[Fact]
		public void Test_RaisesWithEventArgs_WithEventSubscriber_Failure_EventNotRaised()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			Assert.Throws<RaisesException>(() =>
				AssertThat.RaisesWithEventArgs<IEventTest, TestEventArgs>(test, t => t.TestEvent += null, () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		[Fact]
		public void Test_PropertyChanged_EventNotRaised()
		{
			// Arrange.
			var test = new EventTest { IntProperty = 3 };

			// Act/Assert.
			Assert.Throws<PropertyChangedException>(() =>
				AssertThat.PropertyChanged(test, t => t.IntProperty, () => test.IntProperty = 3));
		}

		[Fact]
		public void Test_PropertyChanged_EventRaised_WrongProperty()
		{
			// Arrange.
			var test = new EventTest();

			// Act/Assert.
			Assert.Throws<PropertyChangedException>(() =>
				AssertThat.PropertyChanged(test, t => t.IntProperty, () => test.StringProperty = "value"));
		}

		[Fact]
		public void Test_PropertyChanged_EventRaised()
		{
			// Arrange.
			var test = new EventTest();

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.PropertyChanged(test, t => t.IntProperty, () => test.IntProperty = 3));
		}

		[Fact]
		public void Test_PropertyDoesNotChange_EventNotRaised()
		{
			// Arrange.
			var test = new EventTest { IntProperty = 3 };

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.PropertyDoesNotChange(test, t => t.IntProperty, () => test.IntProperty = 3));
		}

		[Fact]
		public void Test_PropertyDoesNotChange_EventRaised_WrongProperty()
		{
			// Arrange.
			var test = new EventTest();

			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.PropertyDoesNotChange(test, t => t.IntProperty, () => test.StringProperty = "value"));
		}

		[Fact]
		public void Test_PropertyDoesNotChange_EventRaised()
		{
			// Arrange.
			var test = new EventTest();

			// Act/Assert.
			Assert.Throws<PropertyDoesNotChangeException>(() =>
				AssertThat.PropertyDoesNotChange(test, t => t.IntProperty, () => test.IntProperty = 3));
		}

		[Fact]
		public void Test_Throws_Task_Success()
		{
			// Act/Assert.
			Assert.DoesNotThrow(() =>
				AssertThat.Throws<ArgumentException>(Task.Factory.StartNew(() => 
					{ 
						throw new ArgumentException(); 
					}, CancellationToken.None, TaskCreationOptions.None, new SynchronousTaskScheduler())));
		}

		[Fact]
		public void Test_Throws_Task_NoException()
		{
			// Act/Assert.
			var expected = Assert.Throws<ThrowsException>(() =>
				AssertThat.Throws<ArgumentException>(Task.Factory.StartNew(() =>
				{
					string.Empty.GetType();
				}, CancellationToken.None, TaskCreationOptions.None, new SynchronousTaskScheduler())));

			// Assert.
			Assert.NotNull(expected);
		}

		[Fact]
		public void Test_Throws_Task_WrongException()
		{
			// Act/Assert.
			var expected = Assert.Throws<ThrowsException>(() =>
				AssertThat.Throws<ArgumentException>(Task.Factory.StartNew(() =>
				{
					throw new ArgumentOutOfRangeException();
				}, CancellationToken.None, TaskCreationOptions.None, new SynchronousTaskScheduler())));

			// Assert.
			Assert.NotNull(expected);
		}

		[Fact]
		public void Test_DoesNotThrow_Task_Success()
		{
			// Act/Assert.
			Assert.DoesNotThrow(() => 
				AssertThat.DoesNotThrow(Task.Factory.StartNew(() =>
				{
					 string.Empty.GetType();
				}, CancellationToken.None, TaskCreationOptions.None, new SynchronousTaskScheduler())));
		}

		[Fact]
		public void Test_DoesNotThrow_Task_Failure()
		{
			// Act/Assert.
			var expected = Assert.Throws<DoesNotThrowException>(() => 
				AssertThat.DoesNotThrow(Task.Factory.StartNew(() =>
				{
					throw new ArgumentNullException();
				}, CancellationToken.None, TaskCreationOptions.None, new SynchronousTaskScheduler())));

			// Assert.
			Assert.NotNull(expected);
		}

		public interface IEventTest
		{
			event EventHandler<TestEventArgs> TestEvent;
			void OnTestEvent(int value);
			bool EventRaised { get; }
		}

		private class EventTest : IEventTest, INotifyPropertyChanged
		{
			public event EventHandler<TestEventArgs> TestEvent;
			public void OnTestEvent(int value)
			{
				if (TestEvent != null)
				{
					TestEvent(this, new TestEventArgs(value));
					EventRaised = true;
				}
			}

			public bool EventRaised { get; private set; }

			public int IntProperty 
			{
				get { return _intProperty; }
				set
				{
					if (_intProperty != value)
					{
						_intProperty = value;
						OnPropertyChanged();
					}
				}
			}

			private int _intProperty;

			public string StringProperty
			{
				get { return _stringProperty; }
				set
				{
					if (_stringProperty != value)
					{
						_stringProperty = value;
						OnPropertyChanged();
					}
				}
			}

			private string _stringProperty;

			public event PropertyChangedEventHandler PropertyChanged;

			private void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null) 
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public class TestEventArgs : EventArgs
		{
			public TestEventArgs(int value)
			{
				Value = value;
			}

			public int Value { get; private set; }
		}
	}
}