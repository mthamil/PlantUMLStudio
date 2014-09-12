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
using System.ComponentModel;
using System.Linq.Expressions;
using Utilities.Reflection;

namespace Utilities.Observable
{
    /// <summary>
    /// Base class for objects that notify observers about changes to their state.
    /// </summary>
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		/// <see cref="INotifyPropertyChanged.PropertyChanged"/>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed</param>
		protected void OnPropertyChanged(string propertyName)
		{
			var localEvent = PropertyChanged;
			if (localEvent != null)
				localEvent(this, new PropertyChangedEventArgs(propertyName));
		}
	}

    /// <summary>
    /// Base class for objects that notify observers about changes to their state that uses static polymorphism to
    /// safely reference property names.
    /// </summary>
	public abstract class ObservableObject<T> : ObservableObject
	{
		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="property">The property that changed</param>
		protected void OnPropertyChanged(Expression<Func<T, object>> property)
		{
			OnPropertyChanged(Reflect.PropertyOf(property).Name);
		}
	}
}