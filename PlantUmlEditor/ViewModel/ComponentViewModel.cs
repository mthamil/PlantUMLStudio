using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlEditor.Core.Dependencies;
using PlantUmlEditor.ViewModel.Notifications;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents an external component.
	/// </summary>
	public class ComponentViewModel : ViewModelBase
	{
		public ComponentViewModel(IExternalComponent externalComponent)
		{
			_externalComponent = externalComponent;

			_name = Property.New(this, p => p.Name, OnPropertyChanged);
			Name = externalComponent.Name;

			_currentVersion = Property.New(this, p => p.CurrentVersion, OnPropertyChanged);
			_versionProgress = Property.New(this, p => p.VersionProgress, OnPropertyChanged);

			_hasUpdate = Property.New(this, p => p.HasUpdate, OnPropertyChanged);
			_latestVersion = Property.New(this, p => p.LatestVersion, OnPropertyChanged);
			_updateProgress = Property.New(this, p => p.UpdateProgress, OnPropertyChanged);

			UpdateCommand = new BoundRelayCommand<ComponentViewModel>(_ => UpdateAsync(), p => p.CanUpdate, this);
			_canUpdate = Property.New(this, p => p.CanUpdate, OnPropertyChanged);
			_updateCompleted = Property.New(this, p => UpdateCompleted, OnPropertyChanged);
		}

		/// <summary>
		/// Loads a component's information.
		/// </summary>
		public void Load()
		{
			CheckVersionAsync();
			CheckForUpdateAsync();
		}

		private async Task CheckVersionAsync()
		{
			VersionProgress = new ProgressNotification
			{
				HasDiscreteProgress = false,
				PercentComplete = 100
			};

			CurrentVersion = await _externalComponent.GetCurrentVersionAsync();
			VersionProgress.PercentComplete = null;
		}

		private async Task CheckForUpdateAsync()
		{
			UpdateProgress = new ProgressNotification
			{
				HasDiscreteProgress = false,
				PercentComplete = 100
			};

			var updateCheckResult = await _externalComponent.HasUpdateAsync(CancellationToken.None);
			UpdateProgress.PercentComplete = null;

			HasUpdate = updateCheckResult.HasValue;
			if (updateCheckResult.HasValue)
			{
				LatestVersion = updateCheckResult.Value;
				CanUpdate = true;
			}
		}

		/// <summary>
		/// The dependency name.
		/// </summary>
		public string Name
		{
			get { return _name.Value; }
			private set { _name.Value = value; }
		}

		/// <summary>
		/// The component's current version.
		/// </summary>
		public string CurrentVersion 
		{
			get { return _currentVersion.Value; }
			private set { _currentVersion.Value = value; }
		}

		/// <summary>
		/// Tracks progress when checking a dependency's version.
		/// </summary>
		public IProgressNotification VersionProgress
		{
			get { return _versionProgress.Value; }
			private set { _versionProgress.Value = value; }
		}

		/// <summary>
		/// Whether a dependency has an update available.
		/// </summary>
		public bool? HasUpdate
		{
			get { return _hasUpdate.Value; }
			private set { _hasUpdate.Value = value; }
		}

		/// <summary>
		/// The component's latest version.
		/// </summary>
		public string LatestVersion
		{
			get { return _latestVersion.Value; }
			private set { _latestVersion.Value = value; }
		}

		/// <summary>
		/// Tracks progress when checking for updates.
		/// </summary>
		public IProgressNotification UpdateProgress
		{
			get { return _updateProgress.Value; }
			private set { _updateProgress.Value = value; }
		}

		/// <summary>
		/// Whether an update can be performed.
		/// </summary>
		public bool CanUpdate
		{
			get { return _canUpdate.Value; }
			private set { _canUpdate.Value = value; }
		}

		/// <summary>
		/// Command to update a component.
		/// </summary>
		public ICommand UpdateCommand { get; private set; }

		private async Task UpdateAsync()
		{
			CanUpdate = false;
			UpdateProgress = new ProgressNotification
			{
				PercentComplete = 0,
				HasDiscreteProgress = false	
			};

			var progress = new Progress<ProgressChangedEventArgs>(p =>
			{
				if (!UpdateProgress.HasDiscreteProgress)
					UpdateProgress.HasDiscreteProgress = true;

				UpdateProgress.PercentComplete = p.ProgressPercentage;
			});

			await _externalComponent.DownloadLatestAsync(CancellationToken.None, progress);

			UpdateProgress.PercentComplete = null;
			UpdateCompleted = true;
		}

		/// <summary>
		/// Whether an update has finished.
		/// </summary>
		public bool UpdateCompleted
		{
			get { return _updateCompleted.Value; }
			private set { _updateCompleted.Value = value; }
		}

		private readonly Property<string> _name;
		private readonly Property<string> _currentVersion;
		private readonly Property<IProgressNotification> _versionProgress;
		private readonly Property<bool?> _hasUpdate;
		private readonly Property<IProgressNotification> _updateProgress;
		private readonly Property<string> _latestVersion;
		private readonly Property<bool> _canUpdate;
		private readonly Property<bool> _updateCompleted;

		private readonly IExternalComponent _externalComponent;
	}
}