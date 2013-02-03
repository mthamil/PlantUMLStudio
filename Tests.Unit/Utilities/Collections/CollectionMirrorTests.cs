using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Moq;
using Utilities.Collections;
using Xunit;

namespace Tests.Unit.Utilities.Collections
{
	public class CollectionMirrorTests
	{
		[Fact]
		public void Test_InitialSynchronization()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string> { "4", "5", "6" };

			// Act.
			var mirror = new CollectionMirror(source, target);

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, target);
		}

		[Fact]
		public void Test_AddToSource()
		{
			// Arrange.
			var source = new ObservableCollection<string>();
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			source.Add("1");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1" }, source);
			AssertThat.SequenceEqual(new[] { "1" }, target);
		}

		[Fact]
		public void Test_AddToTarget()
		{
			// Arrange.
			var source = new ObservableCollection<string>();
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			target.Add("1");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1" }, source);
			AssertThat.SequenceEqual(new[] { "1" }, target);
		}

		[Fact]
		public void Test_InsertToSource()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			source.Insert(1, "2");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, target);
		}

		[Fact]
		public void Test_InsertToTarget()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			target.Insert(1, "2");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, target);
		}

		[Fact]
		public void Test_RemoveFromSource()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			source.Remove("2");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "3" }, source);
			AssertThat.SequenceEqual(new[] { "1", "3" }, target);
		}

		[Fact]
		public void Test_RemoveFromTarget()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			target.Remove("2");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "3" }, source);
			AssertThat.SequenceEqual(new[] { "1", "3" }, target);
		}

		[Fact]
		public void Test_RemoveIndexFromSource()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			source.RemoveAt(2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2" }, source);
			AssertThat.SequenceEqual(new[] { "1", "2" }, target);
		}

		[Fact]
		public void Test_RemoveIndexFromTarget()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			target.RemoveAt(2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2" }, source);
			AssertThat.SequenceEqual(new[] { "1", "2" }, target);
		}

		[Fact]
		public void Test_ReplaceInSource()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			source[0] = "0";

			// Assert.
			AssertThat.SequenceEqual(new[] { "0", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { "0", "2", "3" }, target);
		}

		[Fact]
		public void Test_ReplaceInTarget()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			target[0] = "0";

			// Assert.
			AssertThat.SequenceEqual(new[] { "0", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { "0", "2", "3" }, target);
		}

		[Fact]
		public void Test_MoveInSource()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			source.Move(0, 2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "2", "3", "1" }, source);
			AssertThat.SequenceEqual(new[] { "2", "3", "1" }, target);
		}

		[Fact]
		public void Test_MoveInTarget()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			target.Move(0, 2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "2", "3", "1" }, source);
			AssertThat.SequenceEqual(new[] { "2", "3", "1" }, target);
		}

		[Fact]
		public void Test_ClearSource()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			source.Clear();

			// Assert.
			AssertThat.SequenceEqual(new string[0], source);
			AssertThat.SequenceEqual(new string[0], target);
		}

		[Fact]
		public void Test_ClearTarget()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			target.Clear();

			// Assert.
			AssertThat.SequenceEqual(new string[0], source);
			AssertThat.SequenceEqual(new string[0], target);
		}

		[Fact]
		public void Test_RemoveMultiple()
		{
			// Arrange.
			var source = new Mock<IObservableList> { DefaultValue = DefaultValue.Empty };
			source.Setup(s => s.GetEnumerator()).Returns(new [] { "1", "2", "3" }.GetEnumerator());

			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source.Object, target);

			// Act.
			source.Raise(s => s.CollectionChanged += null, 
				new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new [] { "2", "3" }, 1));

			// Assert.
			AssertThat.SequenceEqual(new[] { "1" }, target);
		}

		[Fact]
		public void Test_AddMultiple()
		{
			// Arrange.
			var source = new Mock<IObservableList> { DefaultValue = DefaultValue.Empty };
			source.Setup(s => s.GetEnumerator()).Returns(new[] { "1", "2", "3" }.GetEnumerator());

			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source.Object, target);

			// Act.
			source.Raise(s => s.CollectionChanged += null,
				new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { "4", "5" }, 3));

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3", "4", "5" }, target);
		}

		[Fact]
		public void Test_Dispose_Unsubscribes()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<string>();
			var mirror = new CollectionMirror(source, target);

			// Act.
			mirror.Dispose();
			source.Add("4");
			target.Remove("1");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3", "4" }, source);
			AssertThat.SequenceEqual(new[] { "2", "3" }, target);
		}

		[Fact]
		public void Test_InitialSynchronization_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int> { 4, 5, 6 };

			// Act.
			var mirror = new CollectionMirror(source, target, 
				obj => Int32.Parse((string)obj), 
				obj => obj.ToString());

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { 1, 2, 3 }, target);
		}

		[Fact]
		public void Test_AddToSource_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string>();
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source.Add("1");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1" }, source);
			AssertThat.SequenceEqual(new[] { 1 }, target);
		}

		[Fact]
		public void Test_AddToTarget_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string>();
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			target.Add(1);

			// Assert.
			AssertThat.SequenceEqual(new[] { "1" }, source);
			AssertThat.SequenceEqual(new[] { 1 }, target);
		}

		[Fact]
		public void Test_InsertToSource_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source.Insert(1, "2");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { 1, 2, 3 }, target);
		}

		[Fact]
		public void Test_InsertToTarget_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			target.Insert(1, 2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { 1, 2, 3 }, target);
		}

		[Fact]
		public void Test_RemoveFromSource_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source.Remove("2");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "3" }, source);
			AssertThat.SequenceEqual(new[] { 1, 3 }, target);
		}

		[Fact]
		public void Test_RemoveFromTarget_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			target.Remove(2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "3" }, source);
			AssertThat.SequenceEqual(new[] { 1, 3 }, target);
		}

		[Fact]
		public void Test_RemoveIndexFromSource_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source.RemoveAt(2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2" }, source);
			AssertThat.SequenceEqual(new[] { 1, 2 }, target);
		}

		[Fact]
		public void Test_RemoveIndexFromTarget_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			target.RemoveAt(2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2" }, source);
			AssertThat.SequenceEqual(new[] { 1, 2 }, target);
		}

		[Fact]
		public void Test_ReplaceInSource_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source[0] = "0";

			// Assert.
			AssertThat.SequenceEqual(new[] { "0", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { 0, 2, 3 }, target);
		}

		[Fact]
		public void Test_ReplaceInTarget_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			target[0] = 0;

			// Assert.
			AssertThat.SequenceEqual(new[] { "0", "2", "3" }, source);
			AssertThat.SequenceEqual(new[] { 0, 2, 3 }, target);
		}

		[Fact]
		public void Test_MoveInSource_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source.Move(0, 2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "2", "3", "1" }, source);
			AssertThat.SequenceEqual(new[] { 2, 3, 1 }, target);
		}

		[Fact]
		public void Test_MoveInTarget_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			target.Move(0, 2);

			// Assert.
			AssertThat.SequenceEqual(new[] { "2", "3", "1" }, source);
			AssertThat.SequenceEqual(new[] { 2, 3, 1 }, target);
		}

		[Fact]
		public void Test_ClearSource_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source.Clear();

			// Assert.
			AssertThat.SequenceEqual(new string[0], source);
			AssertThat.SequenceEqual(new int[0], target);
		}

		[Fact]
		public void Test_ClearTarget_WithMapping()
		{
			// Arrange.
			var source = new ObservableCollection<string> { "1", "2", "3" };
			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			target.Clear();

			// Assert.
			AssertThat.SequenceEqual(new string[0], source);
			AssertThat.SequenceEqual(new int[0], target);
		}

		[Fact]
		public void Test_RemoveMultiple_WithMapping()
		{
			// Arrange.
			var source = new Mock<IObservableList> { DefaultValue = DefaultValue.Empty };
			source.Setup(s => s.GetEnumerator()).Returns(new[] { "1", "2", "3" }.GetEnumerator());

			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source.Object, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source.Raise(s => s.CollectionChanged += null,
				new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { "2", "3" }, 1));

			// Assert.
			AssertThat.SequenceEqual(new[] { 1 }, target);
		}

		[Fact]
		public void Test_AddMultiple_WithMapping()
		{
			// Arrange.
			var source = new Mock<IObservableList> { DefaultValue = DefaultValue.Empty };
			source.Setup(s => s.GetEnumerator()).Returns(new[] { "1", "2", "3" }.GetEnumerator());

			var target = new ObservableCollection<int>();
			var mirror = new CollectionMirror(source.Object, target,
				obj => Int32.Parse((string)obj),
				obj => obj.ToString());

			// Act.
			source.Raise(s => s.CollectionChanged += null,
				new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { "4", "5" }, 3));

			// Assert.
			AssertThat.SequenceEqual(new[] { 1, 2, 3, 4, 5 }, target);
		}

		public interface IObservableList : INotifyCollectionChanged, IList { }
	}
}
