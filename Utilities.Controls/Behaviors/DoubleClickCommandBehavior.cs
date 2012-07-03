using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Attached behavior that executes a command when a Control is double clicked.
	/// </summary>
	public static class DoubleClickCommandBehavior
	{
		/// <summary>
		/// Gets the double click command parameter for a Control.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(Control))]
		public static object GetCommandParameter(Control control)
		{
			return control.GetValue(CommandParameterProperty);
		}

		/// <summary>
		/// Sets the double click command parameter for a Control.
		/// </summary>
		public static void SetCommandParameter(Control control, object value)
		{
			control.SetValue(CommandParameterProperty, value);
		}

		/// <summary>
		/// The command property.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(DoubleClickCommandBehavior),
			new UIPropertyMetadata(null));

		/// <summary>
		/// Gets the double click command for a Control.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(Control))]
		public static ICommand GetCommand(Control control)
		{
			return (ICommand)control.GetValue(CommandProperty);
		}

		/// <summary>
		/// Sets the double click command for a Control.
		/// </summary>
		public static void SetCommand(Control control, ICommand value)
		{
			control.SetValue(CommandProperty, value);
		}

		/// <summary>
		/// The command property.
		/// </summary>
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(DoubleClickCommandBehavior),
			new UIPropertyMetadata(null, OnCommandChanged));

		static void OnCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			var control = depObj as Control;
			if (control == null)
				return;

			var newCommand = e.NewValue as ICommand;
			if (newCommand == null)
				return;

			if (e.OldValue != null)
				control.MouseDoubleClick -= control_MouseDoubleClick;

			if (e.NewValue != null)
				control.MouseDoubleClick += control_MouseDoubleClick;
		}

		static void control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (!ReferenceEquals(sender, e.Source))
				return;

			var control = e.Source as Control;
			if (control != null)
			{
				var command = GetCommand(control);
				var commandParameter = GetCommandParameter(control);
				if (command.CanExecute(commandParameter))
					command.Execute(commandParameter);
			}
		}
	}
}
