using System.Linq;
using PlantUmlEditor.ViewModel;
using Utilities.Mvvm;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class CodeEditorViewModelTests
	{
		public CodeEditorViewModelTests()
		{
			codeEditor = new CodeEditorViewModel(Enumerable.Empty<ViewModelBase>());
		}

		[Fact]
		public void Test_Content_Initialized()
		{
			// Act.
			codeEditor.Content = "initial content";

			// Assert.
			Assert.Equal("initial content", codeEditor.Content);
			Assert.False(codeEditor.IsModified);
		}

		[Fact]
		public void Test_Content_Changed()
		{
			// Arrange.
			codeEditor.Content = "initial content";

			// Act.
			codeEditor.Content = "new content";

			// Assert.
			Assert.Equal("new content", codeEditor.Content);
			Assert.True(codeEditor.IsModified);
		}

		private readonly CodeEditorViewModel codeEditor;
	}
}