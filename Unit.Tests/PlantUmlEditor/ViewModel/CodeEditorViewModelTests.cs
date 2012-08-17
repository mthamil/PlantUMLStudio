using System;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Moq;
using PlantUmlEditor.ViewModel;
using Utilities.Controls.Behaviors.AvalonEdit;
using Xunit;

namespace Unit.Tests.PlantUmlEditor.ViewModel
{
	public class CodeEditorViewModelTests
	{
		public CodeEditorViewModelTests()
		{
			codeEditor = new CodeEditorViewModel(new StubFoldingStrategy(), highlightingDefinition.Object, Enumerable.Empty<MenuViewModel>());
		}

		[Fact]
		public void Test_Content_Initialized()
		{
			// Act.
			codeEditor.Content = "initial content";

			// Assert.
			Assert.Equal("initial content", codeEditor.Content);
			Assert.False(codeEditor.IsModified);
			Assert.True(codeEditor.Document.UndoStack.IsOriginalFile);
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
			Assert.False(codeEditor.Document.UndoStack.IsOriginalFile);
		}

		[Fact]
		public void Test_Content_Undo()
		{
			// Arrange.
			codeEditor.Content = "initial content";
			codeEditor.Content = "new content";

			// Act/Assert.
			AssertThat.PropertyChanged(codeEditor, 
				p => p.IsModified, 
				() => codeEditor.Document.UndoStack.Undo());

			// Assert.
			Assert.Equal("initial content", codeEditor.Content);
			Assert.False(codeEditor.IsModified);
		}

		[Fact]
		public void Test_IsModified()
		{
			// Arrange.
			codeEditor.Content = "initial content";
			codeEditor.Content = "new content";

			// Act.
			codeEditor.IsModified = false;

			// Assert.
			Assert.True(codeEditor.Document.UndoStack.IsOriginalFile);
		}

		[Fact]
		public void Test_Dispose()
		{
			// Arrange.
			codeEditor.Content = "initial content";

			PropertyChangedEventArgs contentChangedArgs = null;
			PropertyChangedEventHandler contentChangedHandler = (o, e) => contentChangedArgs = e;
			codeEditor.PropertyChanged += contentChangedHandler;

			// Act.
			codeEditor.Dispose();
			codeEditor.Document.Replace(0, 7, "old");

			// Assert.
			Assert.False(codeEditor.IsModified);
			Assert.Null(contentChangedArgs);
		}

		private readonly CodeEditorViewModel codeEditor;
		private readonly Mock<IHighlightingDefinition> highlightingDefinition = new Mock<IHighlightingDefinition>();
	}
}