using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Attached property that, if true, will hide a window's close button since WPF
	/// does not have built-in support for it.
	/// </summary>
	public static class HideCloseButtonBehavior
	{
		/// <summary>
		/// Gets the property value.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static bool GetHideCloseButton(Window window)
		{
			return (bool)window.GetValue(HideCloseButtonProperty);
		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		public static void SetHideCloseButton(Window window, bool value)
		{
			window.SetValue(HideCloseButtonProperty, value);
		}

		/// <summary>
		/// The HideCloseButton dependency property.
		/// </summary>
		public static readonly DependencyProperty HideCloseButtonProperty =
			DependencyProperty.RegisterAttached(
			"HideCloseButton",
			typeof(bool),
			typeof(HideCloseButtonBehavior),
			new UIPropertyMetadata(false, OnHideCloseButtonChanged));

		private static void OnHideCloseButtonChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var window = dependencyObject as Window;
			if (window == null)
				return;

			var newHideCloseButton = (bool)e.NewValue;
			var oldHideCloseButton = (bool)e.OldValue;
			if (newHideCloseButton && !oldHideCloseButton)
			{
				if (!window.IsLoaded)
					window.Loaded += window_Loaded;
				else
					SetCloseButtonVisibility(window, false);
			}
			else if (!newHideCloseButton && oldHideCloseButton)
			{
				if (!window.IsLoaded)
					window.Loaded -= window_Loaded;
				else
					SetCloseButtonVisibility(window, true);
			}
		}

		/// <summary>
		/// If a window is not yet loaded, we cannot change the visibility of the close button,
		/// so this event handler is used to wait until the appropriate time.
		/// </summary>
		private static void window_Loaded(object sender, RoutedEventArgs args)
		{
			var window = sender as Window;
			if (window == null)
				return;

			SetCloseButtonVisibility(window, false);
			window.Loaded -= window_Loaded;
		}

		private static void SetCloseButtonVisibility(Window window, bool shouldBeVisible)
		{
			int visibilityFlag = shouldBeVisible ? WS_SYSMENU : ~WS_SYSMENU;

			var hwnd = new WindowInteropHelper(window).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & visibilityFlag);
		}

		#region Win32 imports

		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		#endregion
	}
}