using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel.Notifications
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