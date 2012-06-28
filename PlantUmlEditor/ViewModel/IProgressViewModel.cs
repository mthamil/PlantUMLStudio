namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Interface for a view model representing progress information for some task.
	/// </summary>
	public interface IProgressViewModel
	{
		/// <summary>
		/// Whether there are currently actual discrete chunks of progress.
		/// </summary>
		bool HasDiscreteProgress { get; set; }

		/// <summary>
		/// Whether there is something currently reporting progress.
		/// </summary>
		bool InProgress { get; }

		/// <summary>
		/// The percentage of work completed.
		/// If null, the percentage is not applicable.
		/// </summary>
		int? PercentComplete { get; set; }

		/// <summary>
		/// The current progress message.
		/// </summary>
		string Message { get; set; }
	}
}