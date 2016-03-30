using System;

namespace PlantUmlStudio.ViewModel.Notifications
{
    /// <summary>
    /// Contains extension methods related to notifications.
    /// </summary>
    public static class NotificationsExtensions
    {
        /// <summary>
        /// Posts an error notification.
        /// </summary>
        public static void Error<TException>(this INotifications notifications, TException exception) where TException : Exception
        {
            notifications.Notify(new ExceptionNotification(exception));
        }
    }
}