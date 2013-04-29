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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlStudio.ViewModel.Notifications
{
	/// <summary>
	/// A centralized reporting hub for application messages to display to the user.
	/// </summary>
	public class NotificationsHub : ViewModelBase, INotifications
	{
		public NotificationsHub()
		{
			_notifications = Property.New(this, p => p.Notifications as ICollection<Notification>, OnPropertyChanged);
			_notifications.Value = new ObservableCollection<Notification>();

			_latestNotification = Property.New(this, p => p.LatestNotification, OnPropertyChanged);
		}

		/// <summary>
		/// All application notifications.
		/// </summary>
		public IEnumerable<Notification> Notifications
		{
			get { return _notifications.Value; }
		}

		/// <summary>
		/// Adds a notification.
		/// </summary>
		/// <param name="notification">The new notification</param>
		private void AddNotification(Notification notification)
		{
			_notifications.Value.Add(notification);
			LatestNotification = notification;
		}

		/// <summary>
		/// Removes a notification.
		/// </summary>
		/// <param name="notification">The notification to remove</param>
		private void RemoveNotification(Notification notification)
		{
			_notifications.Value.Remove(notification);
			if (LatestNotification == notification)
				LatestNotification = _notifications.Value.LastOrDefault();
		}

		/// <summary>
		/// The latest notification.
		/// </summary>
		public Notification LatestNotification 
		{ 
			get { return _latestNotification.Value; }
			private set { _latestNotification.Value = value; }
		}

		/// <see cref="INotifications.StartProgress"/>
		public IProgress<ProgressUpdate> StartProgress(bool hasDiscreteProgress)
		{
			var progress = new Progress<ProgressUpdate>();
			var progressNotification = new ProgressNotification(progress)
			{
				HasDiscreteProgress = hasDiscreteProgress
			};
			AddNotification(progressNotification);

			return progress;
		}

		/// <see cref="INotifications.Notify"/>
		public void Notify(Notification notification)
		{
			AddNotification(notification);
		}

		private readonly Property<Notification> _latestNotification; 
		private readonly Property<ICollection<Notification>> _notifications;
	}
}