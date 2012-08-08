using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utilities.Controls.Behaviors
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


		/// <summary>
		/// Gets the Enabled value.
		/// </summary>
		public static bool GetEnabled(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(EnabledProperty);
		}

		/// <summary>
		/// Sets the Enabled value.
		/// </summary>
		public static void SetEnabled(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(EnabledProperty, value);
		}

		/// <summary>
		/// The Enabled property used to enable the bindable IsClicked property.
		/// </summary>
		public static readonly DependencyProperty EnabledProperty =
			DependencyProperty.RegisterAttached(
			"Enabled",
			typeof(bool),
			typeof(BindableClick),
			new UIPropertyMetadata(false, OnEnabledChanged));

		private static void OnEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			bool value = (bool)e.NewValue;

			var menuItem = dependencyObject as MenuItem;
			if (menuItem == null)
			{
				var button = dependencyObject as ButtonBase;
				if (button == null)
					return;

				if (!items.Contains(button) && value)
				{
					button.Click += item_Click;
					items.Add(button);
				}
				else if (items.Contains(button) && !value)
				{
					button.Click -= item_Click;
					items.Remove(button);
				}
			}
			else
			{
				if (!items.Contains(menuItem) && value)
				{
					menuItem.Click += item_Click;
					items.Add(menuItem);
				}
				else if (items.Contains(menuItem) && !value)
				{
					menuItem.Click -= item_Click;
					items.Remove(menuItem);
				}
			}
		}

		static void item_Click(object sender, RoutedEventArgs e)
		{
			var item = e.Source as DependencyObject;
			if (item == null)
				return;

			SetIsClicked(item, true);
			SetIsClicked(item, false);	// Reset after triggering to true.
		}

		private static readonly ICollection<DependencyObject> items = new HashSet<DependencyObject>();
	}
}