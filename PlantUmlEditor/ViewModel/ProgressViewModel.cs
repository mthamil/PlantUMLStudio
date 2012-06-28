using System;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// A class used to represent progress information for tasks.
	/// </summary>
	public class ProgressViewModel : ViewModelBase, IProgressViewModel
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

		private readonly Property<bool> _hasDiscreteProgress;
		private readonly Property<int?> _percentComplete;
		private readonly Property<string> _message;
	}
}