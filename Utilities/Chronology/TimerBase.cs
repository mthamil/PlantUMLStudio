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

namespace Utilities.Chronology
{
	/// <summary>
	/// Provides a convenient base class for timers.
	/// </summary>
	public abstract class TimerBase : ITimer
	{
		/// <see cref="ITimer.Interval"/>
		public virtual TimeSpan Interval { get; set; }

		/// <see cref="ITimer.Start"/>
		public void Start(object state = null)
		{
			lock (SyncObject)
			{
				if (!TryStart(state))
					throw new InvalidOperationException("Timer is already started.");
			}
		}

		/// <see cref="ITimer.TryStart"/>
		public abstract bool TryStart(object state = null);

		/// <see cref="ITimer.Stop"/>
		public void Stop()
		{
			lock (SyncObject)
			{
				if (!TryStop())
					throw new InvalidOperationException("Timer is already stopped.");
			}
		}

		/// <see cref="ITimer.TryStop"/>
		public abstract bool TryStop();

		/// <see cref="ITimer.Restart"/>
		public void Restart(object state = null)
		{
			lock (SyncObject)
			{
				TryStop();
				Start(state);
			}
		}

		/// <see cref="ITimer.Started"/>
		public abstract bool Started { get; }

		/// <see cref="ITimer.Elapsed"/>
		public event EventHandler<TimerElapsedEventArgs> Elapsed;

		/// <summary>
		/// Raises the <see cref="ITimer.Elapsed"/> event.
		/// </summary>
		/// <param name="elapsedTime">The time at which the timer period elapsed</param>
		/// <param name="state">Associated user state</param>
		protected void OnElapsed(DateTime elapsedTime, object state)
		{
			var localEvent = Elapsed;
			if (localEvent != null)
				localEvent(this, new TimerElapsedEventArgs(elapsedTime, state));
		}

		/// <summary>
		/// Object used for locking.
		/// </summary>
		protected object SyncObject
		{
			get { return _syncObject; }
		}

		private readonly object _syncObject = new object();
	}
}