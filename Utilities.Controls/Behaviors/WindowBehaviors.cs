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