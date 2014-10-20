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
using SharpEssentials.Observable;

namespace PlantUmlStudio.ViewModel.Notifications
{
	/// <summary>
	/// A class used to represent progress information for tasks.
	/// </summary>
	public class ProgressNotification : Notification, IProgressNotification
	{
		public ProgressNotification(Progress<ProgressUpdate> progress)
			: this()
		{
			progress.ProgressChanged += progress_ProgressChanged;
		}

		public ProgressNotification()
		{
			_hasDiscreteProgress = Property.New(this, p => p.HasDiscreteProgress, OnPropertyChanged);
		    _percentComplete = Property.New(this, p => p.PercentComplete, OnPropertyChanged)
		                               .AlsoChanges(p => p.InProgress);
		}

		/// <see cref="IProgressNotification.HasDiscreteProgress"/>
		public bool HasDiscreteProgress
		{
			get { return _hasDiscreteProgress.Value; }
			set { _hasDiscreteProgress.Value = value; }
		}

		/// <see cref="IProgressNotification.InProgress"/>
		public bool InProgress
		{
			get { return PercentComplete.HasValue; }
		}

		/// <see cref="IProgressNotification.PercentComplete"/>
		public int? PercentComplete
		{
			get { return _percentComplete.Value; }
			set { _percentComplete.Value = value; }
		}

		void progress_ProgressChanged(object sender, ProgressUpdate e)
		{
			PercentComplete = e.PercentComplete;
			Message = e.Message;

			if (e.IsFinished)
				((Progress<ProgressUpdate>)sender).ProgressChanged -= progress_ProgressChanged;
		}

		private readonly Property<bool> _hasDiscreteProgress;
		private readonly Property<int?> _percentComplete;
	}
}