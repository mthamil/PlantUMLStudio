using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities.Controls.Behaviors;
using Xunit;

namespace Unit.Tests.Utilities.Controls.Behaviors
{
	public class TextBoxAutoSelectOnFocusBehaviorTests
	{
		[Fact]
		public void Test_GotKeyboardFocus_ShouldAutoSelect()
		{
			// Arrange.
			string text = "this is some text";
			textBox.Text = text;

			TextBoxAutoSelectOnFocusBehavior.SetAutoSelectOnFocus(textBox, true);

			// Preconditions.
			Assert.Equal(0, textBox.SelectionLength);
			Assert.Equal(string.Empty, textBox.SelectedText);

			// Act.
			textBox.RaiseGotKeyboardFocus();

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

			TextBoxAutoSelectOnFocusBehavior.SetAutoSelectOnFocus(textBox, true);
			TextBoxAutoSelectOnFocusBehavior.SetAutoSelectOnFocus(textBox, false);

			// Act.
			textBox.RaiseGotKeyboardFocus();

			// Assert.
			Assert.Equal(0, textBox.SelectionLength);
			Assert.Equal(string.Empty, textBox.SelectedText);
		}

		private readonly TextBoxStub textBox = new TextBoxStub();

		public class TextBoxStub : TextBox
		{
			public void RaiseGotKeyboardFocus()
			{
				var args = new KeyboardFocusChangedEventArgs(new KeyboardDeviceStub(), 100, new UIElement(), this)
				{
					RoutedEvent = GotKeyboardFocusEvent
				};
				RaiseEvent(args);
			}

			private class KeyboardDeviceStub : KeyboardDevice
			{
				public KeyboardDeviceStub() : base(InputManager.Current) { }

				#region Overrides of KeyboardDevice

				protected override KeyStates GetKeyStatesFromSystem(Key key)
				{
					return KeyStates.Down;
				}

				#endregion
			}
		}
	}
}
