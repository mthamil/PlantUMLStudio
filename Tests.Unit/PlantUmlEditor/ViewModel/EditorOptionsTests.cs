using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PlantUmlEditor.ViewModel;
using Xunit.Extensions;

namespace Tests.Unit.PlantUmlEditor.ViewModel
{
	public class EditorOptionsTests
	{
		public static IEnumerable<object[]> PropertiesData
		{
			get
			{
				return new TheoryDataSet<Expression<Func<EditorOptions, bool>>, Action<EditorOptions, bool>>
				{
					{ p => p.HighlightCurrentLine, (e, v) => e.HighlightCurrentLine = v },
					{ p => p.ShowLineNumbers, (e, v) => e.ShowLineNumbers = v },
					{ p => p.EnableVirtualSpace, (e, v) => e.EnableVirtualSpace = v },
					{ p => p.EnableWordWrap, (e, v) => e.EnableWordWrap = v },
					{ p => p.EmptySelectionCopiesEntireLine, (e, v) => e.EmptySelectionCopiesEntireLine = v },
				};
			}
		}

		[Theory]
		[PropertyData("PropertiesData")]
		public void Test_Property_Changes(Expression<Func<EditorOptions, bool>> getter, Action<EditorOptions, bool> setter)
		{
			// Act/Assert.
			AssertThat.PropertyChanged(options, 
				getter,
				() => setter(options, true));
		}

 		private readonly EditorOptions options = new EditorOptions();
	}
}