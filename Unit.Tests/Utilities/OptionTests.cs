using System;
using Utilities;
using Xunit;

namespace Unit.Tests.Utilities
{
	public class OptionTests
	{
		[Fact]
		public void Test_OptionProperties()
		{
			// Test the properties of None.
			var stringNone = Option<string>.None();
			Assert.False(stringNone.HasValue);
			Assert.Throws<InvalidOperationException>(() => { var test = stringNone.Value; });

			// Test the properties of Some.
			var some = Option<string>.Some("some");
			Assert.True(some.HasValue);
			Assert.Equal("some", some.Value);
		}

		[Fact]
		public void Test_OptionNoneEquality()
		{
			// Test the singleton nature of None.
			var stringNone = Option<string>.None();
			var intNone = Option<int>.None();
			Assert.NotSame(stringNone, intNone);
			Assert.False(stringNone.Equals(intNone));

			Assert.Same(Option<string>.None(), Option<string>.None());
			Assert.Equal(Option<string>.None(), Option<string>.None());
		}

		[Fact]
		public void Test_OptionSomeEquality()
		{
			var some = Option<string>.Some("some");
			Assert.Equal(some, some);

			// Test that equality is determined by Some's value.
			var some2 = Option<string>.Some("some");
			Assert.NotSame(some, some2);
			Assert.Equal(some, some2);

			some2 = Option<string>.Some("some2");
			Assert.NotEqual(some, some2);

			var intSome = Option<int>.Some(3);
			Assert.Equal(3, intSome.Value);
			Assert.False(intSome.Equals(some2));
		}

		[Fact]
		public void Test_Option_Some_Conversion_StringLiteral()
		{
			// Act.
			Option<string> some = "test";

			// Assert.
			Assert.True(some.HasValue);
			Assert.Equal("test", some.Value);
		}

		[Fact]
		public void Test_Option_Some_Conversion_ReferenceType()
		{
			// Arrange.
			var value = new object();

			// Act.
			Option<object> some = value;

			// Assert.
			Assert.True(some.HasValue);
			Assert.Equal(value, some.Value);
		}

		[Fact]
		public void Test_Option_Some_Conversion_ValueType()
		{
			// Act.
			Option<int> some = 1;

			// Assert.
			Assert.True(some.HasValue);
			Assert.Equal(1, some.Value);
		}

		[Fact]
		public void Test_Option_None_Conversion_ReferenceType()
		{
			// Arrange.
			string value = null;

			// Act.
			Option<string> none = value;

			// Assert.
			Assert.NotNull(none);
			Assert.False(none.HasValue);
		}
	}
}