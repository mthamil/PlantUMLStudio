using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Manages a textbox's focus auto selection behavior.  Extra behavior is necessary to handle
	/// focus when a textbox is clicked (instead of keyboard navigation) since ordinarily, the selected text 
	/// is cleared immediately after selection.
	/// </summary>
	public class AutoSelectOnFocusBehavior : Behavior<TextBox>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.GotMouseCapture += textBox_GotMouseCapture;
			AssociatedObject.GotFocus += textBox_GotFocus;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.GotMouseCapture -= textBox_GotMouseCapture;
			AssociatedObject.GotFocus -= textBox_GotFocus;
		}

		void textBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var source = (TextBox)e.OriginalSource;
			if (source != AssociatedObject)
				return;

			AssociatedObject.SelectAll();
			_focused = true;

			// Prevent GotMouseCapture code from executing until next focus event.
			AssociatedObject.Dispatcher.BeginInvoke(new Action<TextBox>(_ => _focused = false), DispatcherPriority.Input, AssociatedObject);
		}

		void textBox_GotMouseCapture(object sender, MouseEventArgs e)
		{
			var source = (TextBox)e.OriginalSource;
			if (source != AssociatedObject)
				return;

			if (_focused)
			{
				AssociatedObject.SelectAll();
				AssociatedObject.ReleaseMouseCapture();	// Prevent textbox from clearing selection during MouseUp.
				_focused = false;
			}
		}

		private bool _focused;
	}
}