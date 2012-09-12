using System.Threading;
using System.Threading.Tasks;
using PlantUmlEditor.Core.Dependencies;
using PlantUmlEditor.ViewModel.Notifications;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
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
			_updateCheckProgress = Property.New(this, p => p.UpdateCheckProgress, OnPropertyChanged);
			_latestVersion = Property.New(this, p => p.LatestVersion, OnPropertyChanged);
		}

		/// <summary>
		/// Analyzes a dependency.
		/// </summary>
		public async Task AnalyzeAsync()
		{
			await Task.WhenAll(CheckVersionAsync(), CheckForUpdateAsync());
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
			UpdateCheckProgress = new ProgressNotification
			{
				HasDiscreteProgress = false,
				PercentComplete = 100
			};

			var updateCheckResult = await _externalComponent.HasUpdateAsync(CancellationToken.None);
			HasUpdate = updateCheckResult.HasValue;
			if (updateCheckResult.HasValue)
				LatestVersion = updateCheckResult.Value;
			UpdateCheckProgress.PercentComplete = null;
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
		/// Tracks progress when checking for updates.
		/// </summary>
		public IProgressNotification UpdateCheckProgress
		{
			get { return _updateCheckProgress.Value; }
			private set { _updateCheckProgress.Value = value; }
		}

		/// <summary>
		/// The component's latest version.
		/// </summary>
		public string LatestVersion
		{
			get { return _latestVersion.Value; }
			private set { _latestVersion.Value = value; }
		}

		private readonly Property<string> _name;
		private readonly Property<string> _currentVersion;
		private readonly Property<IProgressNotification> _versionProgress;
		private readonly Property<bool?> _hasUpdate;
		private readonly Property<IProgressNotification> _updateCheckProgress;
		private readonly Property<string> _latestVersion;

		private readonly IExternalComponent _externalComponent;
	}
}