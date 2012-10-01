using System;
using System.Globalization;
using Utilities;
using Xunit;
using Xunit.Extensions;

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
		public void Test_Option_Some_Conversion_ValueType()
		{
			// Act.
			Option<int> some = 1;

			// Assert.
			Assert.True(some.HasValue);
			Assert.Equal(1, some.Value);
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

		[Theory]
		[InlineData(true, "value")]
		[InlineData(false, null)]
		public void Test_Option_From(bool expectValue, string value)
		{
			// Act.
			Option<string> option = Option<string>.From(value);

			// Assert.
			Assert.Equal(expectValue, option.HasValue);
		}

		[Fact]
		public void Test_Some_Select()
		{
			// Arrange.
			Option<int> someInt = 1;

			// Act.
			Option<string> result = someInt.Select(x => x.ToString(CultureInfo.InvariantCulture));
			Option<string> linqResult = from x in someInt select x.ToString(CultureInfo.InvariantCulture);

			// Assert.
			Assert.True(result.HasValue);
			Assert.Equal("1", result.Value);

			Assert.True(linqResult.HasValue);
			Assert.Equal("1", linqResult.Value);
		}

		[Fact]
		public void Test_Some_SelectMany()
		{
			// Arrange.
			Option<int> someInt = 1;

			// Act.
			Option<string> result = someInt.SelectMany(x => new ReturnsOption(x).Get(true));
			Option<string> linqResult = 
				from x in someInt
				from y in new ReturnsOption(x).Get(true)
				select y;

			// Assert.
			Assert.True(result.HasValue);
			Assert.Equal("1", result.Value);

			Assert.True(linqResult.HasValue);
			Assert.Equal("1", linqResult.Value);
		}

		[Fact]
		public void Test_Some_SelectMany_NoResult()
		{
			// Arrange.
			Option<int> someInt = 1;

			// Act.
			Option<string> result = someInt.SelectMany(x => new ReturnsOption(x).Get(false));
			Option<string> linqResult =
				from x in someInt
				from y in new ReturnsOption(x).Get(false)
				select y;

			// Assert.
			Assert.False(result.HasValue);
			Assert.False(linqResult.HasValue);
		}

		[Fact]
		public void Test_None_Select()
		{
			// Arrange.
			var noneInt = Option<int>.None();

			// Act.
			Option<string> result = noneInt.Select(x => x.ToString(CultureInfo.InvariantCulture));
			Option<string> linqResult = from x in noneInt select x.ToString(CultureInfo.InvariantCulture);

			// Assert.
			Assert.False(result.HasValue);
			Assert.False(linqResult.HasValue);
		}

		[Fact]
		public void Test_None_SelectMany()
		{
			// Arrange.
			var noneInt = Option<int>.None();

			// Act.
			Option<string> result = noneInt.SelectMany(x => new ReturnsOption(x).Get(true));
			Option<string> linqResult =
				from x in noneInt
				from y in new ReturnsOption(x).Get(true)
				select y;

			// Assert.
			Assert.False(result.HasValue);
			Assert.False(linqResult.HasValue);
		}

		private class ReturnsOption
		{
			private readonly int _val;

			public ReturnsOption(int val)
			{
				_val = val;
			}

			public Option<string> Get(bool shouldHaveValue)
			{
				if (shouldHaveValue)
					return Option<string>.Some(_val.ToString(CultureInfo.InvariantCulture));

				return Option<string>.None();
			}
		}
	}
}