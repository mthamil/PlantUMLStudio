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
using System.Collections.Specialized;
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
			_maximumRecentFiles = Property.New(this, p => p.MaximumRecentFiles, OnPropertyChanged);

			_autoSaveEnabled = Property.New(this, p => p.AutoSaveEnabled, OnPropertyChanged);
			_autoSaveInterval = Property.New(this, p => p.AutoSaveInterval, OnPropertyChanged);
			
			_highlightCurrentLine = Property.New(this, p => p.HighlightCurrentLine, OnPropertyChanged);
			_showLineNumbers = Property.New(this, p => p.ShowLineNumbers, OnPropertyChanged);
			_enableVirtualSpace = Property.New(this, p => p.EnableVirtualSpace, OnPropertyChanged);
			_enableWordWrap = Property.New(this, p => p.EnableWordWrap, OnPropertyChanged);
			_emptySelectionCopiesEntireLine = Property.New(this, p => p.EmptySelectionCopiesEntireLine, OnPropertyChanged);
			_allowScrollingBelowContent = Property.New(this, p => p.AllowScrollingBelowContent, OnPropertyChanged);

			_canClearRecentFiles = Property.New(this, p => p.CanClearRecentFiles, OnPropertyChanged);
			_saveCompleted = Property.New(this, p => p.SaveCompleted, OnPropertyChanged);

			ClearRecentFilesCommand = Command.For(this).DependsOn(p => p.CanClearRecentFiles).Executes(ClearRecentFiles);
			SaveCommand = Command.For(this).DependsOn(p => p.CanSave).Executes(Save);

			RememberOpenFiles = _settings.RememberOpenFiles;
			MaximumRecentFiles = _settings.MaximumRecentFiles;
			CanClearRecentFiles = _settings.RecentFiles.Count > 0;

			AutoSaveEnabled = _settings.AutoSaveEnabled;
			AutoSaveInterval = _settings.AutoSaveInterval;
			
			HighlightCurrentLine = _settings.HighlightCurrentLine;
			ShowLineNumbers = _settings.ShowLineNumbers;
			EnableVirtualSpace = _settings.EnableVirtualSpace;
			EnableWordWrap = _settings.EnableWordWrap;
			EmptySelectionCopiesEntireLine = _settings.EmptySelectionCopiesEntireLine;
			AllowScrollingBelowContent = _settings.AllowScrollingBelowContent;

			var recentFilesChanged = _settings.RecentFiles as INotifyCollectionChanged;
			if (recentFilesChanged != null)
				recentFilesChanged.CollectionChanged += recentFilesChanged_CollectionChanged;
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
		/// Command that clears the recent files list.
		/// </summary>
		public ICommand ClearRecentFilesCommand { get; private set; }

		/// <summary>
		/// Whether recent files can be cleared.
		/// </summary>
		public bool CanClearRecentFiles
		{
			get { return _canClearRecentFiles.Value; }
			private set { _canClearRecentFiles.Value = value; }
		}

		void recentFilesChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			CanClearRecentFiles = _settings.RecentFiles.Count > 0;
		}

		/// <summary>
		/// Clears the recent files list.
		/// </summary>
		public void ClearRecentFiles()
		{
			_shouldClearRecentFiles = true;
			CanClearRecentFiles = false;
		}

		/// <summary>
		/// Whether to highlight the line the cursor currently resides in.
		/// </summary>
		public bool HighlightCurrentLine
		{
			get { return _highlightCurrentLine.Value; }
			set { _highlightCurrentLine.Value = value; }
		}

		/// <summary>
		/// Whether to show line numbers in the editor.
		/// </summary>
		public bool ShowLineNumbers
		{
			get { return _showLineNumbers.Value; }
			set { _showLineNumbers.Value = value; }
		}

		/// <summary>
		/// Whether virtual space is enabled. That is, can editing occur beyond the end of a line.
		/// </summary>
		public bool EnableVirtualSpace
		{
			get { return _enableVirtualSpace.Value; }
			set { _enableVirtualSpace.Value = value; }
		}

		/// <summary>
		/// Whether word wrap is enabled.
		/// </summary>
		public bool EnableWordWrap
		{
			get { return _enableWordWrap.Value; }
			set { _enableWordWrap.Value = value; }
		}

		/// <summary>
		/// Whether a cut or copy operation with no text selected copies the entire current line.
		/// </summary>
		public bool EmptySelectionCopiesEntireLine
		{
			get { return _emptySelectionCopiesEntireLine.Value; }
			set { _emptySelectionCopiesEntireLine.Value = value; }
		}

		/// <summary>
		/// Whether to allow vertical srolling past the end of an editor's content.
		/// </summary>
		public bool AllowScrollingBelowContent
		{
			get { return _allowScrollingBelowContent.Value; }
			set { _allowScrollingBelowContent.Value = value; }
		}

		/// <summary>
		/// Command that executes a Save operation.
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
			_settings.MaximumRecentFiles = MaximumRecentFiles;

			if (_shouldClearRecentFiles)
				_settings.RecentFiles.Clear();

			_settings.AutoSaveEnabled = AutoSaveEnabled;
			_settings.AutoSaveInterval = AutoSaveInterval;

			_settings.HighlightCurrentLine = HighlightCurrentLine;
			_settings.ShowLineNumbers = ShowLineNumbers;
			_settings.EnableVirtualSpace = EnableVirtualSpace;
			_settings.EnableWordWrap = EnableWordWrap;
			_settings.EmptySelectionCopiesEntireLine = EmptySelectionCopiesEntireLine;
			_settings.AllowScrollingBelowContent = AllowScrollingBelowContent;

			_settings.Save();

			var recentFilesChanged = _settings.RecentFiles as INotifyCollectionChanged;
			if (recentFilesChanged != null)
				recentFilesChanged.CollectionChanged -= recentFilesChanged_CollectionChanged;

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
		private bool _shouldClearRecentFiles;

		private readonly Property<bool> _rememberOpenFiles;
		private readonly Property<int> _maximumRecentFiles;

		private readonly Property<bool> _autoSaveEnabled;
		private readonly Property<TimeSpan> _autoSaveInterval;

		private readonly Property<bool> _highlightCurrentLine;
		private readonly Property<bool> _showLineNumbers;
		private readonly Property<bool> _enableVirtualSpace;
		private readonly Property<bool> _enableWordWrap;
		private readonly Property<bool> _emptySelectionCopiesEntireLine;
		private readonly Property<bool> _allowScrollingBelowContent;

		private readonly Property<bool> _canClearRecentFiles; 
		private readonly Property<bool?> _saveCompleted;

		private readonly ISettings _settings;
	}
}