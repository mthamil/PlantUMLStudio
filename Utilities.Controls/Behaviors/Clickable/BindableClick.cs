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