//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using PlantUmlStudio.Core.Dependencies;
using PlantUmlStudio.Core.Security;
using PlantUmlStudio.ViewModel.Notifications;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Observable;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Represents an external component.
	/// </summary>
	public class ComponentViewModel : ViewModelBase
	{
		public ComponentViewModel(IExternalComponent externalComponent, ISecurityService securityService)
		{
			_externalComponent = externalComponent;
			_securityService = securityService;

			_name = Property.New(this, p => p.Name);
			Name = externalComponent.Name;

			_currentVersion = Property.New(this, p => p.CurrentVersion);
			_versionProgress = Property.New(this, p => p.VersionProgress);

			_hasAvailableUpdate = Property.New(this, p => p.HasAvailableUpdate)
			                              .AlsoChanges(p => p.CanUpdate);
			_latestVersion = Property.New(this, p => p.LatestVersion);
			_updateProgress = Property.New(this, p => p.UpdateProgress);

		    UpdateCommand = Command.For(this)
		                           .DependsOn(p => p.CanUpdate)
		                           .ExecutesAsync(UpdateAsync);

			_updateCompleted = Property.New(this, p => UpdateCompleted);
		}

		/// <summary>
		/// Loads a component's information asynchronously.
		/// </summary>
		public Task LoadAsync()
		{
			return Task.WhenAll(CheckVersionAsync(), CheckForUpdateAsync());
		}

		private async Task CheckVersionAsync()
		{
			VersionProgress = new ProgressNotification
			{
				HasDiscreteProgress = false,
				PercentComplete = 100
			};

		    try
		    {
                CurrentVersion = await _externalComponent.GetCurrentVersionAsync();
		    }
		    catch (FileNotFoundException)
		    {
		        CurrentVersion = "Not Available";
		    }
			
			VersionProgress.PercentComplete = null;
		}

		private async Task CheckForUpdateAsync()
		{
			UpdateProgress = new ProgressNotification
			{
				HasDiscreteProgress = false,
				PercentComplete = 100
			};

			var updateCheckResult = await _externalComponent.HasUpdateAsync();
			UpdateProgress.PercentComplete = null;

			HasAvailableUpdate = updateCheckResult.HasValue;
			updateCheckResult.Apply(latest =>
			{
				LatestVersion = latest;
			});
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
		/// Whether a dependency has an available update.
		/// </summary>
		public bool? HasAvailableUpdate
		{
			get { return _hasAvailableUpdate.Value; }
			private set { _hasAvailableUpdate.Value = value; }
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
		/// Whether current permissions allow an update.
		/// </summary>
		public bool HasUpdatePermission => _securityService.HasAdminPriviledges();

	    /// <summary>
		/// Whether an update can be performed.
		/// </summary>
		public bool CanUpdate => HasAvailableUpdate.HasValue && HasAvailableUpdate.Value && HasUpdatePermission;

	    /// <summary>
		/// Command to update a component.
		/// </summary>
		public IAsyncCommand UpdateCommand { get; }

		private async Task UpdateAsync()
		{
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

			await _externalComponent.DownloadLatestAsync(progress);

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
		private readonly Property<bool?> _hasAvailableUpdate;
		private readonly Property<IProgressNotification> _updateProgress;
		private readonly Property<string> _latestVersion;
		private readonly Property<bool> _updateCompleted;

		private readonly IExternalComponent _externalComponent;
		private readonly ISecurityService _securityService;
	}
}