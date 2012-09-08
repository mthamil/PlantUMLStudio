using System;

namespace PlantUmlEditor.ViewModel.Notifications
{
	/// <summary>
	/// Represents a progress update.
	/// </summary>
	public class ProgressUpdate
	{
		/// <summary>
		/// The percentage complete.
		/// </summary>
		public int? PercentComplete { get; set; }

		/// <summary>
		/// The message associated with the update.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Whether progress is finished.
		/// </summary>
		public bool IsFinished { get; set; }

		/// <summary>
		/// Creates a new progress update indicating failed completion due to an exception.
		/// </summary>
		/// <param name="exception">The exception that caused termination of progress</param>
		public static ProgressUpdate Failed(Exception exception)
		{
			return new ProgressUpdate { PercentComplete = null, Message = exception.Message, IsFinished = true };
		}

		/// <summary>
		/// Creates a new progress update indicating successful completion.
		/// </summary>
		/// <param name="message">The completion message.</param>
		public static ProgressUpdate Completed(string message)
		{
			return new ProgressUpdate { PercentComplete = null, Message = message, IsFinished = true };
		}
	}
}