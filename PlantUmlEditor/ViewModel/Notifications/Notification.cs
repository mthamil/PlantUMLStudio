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