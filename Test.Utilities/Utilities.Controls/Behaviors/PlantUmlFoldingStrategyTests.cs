using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Utilities.Controls.Behaviors;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.Utilities.Controls.Behaviors
{
	public class PlantUmlFoldingStrategyTests
	{
		[Theory]
		[PropertyData("FoldingTestData")]
		public void Test_CreateNewFoldings(string input, IList<NewFolding> expected)
		{
			// Arrange.
			var document = new TextDocument(input);

			// Act.
			int errorOffset;
			var actual = foldingStrategy.CreateNewFoldings(document, out errorOffset).ToList();

			// Assert.
			Assert.Equal(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				var expectedFolding = expected[i];
				var actualFolding = actual[i];

				Assert.Equal(expectedFolding.StartOffset, actualFolding.StartOffset);
				Assert.Equal(expectedFolding.EndOffset, actualFolding.EndOffset);
			}
		}

		public static IEnumerable<object[]> FoldingTestData
		{
			get
			{
				yield return new object[] { 
@"note left
	jufcjsffj
	note right
		svjvjf
	end note
end note", new [] { new NewFolding(0, 65), new NewFolding(23, 55) } };

				yield return new object[] { 
@"activate A
	jufcjsffj
deactivate A", new [] { new NewFolding(0, 36) } };

				yield return new object[] { 
@"activate  A
	jufcjsffj
deactivate       A", new [] { new NewFolding(0, 43) } };

				yield return new object[] { 
@"activate A
	jufcjsffj
deactivate B", new List<NewFolding>() };

				yield return new object[] { 
@"activate A
	jufcjsffj
deactivate Adsds", new List<NewFolding>() };

				yield return new object[] { 
@"activate A jufcjsffj deactivate B", new List<NewFolding>() };

				yield return new object[] { 
@"if ihusjvcs
	jufcjsffj
endif", new List<NewFolding>() };

				yield return new object[] { 
@"if ihusjvcs then
	jufcjsffj
endif", new [] { new NewFolding(0, 35) } };

				yield return new object[] { 
@"--> [jhchjbc] if ihusjvcs then
	jufcjsffj
endif", new [] { new NewFolding(13, 49) } };

				yield return new object[] { 
@"partition icugucue {
	jufcjsffj
}", new [] { new NewFolding(0, 35) } };
			}
		}

		private readonly PlantUmlFoldingStrategy foldingStrategy = new PlantUmlFoldingStrategy();
	}
}