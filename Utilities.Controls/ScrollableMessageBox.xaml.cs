using System;
using System.Windows;

namespace Utilities.Controls
{
	/// <summary>
	/// Interaction logic for ScrollableMessageBox.xaml
	/// </summary>
	public partial class ScrollableMessageBox : Window
	{
		/// <summary>
		/// Creates a new message box.
		/// </summary>
		public ScrollableMessageBox()
		{
			InitializeComponent();
		}

		/// <summary>
		/// The message to show the user.
		/// </summary>
		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for "Message".
		/// </summary>
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ScrollableMessageBox));

		/// <summary>
		/// The message box caption/title to show the user.
		/// </summary>
		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for "Caption".
		/// </summary>
		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(ScrollableMessageBox));

		/// <summary>
		/// The types of buttons to show on the message box.
		/// </summary>
		public MessageBoxButton Buttons
		{
			get { return _buttons; }
			set 
			{ 
				_buttons = value;
				switch (value)
				{
					case MessageBoxButton.OK:
						AffirmativeButtonText = "OK";
						NegativeButtonVisibility = Visibility.Collapsed;
						break;
					case MessageBoxButton.OKCancel:
						AffirmativeButtonText = "OK";
						NegativeButtonText = "Cancel";
						NegativeButtonVisibility = Visibility.Visible;
						break;
					case MessageBoxButton.YesNo:
						AffirmativeButtonText = "Yes";
						NegativeButtonText = "No";
						NegativeButtonVisibility = Visibility.Visible;
						break;
					default:
						throw new ArgumentOutOfRangeException("value", @"Value is not supported.");
				}
			}
		}
		private MessageBoxButton _buttons;

		/// <summary>
		/// The text of the message box's affirmative button.
		/// </summary>
		public string AffirmativeButtonText
		{
			get { return (string)GetValue(AffirmativeButtonTextProperty); }
			set { SetValue(AffirmativeButtonTextProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for "AffirmativeButtonText".
		/// </summary>
		public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(ScrollableMessageBox));

		private void AffirmativeButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		/// <summary>
		/// The text of the message box's negative button.
		/// </summary>
		public string NegativeButtonText
		{
			get { return (string)GetValue(NegativeButtonTextProperty); }
			set { SetValue(NegativeButtonTextProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for "NegativeButtonText".
		/// </summary>
		public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(ScrollableMessageBox));

		/// <summary>
		/// The visibility of the message box's negative button.
		/// </summary>
		public Visibility NegativeButtonVisibility
		{
			get { return (Visibility)GetValue(NegativeButtonVisibilityProperty); }
			set { SetValue(NegativeButtonVisibilityProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for "NegativeButtonVisibility".
		/// </summary>
		public static readonly DependencyProperty NegativeButtonVisibilityProperty = DependencyProperty.Register("NegativeButtonVisibility", typeof(Visibility), typeof(ScrollableMessageBox));

		private void NegativeButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
