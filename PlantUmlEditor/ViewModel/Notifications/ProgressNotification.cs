using System;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel.Notifications
{
	/// <summary>
	/// A class used to represent progress information for tasks.
	/// </summary>
	public class ProgressNotification : Notification, IProgressNotification
	{
		public ProgressNotification(Progress<ProgressUpdate> progress)
			: this()
		{
			progress.ProgressChanged += progress_ProgressChanged;
		}

		public ProgressNotification()
		{
			_hasDiscreteProgress = Property.New(this, p => p.HasDiscreteProgress, OnPropertyChanged);
			_percentComplete = Property.New(this, p => p.PercentComplete, OnPropertyChanged)
				.AlsoChanges(p => p.InProgress);
		}

		/// <see cref="IProgressNotification.HasDiscreteProgress"/>
		public bool HasDiscreteProgress
		{
			get { return _hasDiscreteProgress.Value; }
			set { _hasDiscreteProgress.Value = value; }
		}

		/// <see cref="IProgressNotification.InProgress"/>
		public bool InProgress
		{
			get { return PercentComplete.HasValue; }
		}

		/// <see cref="IProgressNotification.PercentComplete"/>
		public int? PercentComplete
		{
			get { return _percentComplete.Value; }
			set { _percentComplete.Value = value; }
		}

		void progress_ProgressChanged(object sender, ProgressUpdate e)
		{
			PercentComplete = e.PercentComplete;
			Message = e.Message;

			if (e.IsFinished)
				((Progress<ProgressUpdate>)sender).ProgressChanged -= progress_ProgressChanged;
		}

		private readonly Property<bool> _hasDiscreteProgress;
		private readonly Property<int?> _percentComplete;
	}
}