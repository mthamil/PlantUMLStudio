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
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlStudio.ViewModel.Notifications
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
			_summary = Property.New(this, p => p.Summary, OnPropertyChanged);
			_hasMoreInfo = Property.New(this, p => p.HasMoreInfo, OnPropertyChanged);
			_message = Property.New(this, p => p.Message, OnPropertyChanged);
			_severity = Property.New(this, p => p.Severity, OnPropertyChanged);
		}

		/// <summary>
		/// An abbreviated summary of a notification's message.
		/// </summary>
		public string Summary
		{
			get { return _summary.Value; }
			private set { _summary.Value = value; }
		}

		/// <summary>
		/// Whether a notifcation's summary does not contain the full message.
		/// </summary>
		public bool HasMoreInfo
		{
			get { return _hasMoreInfo.Value; }
			private set { _hasMoreInfo.Value = value; }
		}

		/// <summary>
		/// A notification's full message content.
		/// </summary>
		public string Message
		{
			get { return _message.Value; }
			protected set 
			{ 
				if (_message.TrySetValue(value))
					CreateSummary();
			}
		}

		private void CreateSummary()
		{
			int newLineIndex = Message.IndexOf(Environment.NewLine);
			int summaryLength = Math.Min(MAX_SUMMARY_CHARS, newLineIndex > -1 ? newLineIndex : Message.Length);
			var summary = Message.Substring(0, summaryLength);
			HasMoreInfo = summary != Message;
			Summary = summary;
		}

		/// <summary>
		/// A notification's severity.
		/// </summary>
		public Severity Severity
		{
			get { return _severity.Value; }
			set { _severity.Value = value; }
		}

		private readonly Property<string> _summary;
		private readonly Property<bool> _hasMoreInfo; 
		private readonly Property<string> _message;
		private readonly Property<Severity> _severity;

		private const int MAX_SUMMARY_CHARS = 100;
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