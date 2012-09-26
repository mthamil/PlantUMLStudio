using System.Windows;

namespace Utilities.Controls.Behaviors.Clickable
{
	/// <summary>
	/// Provides a bindable property useful as a trigger when a Button or MenuItem is clicked.
	/// </summary>
	public static class BindableClick
	{
		/// <summary>
		/// Gets the IsClicked value.
		/// </summary>
		public static bool GetIsClicked(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(IsClickedProperty);
		}

		/// <summary>
		/// Sets the IsClicked value.
		/// </summary>
		public static void SetIsClicked(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(IsClickedProperty, value);
		}

		/// <summary>
		/// The IsClicked property.
		/// </summary>
		public static readonly DependencyProperty IsClickedProperty =
			DependencyProperty.RegisterAttached(
				"IsClicked",
				typeof(bool),
				typeof(BindableClick),
				new UIPropertyMetadata(false));
	}
}