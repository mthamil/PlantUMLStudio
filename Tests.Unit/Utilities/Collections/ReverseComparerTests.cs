using System;
using System.Collections.Generic;
using Utilities.Collections;
using Xunit;

namespace Tests.Unit.Utilities.Collections
{
	public class ReverseComparerTests
	{
		[Fact]
		public void Test_ReverseComparer_NullBaseComparerThrowsException()
		{
			// Arrange.
			IComparer<int> baseComparer = null;

			// Act/Assert.
			Assert.Throws<ArgumentNullException>(() => new ReverseComparer<int>(baseComparer));
		}

		[Fact]
		public void Test_ReverseComparer_LessThan()
		{
			// Arrange.
			IComparer<int> baseComparer = Comparer<int>.Default;
			IComparer<int> reverseComparer = new ReverseComparer<int>(baseComparer);

			// Act.
			int result = baseComparer.Compare(1, 2);
			int reversedResult = reverseComparer.Compare(1, 2);

			// Assert.
			Assert.NotEqual(result, reversedResult);
			Assert.True(result < 0);
			Assert.True(reversedResult > 0);
		}

		[Fact]
		public void Test_ReverseComparer_GreaterThan()
		{
			// Arrange.
			IComparer<int> baseComparer = Comparer<int>.Default;
			IComparer<int> reverseComparer = new ReverseComparer<int>(baseComparer);

			// Act.
			int result = baseComparer.Compare(2, 1);
			int reversedResult = reverseComparer.Compare(2, 1);

			// Assert.
			Assert.NotEqual(result, reversedResult);
			Assert.True(result > 0);
			Assert.True(reversedResult < 0);
		}

		[Fact]
		public void Test_ReverseComparer_EqualTo()
		{
			// Arrange.
			IComparer<int> baseComparer = Comparer<int>.Default;
			IComparer<int> reverseComparer = new ReverseComparer<int>(baseComparer);

			// Act.
			int result = baseComparer.Compare(2, 2);
			int reversedResult = reverseComparer.Compare(2, 2);

			// Assert.
			Assert.Equal(result, reversedResult);
			Assert.Equal(0, result);
			Assert.Equal(0, reversedResult);
		}
	}
}