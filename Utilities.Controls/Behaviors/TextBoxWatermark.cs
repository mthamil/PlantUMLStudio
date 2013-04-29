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

using System.Windows;
using System.Windows.Controls;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Attached property that provides a string that can be used in conjunction with the WatermarkTextBox style.
	/// </summary>
	public static class TextBoxWatermark
	{
		/// <summary>
		/// Gets the watermark string.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static string GetWatermark(TextBox textBox)
		{
			return (string)textBox.GetValue(WatermarkProperty);
		}

		/// <summary>
		/// Sets the watermark string.
		/// </summary>
		public static void SetWatermark(TextBox textBox, string value)
		{
			textBox.SetValue(WatermarkProperty, value);
		}

		/// <summary>
		/// The watermark property.
		/// </summary>
		public static readonly DependencyProperty WatermarkProperty =
			DependencyProperty.RegisterAttached(
			"Watermark",
			typeof(string),
			typeof(TextBoxWatermark),
			new FrameworkPropertyMetadata(string.Empty));
	}
}