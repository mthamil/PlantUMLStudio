using System.ComponentModel;
using System.Linq;
using ICSharpCode.AvalonEdit.Highlighting;
using Moq;
using PlantUmlStudio.ViewModel;
using Tests.Unit.Utilities.Controls.Behaviors.AvalonEdit;
using Utilities.Clipboard;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.ViewModel
{
	public class CodeEditorViewModelTests
	{
		public CodeEditorViewModelTests()
		{
			codeEditor = new CodeEditorViewModel(new StubFoldingStrategy(), highlightingDefinition.Object, Enumerable.Empty<MenuViewModel>(),
				clipboard.Object);
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
		public void Test_UndoCommand()
		{
			// Arrange.
			codeEditor.Content = "initial content";
			codeEditor.Content = "next content";

			// Act.
			codeEditor.UndoCommand.Execute(null);

			// Assert.
			Assert.Equal("initial content", codeEditor.Content);
		}

		[Fact]
		public void Test_RedoCommand()
		{
			// Arrange.
			codeEditor.Content = "initial content";
			codeEditor.Content = "next content";
			codeEditor.Document.UndoStack.Undo();

			// Act.
			codeEditor.RedoCommand.Execute(null);

			// Assert.
			Assert.Equal("next content", codeEditor.Content);
		}

		[Fact]
		public void Test_CopyCommand()
		{
			// Arrange.
			codeEditor.Content = "content";
			codeEditor.SelectionStart = 2;
			codeEditor.SelectionLength = 3;

			// Act.
			codeEditor.CopyCommand.Execute(null);

			// Assert.
			clipboard.Verify(c => c.SetText("nte"));
			Assert.Equal("content", codeEditor.Content);
		}

		[Fact]
		public void Test_CutCommand()
		{
			// Arrange.
			codeEditor.Content = "content";
			codeEditor.SelectionStart = 2;
			codeEditor.SelectionLength = 3;

			// Act.
			codeEditor.CutCommand.Execute(null);

			// Assert.
			clipboard.Verify(c => c.SetText("nte"));
			Assert.Equal("cont", codeEditor.Content);
		}

		[Fact]
		public void Test_PasteCommand_TextSelected()
		{
			// Arrange.
			codeEditor.Content = "content";

			clipboard.Setup(c => c.GetText()).Returns("pasted");
			codeEditor.SelectionStart = 4;
			codeEditor.SelectionLength = 3;

			// Act.
			codeEditor.PasteCommand.Execute(null);

			// Assert.
			Assert.Equal("contpasted", codeEditor.Content);
		}

		[Fact]
		public void Test_PasteCommand_NoTextSelected()
		{
			// Arrange.
			codeEditor.Content = "content";

			clipboard.Setup(c => c.GetText()).Returns("pasted");
			codeEditor.ContentIndex = 3;

			// Act.
			codeEditor.PasteCommand.Execute(null);

			// Assert.
			Assert.Equal("conpastedtent", codeEditor.Content);
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
		private readonly Mock<IClipboard> clipboard = new Mock<IClipboard>();
	}
}