//  PlantUML Editor
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
using System.Windows.Input;
using PlantUmlEditor.Configuration;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// View model for a settings screen.
	/// </summary>
	public class SettingsViewModel : ViewModelBase
	{
		public SettingsViewModel(ISettings settings)
		{
			_settings = settings;

			_rememberOpenFiles = Property.New(this, p => p.RememberOpenFiles, OnPropertyChanged);
			_autoSaveEnabled = Property.New(this, p => p.AutoSaveEnabled, OnPropertyChanged);
			_autoSaveInterval = Property.New(this, p => p.AutoSaveInterval, OnPropertyChanged);
			_maximumRecentFiles = Property.New(this, p => p.MaximumRecentFiles, OnPropertyChanged);

			_saveCompleted = Property.New(this, p => p.SaveCompleted, OnPropertyChanged);

			SaveCommand = new BoundRelayCommand<SettingsViewModel>(_ => Save(), p => p.CanSave, this);

			RememberOpenFiles = _settings.RememberOpenFiles;
			AutoSaveEnabled = _settings.AutoSaveEnabled;
			AutoSaveInterval = _settings.AutoSaveInterval;
			MaximumRecentFiles = _settings.MaximumRecentFiles;
		}

		/// <summary>
		/// Whether to remember the files that were open when the application closes.
		/// </summary>
		public bool RememberOpenFiles
		{
			get { return _rememberOpenFiles.Value; }
			set { _rememberOpenFiles.Value = value; }
		}

		/// <summary>
		/// Whether to enable auto save.
		/// </summary>
		public bool AutoSaveEnabled
		{
			get { return _autoSaveEnabled.Value; }
			set { _autoSaveEnabled.Value = value; }
		}

		/// <summary>
		/// The auto save interval.
		/// </summary>
		public TimeSpan AutoSaveInterval
		{
			get { return _autoSaveInterval.Value; }
			set { _autoSaveInterval.Value = value; }
		}

		/// <summary>
		/// The maximum number of recent files to keep.
		/// </summary>
		public int MaximumRecentFiles
		{
			get { return _maximumRecentFiles.Value; }
			set { _maximumRecentFiles.Value = value; }
		}

		/// <summary>
		/// Commands that executes a Save operation.
		/// </summary>
		public ICommand SaveCommand { get; private set; }

		/// <summary>
		/// Whether a save can currently be performed.
		/// </summary>
		public bool CanSave
		{
			get { return !_isSaving; }
		}

		/// <summary>
		/// Saves settings changes.
		/// </summary>
		public void Save()
		{
			_isSaving = true;

			_settings.RememberOpenFiles = RememberOpenFiles;
			_settings.AutoSaveEnabled = AutoSaveEnabled;
			_settings.AutoSaveInterval = AutoSaveInterval;
			_settings.MaximumRecentFiles = MaximumRecentFiles;

			_settings.Save();

			_isSaving = false;
			SaveCompleted = true;
		}

		/// <summary>
		/// Whether a save operation has completed.
		/// </summary>
		public bool? SaveCompleted
		{
			get { return _saveCompleted.Value; }
			private set { _saveCompleted.Value = value; }
		}

		private bool _isSaving;

		private readonly Property<bool> _rememberOpenFiles;
		private readonly Property<bool> _autoSaveEnabled;
		private readonly Property<TimeSpan> _autoSaveInterval;
		private readonly Property<int> _maximumRecentFiles; 

		private readonly Property<bool?> _saveCompleted;

		private readonly ISettings _settings;
	}
}