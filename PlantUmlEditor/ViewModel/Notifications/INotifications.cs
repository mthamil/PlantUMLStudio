using System;

namespace PlantUmlEditor.ViewModel.Notifications
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