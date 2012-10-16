//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Behavior that enables configuration of a Window's titlebar.
	/// </summary>
	public class TitleBarOptions : Behavior<Window>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			if (AssociatedObject.IsLoaded)
				SetButtonVisibility(AssociatedObject, ShowButtons);
			else
				AssociatedObject.Loaded += AssociatedObject_Loaded;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
		}

		/// <summary>
		/// Gets or sets whether a Window's titlebar buttons are visible.
		/// </summary>
		public bool ShowButtons
		{
			get { return (bool)GetValue(ShowButtonsProperty); }
			set { SetValue(ShowButtonsProperty, value); }
		}

		/// <summary>
		/// The ShowButtons dependency property.
		/// </summary>
		public static readonly DependencyProperty ShowButtonsProperty =
			DependencyProperty.Register(
			"ShowButtons",
			typeof(bool),
			typeof(TitleBarOptions),
			new UIPropertyMetadata(false, ShowButtonsChanged));

		private static void ShowButtonsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (TitleBarOptions)dependencyObject;
			if (behavior.AssociatedObject != null)
				SetButtonVisibility(behavior.AssociatedObject, (bool)e.NewValue);
		}

		void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
		{
			SetButtonVisibility(AssociatedObject, ShowButtons);
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
		}

		private static void SetButtonVisibility(Window window, bool isVisible)
		{
			long visibilityFlag = isVisible ? WS_SYSMENU : ~WS_SYSMENU;

			var hwnd = new WindowInteropHelper(window).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & visibilityFlag);
		}

		#region Win32 imports

		private const int GWL_STYLE = -16;
		private const long WS_SYSMENU = 0x00080000L;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

		#endregion
	}
}