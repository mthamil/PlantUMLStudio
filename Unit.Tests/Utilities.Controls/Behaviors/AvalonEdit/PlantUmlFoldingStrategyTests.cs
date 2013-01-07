using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Utilities.Controls.Behaviors.AvalonEdit.Folding;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.Utilities.Controls.Behaviors.AvalonEdit
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
				return new TheoryDataSet<string, IList<NewFolding>>
				{
					{ 
@"note left
	jufcjsffj
	note right
		svjvjf
			note over A
				ikjhkjhfj
			end note
	end note
end note", new [] { new NewFolding(0, 109), new NewFolding(23, 99), new NewFolding(46, 88) } },

					{ 
@"activate A
	jufcjsffj
deactivate A", new [] { new NewFolding(0, 36) } },

					{ 
@"activate  A
	jufcjsffj
deactivate       A", new [] { new NewFolding(0, 43) } },

					{ 
@"activate A
	jufcjsffj
deactivate B", new List<NewFolding>() },

					{ 
@"activate A
	jufcjsffj
deactivate Adsds", new List<NewFolding>() },

					{ 
@"activate A jufcjsffj deactivate B", new List<NewFolding>() },

					{ 
@"if ihusjvcs
	jufcjsffj
endif", new List<NewFolding>() },

					{ 
@"if ihusjvcs then
	jufcjsffj
endif asas", new [] { new NewFolding(0, 40) } },	// This tests that we are not TOO strict with folding.

					{ 
@"if ihusjvcs then
	jufcjsffj
endif", new [] { new NewFolding(0, 35) } },

					{ 
@"--> [jhchjbc] if ihusjvcs then
	jufcjsffj
endif", new [] { new NewFolding(13, 49) } },

					{ 
@"partition icugucue {
	jufcjsffj
}", new [] { new NewFolding(0, 35) } },

					{ 
@"class C {
	jufcjsffj
}", new [] { new NewFolding(0, 24) } },

					{ 
@"interface I {
	jufcjsffj
}", new [] { new NewFolding(0, 28) } },

					{ 
@"state S {
	jufcjsffj
}", new [] { new NewFolding(0, 24) } },

					{ 
@"enum E {
	jufcjsffj
}", new [] { new NewFolding(0, 23) } },

					{ 
@"abstract class C {
	
}", new [] { new NewFolding(0, 24) } },

					{ 
@"package P {
	jufcjsffj
}", new [] { new NewFolding(0, 26) } },

					{ 
@"package P
	jufcjsffj
end package", new [] { new NewFolding(0, 34) } },

					{ 
@"namespace Name.space {
	jufcjsffj
}", new [] { new NewFolding(0, 37) } },

					{ 
@"title
	jufcjsffj
end title", new [] { new NewFolding(0, 28) } },

					{ 
@"title
	jufcjsffj
end titledfdfdf", new List<NewFolding>() },

					{ 
@"box ""Box""
	jufcjsffj
end box", new [] { new NewFolding(0, 30) } },

					{ 
@"box ""Box""
	jufcjsffj
end boxghghg", new List<NewFolding>() },

					{ 
@"boxsdsds ""A""
	jufcjsffj
end box", new List<NewFolding>() }

				};
			}
		}

		private readonly PatternBasedFoldingStrategy foldingStrategy = new PatternBasedFoldingStrategy(new PlantUmlFoldRegions());
	}
}