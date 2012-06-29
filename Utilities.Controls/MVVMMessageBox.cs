/*
 * Author: mukapu (http://geekswithblogs.net/mukapu)
 * Feel free to use it as you want it! 
 * Comments welcome!
 * 
 * One world, one team, one goal - let's win!
 */

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utilities.Controls
{
	/// <summary>
    /// MessageBox class to wrap the standard windows message box and expose MVVM friendly.
    /// DesignTimeVisible property is set to false so it doesn't show up in the designer.
    /// </summary>
    [DesignTimeVisible(false)]
    public class MVVMMessageBox : Control
    {
		 /// <summary>
        /// Constructor
        /// </summary>
		public MVVMMessageBox()
        {
            // The control doesn't have any specific rendering of its own.
            Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// Property to trigger the display of the message box. Whenever this (implicitly the variable it is bound to) is set a true, the message box will display
        /// </summary>
        public bool Trigger
        {
            get
            {
                return (bool)GetValue(TriggerProperty);
            }
            set
            {
                SetValue(TriggerProperty, value);
            }
        }

        /// <summary>
        /// Type of message box. Can take values "OK", "OKCancel", "YesNo", "YesNoCancel". 
        /// The appropriate dependency property should be set as:
        /// If "OK", no action property is required, but one may be provided to execute when OK is selected. 
        /// For "OKCancel", OkAction and CancelAction is required.
        /// If "YesNo" and "YesNoCancel", YesAction and NoAction are required.
        /// For "YesNoCancel" in addition, CancelAction should be specified.
        /// </summary>
		public MessageBoxButton Type
        {
            get
            {
				return (MessageBoxButton)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        /// <summary>
        /// On a Ok/Cancel dialog, this will Execute the bound DelegateCommand on the user clicking "Ok".
        /// </summary>
		public ICommand OkAction
        {
            get
            {
				return (ICommand)GetValue(OkActionProperty);
            }
            set
            {
                SetValue(OkActionProperty, value);
            }
        }

        /// <summary>
        /// On a Yes/No or Yes/No/Cancel dialog, this will Execute the bound DelegateCommand on the user clicking "Yes".
        /// </summary>
		public ICommand YesAction
        {
            get
            {
				return (ICommand)GetValue(YesActionProperty);
            }
            set
            {
                SetValue(YesActionProperty, value);
            }
        }

        /// <summary>
        /// On a Yes/No or Yes/No/Cancel dialog, this will Execute the bound DelegateCommand on the user clicking "No".
        /// </summary>
		public ICommand NoAction
        {
            get
            {
				return (ICommand)GetValue(NoActionProperty);
            }
            set
            {
                SetValue(NoActionProperty, value);
            }
        }

        /// <summary>
        /// On a Yes/No/Cancel or Ok/Cancel dialog, this will Execute the bound DelegateCommand on the user clicking "Cancel".
        /// </summary>
		public ICommand CancelAction
        {
            get
            {
				return (ICommand)GetValue(NoActionProperty);
            }
            set
            {
                SetValue(NoActionProperty, value);
            }
        }

        /// <summary>
        /// The message to show the user.
        /// </summary>
        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        /// <summary>
        /// The message box caption/title to show the user.
        /// </summary>
        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for "Trigger". This also overrides the PropertyChangedCallback to trigger the message box display.
        /// </summary>
		public static readonly DependencyProperty TriggerProperty = DependencyProperty.Register("Trigger", typeof(bool), typeof(MVVMMessageBox),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTriggerChange)));
        /// <summary>
        /// DependencyProperty for "Type".
        /// </summary>
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(MessageBoxButton), typeof(MVVMMessageBox));
        /// <summary>
        /// DependencyProperty for "OkAction".
        /// </summary>
		public static readonly DependencyProperty OkActionProperty = DependencyProperty.Register("OkAction", typeof(ICommand), typeof(MVVMMessageBox));
        /// <summary>
        /// DependencyProperty for "YesAction".
        /// </summary>
		public static readonly DependencyProperty YesActionProperty = DependencyProperty.Register("YesAction", typeof(ICommand), typeof(MVVMMessageBox));
        /// <summary>
        /// DependencyProperty for "NoAction".
        /// </summary>
		public static readonly DependencyProperty NoActionProperty = DependencyProperty.Register("NoAction", typeof(ICommand), typeof(MVVMMessageBox));
        /// <summary>
        /// DependencyProperty for "CancelAction".
        /// </summary>
		public static readonly DependencyProperty CancelActionProperty = DependencyProperty.Register("CancelAction", typeof(ICommand), typeof(MVVMMessageBox));
        /// <summary>
        /// DependencyProperty for "Message".
        /// </summary>
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MVVMMessageBox));
        /// <summary>
        /// DependencyProperty for "Caption".
        /// </summary>
		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(MVVMMessageBox));

        /// <summary>
        /// The "Trigger" propery changed override. Whenever the "Trigger" property changes to true or false this will be executed.
        /// When the property changes to true, the message box will be shown.
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        private static void OnTriggerChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
			var messageBox = (MVVMMessageBox)dependencyObject;
            if (!messageBox.Trigger) return;

            switch (messageBox.Type)
            {
                case MessageBoxButton.OK:
                    messageBox.ShowInfo();
                    break;
                case MessageBoxButton.OKCancel:
                    messageBox.ShowOkCancel();
                    break;
                case MessageBoxButton.YesNo:
                    messageBox.ShowYesNo();
                    break;
                case MessageBoxButton.YesNoCancel:
                    messageBox.ShowYesNoCancel();
                    break;
            }
        }

        /// <summary>
        /// Displays the Info message box.
        /// </summary>
        private void ShowInfo()
        {
            MessageBox.Show(Message, Caption, MessageBoxButton.OK, MessageBoxImage.Information);

			if (OkAction != null)
				OkAction.Execute(null);
        }

        /// <summary>
        /// Displays the Info message box.
        /// </summary>
        private void ShowOkCancel()
        {
			ICommand action = MessageBox.Show(Message, Caption, MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.OK ? OkAction : CancelAction;

        	action.Execute(null);
        }

		/// <summary>
        /// Displays the Ok/Cancel message box and based on user action executes the appropriate command.
        /// </summary>
        private void ShowYesNo()
        {
			ICommand action = MessageBox.Show(Message, Caption, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes ? YesAction : NoAction;

        	action.Execute(null);
        }

		/// <summary>
        /// Displays the Yes/No/Cancel message box and based on user action executes the appropriate command.
        /// </summary>
        private void ShowYesNoCancel()
        {
            MessageBoxResult result = MessageBox.Show(Message, Caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

			ICommand action;
            switch (result)
            {
                case MessageBoxResult.Yes:
                    action = YesAction;
                    break;
                case MessageBoxResult.No:
                    action = NoAction;
                    break;
            	default:
                    action = CancelAction;
                    break;
            }

            action.Execute(null);
        }
    }
}
