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

namespace Utilities.Chronology
{
	/// <summary>
	/// This interface brings consistency to both the Form and Thread based versions of Windows timers
	/// </summary>
	public interface ITimer
	{
		/// <summary>
		/// Set the timer interval
		/// </summary>
		TimeSpan Interval { get; set; }

		/// <summary>
		/// Call to start the timer
		/// </summary>
		/// <param name="state">Optional user data</param>
		/// <exception cref="System.InvalidOperationException">
		/// Thrown if the timer has already been started.
		/// </exception>
		void Start(object state = null);

		/// <summary>
		/// Attempts to start a timer. Returns false if the timer
		/// was already started.
		/// </summary>
		/// <param name="state">Optional user data</param>
		/// <returns>False if the timer was already started</returns>
		bool TryStart(object state = null);

		/// <summary>
		/// Call to stop the timer
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// Thrown if the timer has not yet been started.
		/// </exception>	
		void Stop();

		/// <summary>
		/// Attempts to stop a timer. Returns false if the timer
		/// was already stopped.
		/// </summary>
		/// <returns>False if the timer was already stopped</returns>
		bool TryStop();

		/// <summary>
		/// Stops a timer if it was already started, and then starts it again.
		/// </summary>
		/// <param name="state">Optional user data</param>
		void Restart(object state = null);

		/// <summary>
		/// Whether a timer is running.
		/// </summary>
		bool Started { get; }

		/// <summary>
		/// This event is raised when the timer has elapsed
		/// </summary>
		event EventHandler<TimerElapsedEventArgs> Elapsed;
	}

	/// <summary>
	/// Event args for when a timer interval elapses.
	/// </summary>
	public class TimerElapsedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="signalTime">The time the elapsed event was raised</param>
		/// <param name="state">Arbitrary data provided by timer consumers</param>
		public TimerElapsedEventArgs(DateTime signalTime, object state)
		{
			SignalTime = signalTime;
			State = state;
		}

		/// <summary>
		/// The time the elapsed event was raised.
		/// </summary>
		public DateTime SignalTime { get; private set; }

		/// <summary>
		/// Arbitrary data provided by timer consumers.
		/// </summary>
		public object State { get; private set; }
	}
}
