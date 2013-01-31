using System;
using System.Collections.Generic;
using System.Windows;
using Utilities.Controls.Selectors;
using Xunit;

namespace Tests.Unit.Utilities.Controls.Selectors
{
	public class TypeMapDataTemplateSelectorTests
	{
		[Fact]
		public void Test_ExactMatch()
		{
			// Arrange.
			var template = new DataTemplate { DataType = typeof(string) };

			var selector = new TypeMapDataTemplateSelector(new List<DataTemplate>
			{
				new DataTemplate { DataType = typeof(int) },
				template
			});

			// Act.
			var actual = selector.SelectTemplate("test", null);

			// Assert.
			Assert.Equal(template, actual);
		}

		[Fact]
		public void Test_NoMatch()
		{
			// Arrange.
			var selector = new TypeMapDataTemplateSelector(new List<DataTemplate>
			{
				new DataTemplate { DataType = typeof(int) },
				new DataTemplate { DataType = typeof(string) }
			});

			// Act.
			var actual = selector.SelectTemplate(new object(), null);

			// Assert.
			Assert.Null(actual);
		}

		[Fact]
		public void Test_NonExactMatch()
		{
			// Arrange.
			var broadTemplate = new DataTemplate { DataType = typeof(IComparable<string>) };
			var moreBroadTemplate = new DataTemplate { DataType = typeof(IConvertible) };

			var selector = new TypeMapDataTemplateSelector(new List<DataTemplate>
			{
				moreBroadTemplate,
				broadTemplate
			});

			// Act.
			var actual = selector.SelectTemplate("test", null);

			// Assert.
			Assert.Equal(moreBroadTemplate, actual);
		}

		[Fact]
		public void Test_NonExactMatch_OrderMatters()
		{
			// Arrange.
			var broadTemplate = new DataTemplate { DataType = typeof(IComparable<string>) };
			var moreBroadTemplate = new DataTemplate { DataType = typeof(IConvertible) };

			var selector = new TypeMapDataTemplateSelector(new List<DataTemplate>
			{
				broadTemplate,
				moreBroadTemplate
			});

			// Act.
			var actual = selector.SelectTemplate("test", null);

			// Assert.
			Assert.Equal(broadTemplate, actual);
		} 
	}
}