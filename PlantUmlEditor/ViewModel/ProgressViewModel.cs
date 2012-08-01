using System;
using Utilities.Concurrency;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// A class used to represent progress information for tasks.
	/// </summary>
	public class ProgressViewModel : ViewModelBase, IProgressViewModel, IProgressRegistration
	{
		public ProgressViewModel()
		{
			_hasDiscreteProgress = Property.New(this, p => p.HasDiscreteProgress, OnPropertyChanged);
			_percentComplete = Property.New(this, p => p.PercentComplete, OnPropertyChanged)
				.AlsoChanges(p => p.InProgress);
			_message = Property.New(this, p => p.Message, OnPropertyChanged);
		}

		/// <see cref="IProgressViewModel.HasDiscreteProgress"/>
		public bool HasDiscreteProgress
		{
			get { return _hasDiscreteProgress.Value; }
			set { _hasDiscreteProgress.Value = value; }
		}

		/// <see cref="IProgressViewModel.InProgress"/>
		public bool InProgress
		{
			get { return PercentComplete.HasValue; }
		}

		/// <see cref="IProgressViewModel.PercentComplete"/>
		public int? PercentComplete
		{
			get { return _percentComplete.Value; }
			set { _percentComplete.Value = value; }
		}

		/// <see cref="IProgressViewModel.Message"/>
		public string Message
		{
			get { return _message.Value; }
			set { _message.Value = value; }
		}

		/// <see cref="IProgressRegistration.New"/>
		public IProgress<ProgressUpdate> New(bool hasDiscreteProgress)
		{
			HasDiscreteProgress = hasDiscreteProgress;
			return new Progress<ProgressUpdate>(p =>
			{
				PercentComplete = p.PercentComplete;
				Message = p.Message;
			});
		}

		private readonly Property<bool> _hasDiscreteProgress;
		private readonly Property<int?> _percentComplete;
		private readonly Property<string> _message;
	}

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
		/// Creates a new progress update indicating completion due to an exception.
		/// </summary>
		/// <param name="exception">The exception that caused termination of progress</param>
		public static ProgressUpdate Failed(Exception exception)
		{
			return new ProgressUpdate { PercentComplete = null, Message = exception.Message };
		}

		/// <summary>
		/// Creates a new progress update indicating successful completion.
		/// </summary>
		/// <param name="message">The completion message.</param>
		public static ProgressUpdate Completed(string message)
		{
			return new ProgressUpdate { PercentComplete = null, Message = message };
		}
	}
}