//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2008 Grant Frisken, Infralution (original author)
//  Originally licensed under the CodeProject Open License.
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
using System.Globalization;
using System.Windows;

namespace Utilities.Controls.Localization
{
	/// <summary>
	/// Interface for an object that provides the ability to dynamically
	/// change the UICulture of WPF Windows and controls.
	/// </summary>
	public interface ICultureManager
	{
		/// <summary>
		/// Gets or sets the UICulture for an <see cref="ICultureManager"/> and raises the <see cref="UICultureChanged"/>
		/// event causing any XAML elements using the <see cref="LocalizeExtension"/> to automatically
		/// update.
		/// </summary>
		CultureInfo UICulture { get; set; }

		/// <summary>
		/// Raised when <see cref="ICultureManager.UICulture"/> is changed.
		/// </summary>
		/// <remarks>
		/// It is advisable to use a <see cref="WeakEventManager"/> to subscribe to this event since an <see cref="ICultureManager"/>
		/// will often far outlive its multitude of listeners.
		/// </remarks>
		event EventHandler<EventArgs> UICultureChanged;
	}
}