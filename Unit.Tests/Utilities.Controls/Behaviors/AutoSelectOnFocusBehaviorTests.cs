using System.Windows;
using System.Windows.Controls;
using Utilities.Controls.Behaviors;
using Xunit;

namespace Unit.Tests.Utilities.Controls.Behaviors
{
	public class AutoSelectOnFocusBehaviorTests
	{
		[Fact]
		public void Test_GotKeyboardFocus_ShouldAutoSelect()
		{
			// Arrange.
			string text = "this is some text";
			textBox.Text = text;

			var behavior = new AutoSelectOnFocusBehavior();
			behavior.Attach(textBox);

			// Preconditions.
			Assert.Equal(0, textBox.SelectionLength);
			Assert.Equal(string.Empty, textBox.SelectedText);

			// Act.
			textBox.RaiseGotFocus();

			// Assert.
			Assert.Equal(text.Length, textBox.SelectionLength);
			Assert.Equal(text, textBox.SelectedText);
		}

		[Fact]
		public void Test_SettingToFalse_ShouldUnsubscribe()
		{
			// Arrange.
			string text = "this is some text";
			textBox.Text = text;

			var behavior = new AutoSelectOnFocusBehavior();
			behavior.Attach(textBox);
			behavior.Detach();

			// Act.
			textBox.RaiseGotFocus();

			// Assert.
			Assert.Equal(0, textBox.SelectionLength);
			Assert.Equal(string.Empty, textBox.SelectedText);
		}

		private readonly TextBoxStub textBox = new TextBoxStub();

		public class TextBoxStub : TextBox
		{
			public void RaiseGotFocus()
			{
				var args = new RoutedEventArgs(GotFocusEvent, this);
				RaiseEvent(args);
			}
		}
	}
}
