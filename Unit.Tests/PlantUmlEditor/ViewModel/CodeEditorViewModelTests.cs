using System;
using System.Linq;
using PlantUmlEditor.ViewModel;
using Utilities.Controls.Behaviors.AvalonEdit;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class CodeEditorViewModelTests
	{
		public CodeEditorViewModelTests()
		{
			codeEditor = new CodeEditorViewModel(new StubFoldingStrategy(), new Uri(@"file:///PlantUML.xshd"), Enumerable.Empty<MenuViewModel>());
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

			// Act/Assert.
			AssertThat.PropertyChanged(codeEditor, p => p.IsModified, () =>
			{
				codeEditor.Content = "new content";
			});

			// Assert.
			Assert.Equal("new content", codeEditor.Content);
			Assert.True(codeEditor.IsModified);
		}

		private readonly CodeEditorViewModel codeEditor;
	}
}