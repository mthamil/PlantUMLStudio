//  PlantUML Editor
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
using System.Windows.Threading;

namespace Utilities.Chronology
{
	/// <summary>
	/// Class that wraps a timer based on <see cref="DispatcherTimer"/>.
	/// </summary>
	public class DispatcherTimerAdapter : TimerBase
	{
		/// <see cref="ITimer.TryStart"/>
		public override bool TryStart(object state = null)
		{
			lock (SyncObject)
			{
				if (_timer.IsEnabled)
					return false;

				_timer.Interval = Interval;
				_state = state;
				_timer.Tick += timer_Tick;
				try
				{
					_timer.Start();
				}
				catch (Exception)
				{
					_timer.Tick -= timer_Tick;
					throw;
				}

				return true;
			}
		}

		/// <see cref="ITimer.TryStop"/>
		public override bool TryStop()
		{
			lock (SyncObject)
			{
				if (!_timer.IsEnabled)
					return false;

				_timer.Tick -= timer_Tick;
				_timer.Stop();

				return true;
			}
		}

		/// <see cref="ITimer.Started"/>
		public override bool Started
		{
			get
			{
				lock (SyncObject)
				{
					return _timer.IsEnabled;
				}
			}
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			OnElapsed(DateTime.Now, _state);
		}

		private object _state;

		/// <summary>
		/// The underlying timer for which this object acts as a wrapper.
		/// </summary>
		private readonly DispatcherTimer _timer = new DispatcherTimer();
	}
}