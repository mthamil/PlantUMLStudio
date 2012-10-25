//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 

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
		/// Creates a notification with a string message.
		/// </summary>
		/// <param name="message">The notification message</param>
		public Notification(string message)
			: this()
		{
			Message = message;
			Severity = Severity.Informational;
		}

		protected Notification()
		{
			_message = Property.New(this, p => p.Message, OnPropertyChanged);
			_severity = Property.New(this, p => p.Severity, OnPropertyChanged);
		}

		/// <summary>
		/// A notification's message content.
		/// </summary>
		public string Message
		{
			get { return _message.Value; }
			protected set { _message.Value = value; }
		}

		/// <summary>
		/// A notification's severity.
		/// </summary>
		public Severity Severity
		{
			get { return _severity.Value; }
			set { _severity.Value = value; }
		}

		private readonly Property<string> _message;
		private readonly Property<Severity> _severity;
	}

	/// <summary>
	/// Notification message severity levels.
	/// </summary>
	public enum Severity
	{
		/// <summary>
		/// An informational message.
		/// </summary>
		Informational,

		/// <summary>
		/// A critical message.
		/// </summary>
		Critical
	}
}