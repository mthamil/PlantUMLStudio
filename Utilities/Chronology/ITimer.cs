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
		/// <exception cref="System.InvalidOperationException">
		/// Thrown if the timer has already been started.
		/// </exception>
		void Start();

		/// <summary>
		/// Attempts to start a timer. Returns false if the timer
		/// was already started.
		/// </summary>
		/// <returns>False if the timer was already started</returns>
		bool TryStart();

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
		/// Whether a timer is running.
		/// </summary>
		bool Started { get; }

		/// <summary>
		/// This event is raised when the timer has elapsed
		/// </summary>
		event EventHandler Elapsed;
	}
}
