using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Contains attached properties and behaviors for the Window class.
	/// </summary>
	public static class WindowBehaviors
	{
		/// <summary>
		/// Gets the Closing command parameter for a Window.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static object GetClosingCommandParameter(Window window)
		{
			return window.GetValue(ClosingCommandParameterProperty);
		}

		/// <summary>
		/// Sets the Closing command parameter for a Window.
		/// </summary>
		public static void SetClosingCommandParameter(Window window, object value)
		{
			window.SetValue(ClosingCommandParameterProperty, value);
		}

		/// <summary>
		/// The Closing command property.
		/// </summary>
		public static readonly DependencyProperty ClosingCommandParameterProperty =
			DependencyProperty.RegisterAttached(
			"ClosingCommandParameter",
			typeof(object),
			typeof(WindowBehaviors),
			new UIPropertyMetadata(null));

		/// <summary>
		/// Gets the Closing command for a Window.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static ICommand GetClosingCommand(Window window)
		{
			return (ICommand)window.GetValue(ClosingCommandProperty);
		}

		/// <summary>
		/// Sets the Closingcommand for a Window.
		/// </summary>
		public static void SetClosingCommand(Window window, ICommand value)
		{
			window.SetValue(ClosingCommandProperty, value);
		}

		/// <summary>
		/// The Closing command property.
		/// </summary>
		public static readonly DependencyProperty ClosingCommandProperty =
			DependencyProperty.RegisterAttached(
			"ClosingCommand",
			typeof(ICommand),
			typeof(WindowBehaviors),
			new UIPropertyMetadata(null, OnCommandChanged));

		static void OnCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var window = dependencyObject as Window;
			if (window == null)
				return;

			var newCommand = e.NewValue as ICommand;
			if (newCommand == null)
				return;

			if (e.OldValue != null)
				window.Closing -= window_Closing;

			if (e.NewValue != null)
				window.Closing += window_Closing;
		}

		static void window_Closing(object sender, CancelEventArgs e)
		{
			var window = sender as Window;
			if (window != null)
			{
				var command = GetClosingCommand(window);
				if (command != null)
				{
					var commandParameter = GetClosingCommandParameter(window);
					if (command.CanExecute(commandParameter))
						command.Execute(commandParameter);
				}
			}
		}
	}
}