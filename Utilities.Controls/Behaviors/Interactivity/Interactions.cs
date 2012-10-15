using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interactivity;

namespace Utilities.Controls.Behaviors.Interactivity
{
	/// <summary>
	/// Contains functionality related to System.Windows.Interaction. Primarily, attached
	/// properties are provided which allow adding Triggers and Behaviors to a DependencyObject
	/// through the use of Styles.
	/// </summary>
	/// <remarks>If used in a style, Triggers and Behaviors must use the x:Shared="False" attribute.</remarks>
	public static class Interactions
	{
		/// <summary>
		/// Allows adding behaviors to an object.
		/// </summary>
		public static void SetBehaviors(DependencyObject dependencyObject, Behaviors value)
		{
			dependencyObject.SetValue(BehaviorsProperty, value);
		}

		/// <summary>
		/// Gets an object's behaviors.
		/// </summary>
		public static Behaviors GetBehaviors(DependencyObject dependencyObject)
		{
			return (Behaviors)dependencyObject.GetValue(BehaviorsProperty);
		}

		/// <summary>
		/// The Behaviors attached property.
		/// </summary>
		public static readonly DependencyProperty BehaviorsProperty =
			DependencyProperty.RegisterAttached(
				"Behaviors",
				typeof(Behaviors),
				typeof(Interactions),
				new PropertyMetadata(default(Behaviors), OnBehaviorsPropertyChanged));

		private static void OnBehaviorsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null)
				return;

			// Add new Behaviors to the BehaviorCollection.
			var behaviors = Interaction.GetBehaviors(dependencyObject);
			foreach (var behavior in (Behaviors)e.NewValue)
				behaviors.Add(behavior);
		}

		/// <summary>
		/// Allows adding triggers to an object.
		/// </summary>
		public static void SetTriggers(DependencyObject dependencyObject, Triggers value)
		{
			dependencyObject.SetValue(TriggersProperty, value);
		}

		/// <summary>
		/// Gets an object's triggers.
		/// </summary>
		public static Triggers GetTriggers(DependencyObject dependencyObject)
		{
			return (Triggers)dependencyObject.GetValue(TriggersProperty);
		}

		/// <summary>
		/// The Triggers attached property.
		/// </summary>
		public static readonly DependencyProperty TriggersProperty =
			DependencyProperty.RegisterAttached(
				"Triggers",
				typeof(Triggers),
				typeof(Interactions),
				new PropertyMetadata(default(Triggers), OnTriggersPropertyChanged));

		private static void OnTriggersPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null)
				return;

			// Add new Triggers to the TriggerCollection.
			var triggers = Interaction.GetTriggers(dependencyObject);
			foreach (var trigger in (Triggers)e.NewValue)
				triggers.Add(trigger);
		}
	}

	/// <summary>
	/// A collection of triggers.
	/// If used in a style, the x:Shared="False" attribute must be used.
	/// </summary>
	public class Triggers : Collection<System.Windows.Interactivity.TriggerBase> { }

	/// <summary>
	/// A collection of behaviors.
	/// If used in a style, the x:Shared="False" attribute must be used.
	/// </summary>
	public class Behaviors : Collection<Behavior> { }
}