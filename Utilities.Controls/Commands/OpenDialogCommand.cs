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
using System.Windows;

namespace Utilities.Controls.Commands
{
	/// <summary>
	/// Creates and displays a window of a given type.
	/// </summary>
	public class OpenDialogCommand : DependencyCommandBase
	{
		/// <summary>
		/// Creates and opens a new instance of the specified Window
		/// type.
		/// </summary>
		/// <param name="parameter">
		/// The data context to set on the window. If null, 
		/// the data context will not be set, preventing an override
		/// of any existing context.
		/// </param>
		public override void Execute(object parameter)
		{
			var window = (Window)Activator.CreateInstance(Type);
			if (parameter != null)
				window.DataContext = parameter;

			if (Owner != null)
				window.Owner = Owner;

			window.ShowDialog();
		}

		/// <summary>
		/// The type of window to create.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// The new window's owner.
		/// </summary>
		public Window Owner
		{
			get { return (Window)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}

		/// <summary>
		/// The Owner dependency property.
		/// </summary>
		public static readonly DependencyProperty OwnerProperty =
			DependencyProperty.RegisterAttached(
			"Owner",
			typeof(Window),
			typeof(OpenDialogCommand),
			new FrameworkPropertyMetadata(null));
	}
}