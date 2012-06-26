using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PlantUmlEditor.Controls.Behaviors
{
	/// <summary>
	/// Attached property that, if true, will automatically select the text in a textbox when it receives focus.
	/// </summary>
	public static class TextBoxAutoSelectOnFocusBehavior
	{
		/// <summary>
		/// Gets the property value.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static bool GetAutoSelectOnFocus(TextBox textBox)
		{
			return (bool)textBox.GetValue(AutoSelectOnFocusProperty);
		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		public static void SetAutoSelectOnFocus(TextBox textBox, bool value)
		{
			textBox.SetValue(AutoSelectOnFocusProperty, value);
		}

		/// <summary>
		/// The AutoSelectOnFocus dependency property.
		/// </summary>
		public static readonly DependencyProperty AutoSelectOnFocusProperty =
			DependencyProperty.RegisterAttached(
			"AutoSelectOnFocus",
			typeof(bool),
			typeof(TextBoxAutoSelectOnFocusBehavior),
			new UIPropertyMetadata(false, OnAutoSelectOnFocusChanged));

		private static void OnAutoSelectOnFocusChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			TextBox textBox = depObj as TextBox;
			if (textBox == null)
				return;

			if (!Equals(e.NewValue, e.OldValue))
			{
				bool newValue = (bool)e.NewValue;

				if (newValue)
				{
					if (!behaviors.ContainsKey(textBox))
						behaviors.Add(textBox, new FocusAutoSelectionBehavior(textBox));
				}
				else
				{
					FocusAutoSelectionBehavior behavior;
					if (behaviors.TryGetValue(textBox, out behavior))
					{
						behavior.Unregister();
						behaviors.Remove(textBox);
					}
				}
			}
		}

		/// <summary>
		/// Manages a textbox's focus auto selection behavior.  Extra behavior is necessary to handle
		/// focus when a textbox is clicked (instead of keyboard navigation) since ordinarily, the selected text 
		/// is cleared immediately after selection.
		/// </summary>
		private class FocusAutoSelectionBehavior
		{
			public FocusAutoSelectionBehavior(TextBox textBox)
			{
				_textBox = textBox;
				_textBox.GotKeyboardFocus += textBox_GotKeyboardFocus;
				_textBox.GotMouseCapture += textBox_GotMouseCapture;
			}

			void textBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
			{
				var source = (TextBox)e.OriginalSource;
				if (source != _textBox)
					return;

				_textBox.SelectAll();
				_focused = true;

				// Prevent GotMouseCapture code from executing until next focus event.
				_textBox.Dispatcher.BeginInvoke(new Action<TextBox>(_ => _focused = false), DispatcherPriority.Input, _textBox);
			}

			void textBox_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
			{
				var source = (TextBox)e.OriginalSource;
				if (source != _textBox)
					return;

				if (_focused)
				{
					_textBox.SelectAll();
					_textBox.ReleaseMouseCapture();	// Prevent textbox from clearing selection during MouseUp.
					_focused = false;
				}
			}

			public void Unregister()
			{
				_textBox.GotKeyboardFocus -= textBox_GotKeyboardFocus;
				_textBox.GotMouseCapture -= textBox_GotMouseCapture;
			}

			private readonly TextBox _textBox;
			private bool _focused;
		}

		private static readonly IDictionary<TextBox, FocusAutoSelectionBehavior> behaviors = new ConcurrentDictionary<TextBox, FocusAutoSelectionBehavior>();
	}
}