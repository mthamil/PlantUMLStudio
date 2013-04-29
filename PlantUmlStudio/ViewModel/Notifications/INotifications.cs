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

namespace PlantUmlStudio.ViewModel.Notifications
{
	/// <summary>
	/// Handles centralized progress reporting registration.
	/// </summary>
	public interface INotifications
	{
		/// <summary>
		/// Creates and registers a new progress object for reporting.
		/// </summary>
		/// <param name="hasDiscreteProgress">Whether discrete progress is being reported</param>
		/// <returns>A new object for progress reporting</returns>
		IProgress<ProgressUpdate> StartProgress(bool hasDiscreteProgress = true);

		/// <summary>
		/// Posts a notification.
		/// </summary>
		/// <param name="notification">The new notification</param>
		void Notify(Notification notification);
	}
}