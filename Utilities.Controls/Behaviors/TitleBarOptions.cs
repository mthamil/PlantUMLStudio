//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Behavior that enables configuration of a Window's titlebar.
	/// </summary>
	public class TitleBarOptions : LoadDependentBehavior<Window>
	{
		/// <see cref="LoadDependentBehavior{T}.OnLoaded"/>
		protected override void OnLoaded()
		{
			SetButtonVisibility(AssociatedObject, ShowButtons);
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

		private static void SetButtonVisibility(Window window, bool isVisible)
		{
            long visibilityFlag = isVisible ? NativeMethods.WS_SYSMENU : ~NativeMethods.WS_SYSMENU;

			var hwnd = new WindowInteropHelper(window).Handle;
		    var newFlagValue = new IntPtr((long)NativeMethods.GetWindowLongPtr(hwnd, NativeMethods.GWL_STYLE) & visibilityFlag);
            NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_STYLE, newFlagValue);
		}

	    private static class NativeMethods
	    {
	        public const int GWL_STYLE = -16;
            public const long WS_SYSMENU = 0x00080000L;

            public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
            {
                return IntPtr.Size == 4 
                    ? GetWindowLongPtr32(hWnd, nIndex) 
                    : GetWindowLongPtr64(hWnd, nIndex);
            }

	        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
            static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
            static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

            public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
            {
                return IntPtr.Size == 4 
                    ? SetWindowLongPtr32(hWnd, nIndex, dwNewLong) 
                    : SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }

	        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
            static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
            static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
	    }
	}
}