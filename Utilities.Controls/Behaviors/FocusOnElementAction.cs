using System;
using System.Windows;
using System.Windows.Interactivity;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// An action that focuses on a given UI element.
	/// </summary>
	public class FocusOnElementAction : TriggerAction<UIElement>
	{
		#region Overrides of TriggerAction

		/// <see cref="System.Windows.Interactivity.TriggerAction.Invoke"/>
		protected override void Invoke(object parameter)
		{
			Dispatcher.BeginInvoke(new Action(() => ElementToFocus.Focus()));
		}

		#endregion

		/// <summary>
		/// The element to focus on.
		/// </summary>
		public UIElement ElementToFocus
		{
			get { return (UIElement)GetValue(ElementToFocusProperty); }
			set { SetValue(ElementToFocusProperty, value); }
		}

		/// <summary>
		/// The ElementToFocus dependency property.
		/// </summary>
		public static readonly DependencyProperty ElementToFocusProperty =
			DependencyProperty.Register(
			"ElementToFocus",
			typeof(UIElement),
			typeof(FocusOnElementAction),
			new UIPropertyMetadata(default(UIElement)));
	}
}