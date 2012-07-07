using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
			var exception = AssertThat.Throws<SequenceEqualException<int>>(() =>
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
			var exception = AssertThat.Throws<SequenceEqualException<int>>(() =>
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
			var exception = AssertThat.Throws<SequenceEqualException<int>>(() =>
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
			AssertThat.DoesNotThrow(() =>
				AssertThat.SequenceEqual(expected, actual));
		}

		[Fact]
		public void Test_SequenceEqual_EqualSequences()
		{
			// Arrange.
			var expected = new List<int> { 1, 2, 3 };
			var actual = Enumerable.Range(1, 3);

			// Act/Assert.
			AssertThat.DoesNotThrow(() =>
				AssertThat.SequenceEqual(expected, actual));
		}

		[Fact]
		public void Test_SequenceEqual_EmptyExpected()
		{
			// Arrange.
			var expected = Enumerable.Empty<int>();
			var actual = new List<int> { 1, 2, 3 };

			// Act/Assert.
			var exception = AssertThat.Throws<SequenceEqualException<int>>(() =>
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
			var exception = AssertThat.Throws<SequenceEqualException<int>>(() =>
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
			AssertThat.DoesNotThrow(() =>
				AssertThat.Raises(test, "TestEvent", () => test.OnTestEvent(4)));

			Assert.True(test.EventRaised);
		}

		[Fact]
		public void Test_Raises_WithEventName_Failure()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			AssertThat.Throws<RaisesException>(() =>
				AssertThat.Raises(test, "TestEvent", () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		//[Fact]
		//public void Test_Raises_WithEventSubscriber_Success()
		//{
		//    // Arrange.
		//    IEventTest test = new EventTest();

		//    // Act/Assert.
		//    AssertThat.DoesNotThrow(() =>
		//        AssertThat.Raises(test, t => t.TestEvent += null, () => test.OnTestEvent(4)));

		//    Assert.True(test.EventRaised);
		//}

		//[Fact]
		//public void Test_Raises_WithEventSubscriber_Failure()
		//{
		//    // Arrange.
		//    IEventTest test = new EventTest();

		//    // Act/Assert.
		//    AssertThat.Throws<RaisesException>(() =>
		//        AssertThat.Raises(test, t => t.TestEvent += null, () => test.ToString()));

		//    Assert.False(test.EventRaised);
		//}

		[Fact]
		public void Test_DoesNotRaise_WithEventName_Success()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			AssertThat.Throws<RaisesException>(() =>
				AssertThat.DoesNotRaise(test, "TestEvent", () => test.OnTestEvent(4)));

			Assert.True(test.EventRaised);
		}

		[Fact]
		public void Test_DoesNotRaise_WithEventName_Failure()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			AssertThat.DoesNotThrow(() =>
				AssertThat.DoesNotRaise(test, "TestEvent", () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		//[Fact]
		//public void Test_DoesNotRaise_WithEventSubscriber_Success()
		//{
		//    // Arrange.
		//    IEventTest test = new EventTest();

		//    // Act/Assert.
		//    AssertThat.Throws<RaisesException>(() =>
		//        AssertThat.DoesNotRaise(test, t => t.TestEvent += null, () => test.OnTestEvent(4)));

		//    Assert.True(test.EventRaised);
		//}

		//[Fact]
		//public void Test_DoesNotRaise_WithEventSubscriber_Failure()
		//{
		//    // Arrange.
		//    IEventTest test = new EventTest();

		//    // Act/Assert.
		//    AssertThat.DoesNotThrow(() =>
		//        AssertThat.DoesNotRaise(test, t => t.TestEvent += null, () => test.ToString()));

		//    Assert.False(test.EventRaised);
		//}

		[Fact]
		public void Test_RaisesWithEventArgs_WithEventName_Success()
		{
			// Arrange.
			IEventTest test = new EventTest();

			// Act/Assert.
			TestEventArgs args = null;
			AssertThat.DoesNotThrow(() =>
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
			AssertThat.Throws<RaisesException>(() =>
				AssertThat.RaisesWithEventArgs<TestEventArgs>(test, "TestEvent", () => test.ToString()));

			Assert.False(test.EventRaised);
		}

		//[Fact]
		//public void Test_RaisesWithEventArgs_WithEventSubscriber_Success()
		//{
		//    // Arrange.
		//    IEventTest test = new EventTest();

		//    // Act/Assert.
		//    TestEventArgs args = null;
		//    AssertThat.DoesNotThrow(() =>
		//        args = AssertThat.RaisesWithEventArgs<IEventTest, TestEventArgs>(test, t => t.TestEvent += null, () => test.OnTestEvent(4)));

		//    Assert.Equal(4, args.Value);
		//    Assert.True(test.EventRaised);
		//}

		//[Fact]
		//public void Test_RaisesWithEventArgs_WithEventSubscriber_Failure_EventNotRaised()
		//{
		//    // Arrange.
		//    IEventTest test = new EventTest();

		//    // Act/Assert.
		//    AssertThat.Throws<RaisesException>(() =>
		//        AssertThat.RaisesWithEventArgs<IEventTest, TestEventArgs>(test, t => t.TestEvent += null, () => test.ToString()));

		//    Assert.False(test.EventRaised);
		//}

		[Fact]
		public void Test_Throws_Success()
		{
			// Act.
			ThrowsException expected = null;
			try
			{
				AssertThat.Throws<ArgumentException>(() => { throw new ArgumentException(); });
			}
			catch (ThrowsException e)
			{
				expected = e;
			}

			// Assert.
			Assert.Null(expected);
		}

		[Fact]
		public void Test_Throws_NoException()
		{
			// Act.
			ThrowsException expected = null;
			try
			{
				AssertThat.Throws<ArgumentException>(() => string.Empty.GetType());
			}
			catch (ThrowsException e)
			{
				expected = e;
			}

			// Assert.
			Assert.NotNull(expected);
			Assert.Equal("ThrowsException : Throws Assertion Failure\r\nExpected: ArgumentException, Actual: No Exception", expected.Message);
		}

		[Fact]
		public void Test_Throws_WrongException()
		{
			// Act.
			ThrowsException expected = null;
			try
			{
				AssertThat.Throws<ArgumentException>(() => { throw new ArgumentOutOfRangeException(); });
			}
			catch (ThrowsException e)
			{
				expected = e;
			}

			// Assert.
			Assert.NotNull(expected);
			Assert.Equal("ThrowsException : Throws Assertion Failure\r\nExpected: ArgumentException, Actual: ArgumentOutOfRangeException", expected.Message);
		}

		[Fact]
		public void Test_DoesNotThrow_Success()
		{
			// Act.
			ThrowsException expected = null;
			try
			{
				AssertThat.DoesNotThrow(() => string.Empty.GetType());
			}
			catch (ThrowsException e)
			{
				expected = e;
			}

			// Assert.
			Assert.Null(expected);
		}

		[Fact]
		public void Test_DoesNotThrow_Failure()
		{
			// Act.
			ThrowsException expected = null;
			try
			{
				AssertThat.DoesNotThrow(() => { throw new ArgumentNullException(); });
			}
			catch (ThrowsException e)
			{
				expected = e;
			}

			// Assert.
			Assert.NotNull(expected);
			Assert.Equal("ThrowsException : Throws Assertion Failure\r\nExpected: No Exception, Actual: ArgumentNullException", expected.Message);
		}

		public interface IEventTest
		{
			event EventHandler<TestEventArgs> TestEvent;
			void OnTestEvent(int value);
			bool EventRaised { get; }
		}

		private class EventTest : IEventTest
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