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