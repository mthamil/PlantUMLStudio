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
		void Start();

		/// <summary>
		/// Call to stop the timer
		/// </summary>
		void Stop();

		/// <summary>
		/// This event is raised when the timer has elapsed
		/// </summary>
		event EventHandler Elapsed;
	}
}
