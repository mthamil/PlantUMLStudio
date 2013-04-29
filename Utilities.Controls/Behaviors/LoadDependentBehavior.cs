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

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Provides a base class for a <see cref="T:System.Windows.Interactivity.Behavior`1"/> that requires attachment to be
	/// performed once its <c>AssociatedObject</c> is loaded.
	/// </summary>
	/// <typeparam name="T">The type the <see cref="T:System.Windows.Interactivity.Behavior`1"/> can be attached to.</typeparam>
	public abstract class LoadDependentBehavior<T> : Behavior<T> where T : FrameworkElement
	{
		protected override void OnAttached()
		{
			if (AssociatedObject.IsLoaded)
				OnLoaded();
			else
				AssociatedObject.Loaded += AssociatedObject_Loaded;
		}

		/// <summary>
		/// This method will be called once a <see cref="T:System.Windows.Interactivity.Behavior`1"/>'s <c>AssociatedObject</c> is loaded.
		/// </summary>
		protected abstract void OnLoaded();

		private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
		{
			OnLoaded();
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
		}
	}
}