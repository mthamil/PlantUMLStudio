using System;

namespace Utilities.Chronology
{
	/// <summary>
	/// Provides time services.
	/// </summary>
	public interface IClock
	{
		/// <summary>
		/// Gets the current date/time.
		/// </summary>
		DateTimeOffset Now { get; }

		/// <summary>
		/// Gets the current UTC date/time.
		/// </summary>
		DateTimeOffset UtcNow { get; }
	}
}