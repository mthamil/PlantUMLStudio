using System;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel.Notifications
{
	/// <summary>
	/// Represents an application notification.
	/// </summary>
	public class Notification : ViewModelBase
	{
		/// <summary>
		/// Creates a notification based on an exception.
		/// </summary>
		/// <param name="exception"></param>
		public Notification(Exception exception)
			: this()
		{
			Message = exception.Message;
		}

		public Notification()
		{
			_message = Property.New(this, p => p.Message, OnPropertyChanged);
		}

		/// <summary>
		/// A notification's message content.
		/// </summary>
		public string Message
		{
			get { return _message.Value; }
			set { _message.Value = value; }
		}

		private readonly Property<string> _message;
	}
}