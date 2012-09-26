using System.Windows;
using System.Windows.Interactivity;

namespace Utilities.Controls.Behaviors.Clickable
{
	/// <summary>
	/// Updates the BindableClick.IsClicked property.
	/// </summary>
	public class BindableClickAction : TriggerAction<DependencyObject>
	{
		/// <see cref="System.Windows.Interactivity.TriggerAction.Invoke"/>
		protected override void Invoke(object parameter)
		{
			BindableClick.SetIsClicked(AssociatedObject, true);
			BindableClick.SetIsClicked(AssociatedObject, false);	// Reset after triggering so that the next click will register as a change in value.
		}
	}
}