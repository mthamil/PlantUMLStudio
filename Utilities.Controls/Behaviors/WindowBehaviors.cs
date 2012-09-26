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
		/// Sets a window's dialog result.
		/// </summary>
		public static void SetDialogResult(Window window, bool? value)
		{
			window.SetValue(DialogResultProperty, value);
		}

		/// <summary>
		/// Gets a window's dialog result.
		/// </summary>
		public static bool? GetDialogResult(Window window)
		{
			return (bool?)window.GetValue(DialogResultProperty);
		}

		/// <summary>
		/// The BindableDialogResult attached property.
		/// </summary>
		public static readonly DependencyProperty DialogResultProperty =
			DependencyProperty.RegisterAttached(
				"DialogResult",
				typeof(bool?),
				typeof(WindowBehaviors),
				new PropertyMetadata(default(bool?), OnBindableDialogResultChanged));

		private static void OnBindableDialogResultChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var window = dependencyObject as Window;
			if (window == null)
				return;

			window.DialogResult = (bool?)e.NewValue;
		}
	}
}