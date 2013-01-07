using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using PlantUmlEditor.Converters;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.PlantUmlEditor.Converters
{
	public class DiagramTabNameConverterTests
	{
		[Theory]
		[InlineData("name", "name", false)]
		[InlineData("name!", "name", true)]
		public void Test_Convert(string expected, string name, bool isModified)
		{
			// Act.
			var actual = converter.Convert(new object[] { name, isModified }, null, null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[PropertyData("InvalidData")]
		public void Test_Convert_InvalidArguments(object[] values)
		{
			// Act.
			var converted = converter.Convert(values, null, null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(DependencyProperty.UnsetValue, converted);
		}

		public static IEnumerable<object[]> InvalidData
		{
			get
			{
				return new TheoryDataSet<object[]>
				{
					null,
					new object[] { "test", true, 1 },
					new object[] { 1, true },
					new object[] { "test", 1 }
				};
			}
		}

		private readonly DiagramTabNameConverter converter = new DiagramTabNameConverter { ModifiedFormat = "{0}!" };
	}
}