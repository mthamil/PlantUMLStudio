using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlantUmlEditor.Controls.Behaviors
{
	/// <summary>
	/// Attached behavior that executes a command when a Control is double clicked.
	/// </summary>
	public static class DoubleClickCommandBehavior
	{
		/// <summary>
		/// Gets the command for a Control.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(Control))]
		public static ICommand GetDoubleClickCommand(Control control)
		{
			return (ICommand)control.GetValue(DoubleClickCommandProperty);
		}

		/// <summary>
		/// Sets the command for a Control.
		/// </summary>
		public static void SetDoubleClickCommand(Control control, ICommand value)
		{
			control.SetValue(DoubleClickCommandProperty, value);
		}

		/// <summary>
		/// The command property.
		/// </summary>
		public static readonly DependencyProperty DoubleClickCommandProperty =
			DependencyProperty.RegisterAttached(
			"DoubleClickCommand",
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

			if ((e.NewValue != null) && (e.OldValue == null))
			{
				control.MouseDoubleClick += control_MouseDoubleClick;
			}
			else if ((e.NewValue == null) && (e.OldValue != null))
			{
				control.MouseDoubleClick -= control_MouseDoubleClick;
			}
		}

		static void control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// Only react to the event raised by the Control
			// that was clicked. Ignore all ancestors
			// who are merely reporting that a descendant's DoubleClick fired.
			if (!ReferenceEquals(sender, e.OriginalSource))
				return;

			var control = e.OriginalSource as Control;
			if (control != null)
			{
				var command = GetDoubleClickCommand(control);
				if (command.CanExecute(control.DataContext))	// The command parameter is the current binding.
					command.Execute(control.DataContext);
			}
		}
	}
}
