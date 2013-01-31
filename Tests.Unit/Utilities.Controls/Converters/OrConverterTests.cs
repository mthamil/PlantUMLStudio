using System.Globalization;
using Utilities.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.Utilities.Controls.Converters
{
	public class OrConverterTests
	{
		[Theory]
		[InlineData(true,  new[] { true,  false, false })]
		[InlineData(true,  new[] { false, true,  false })]
		[InlineData(true,  new[] { false, false, true })]
		[InlineData(true,  new[] { true,  false, true })]
		[InlineData(true,  new[] { true,  true,  true })]
		[InlineData(false, new[] { false, false, false })]
		public void Test_Convert(bool expected, bool[] input)
		{
			// Arrange.
			object[] values = new object[input.Length];
			input.CopyTo(values, 0);

			// Act.
			bool actual = (bool)converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly OrConverter converter = new OrConverter();
	}
}