using System;

namespace Utilities.Chronology
{
	/// <summary>
	/// Clock based on the current system date and time.
	/// </summary>
	public class SystemClock : IClock
	{
		#region Implementation of IClock

		/// <see cref="IClock.Now"/>
		public DateTimeOffset Now
		{
			get { return DateTimeOffset.Now; }
		}

		/// <see cref="IClock.UtcNow"/>
		public DateTimeOffset UtcNow
		{
			get { return DateTimeOffset.UtcNow; }
		}

		#endregion
	}
}