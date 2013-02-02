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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PlantUmlEditor.Properties;
using Utilities.InputOutput;
using Utilities.PropertyChanged;
using Utilities.Reflection;

namespace PlantUmlEditor.Configuration
{
	/// <summary>
	/// An adapter around the generated .NET Settings class.
	/// </summary>
	public class DotNetSettings : PropertyChangedNotifier<ISettings>, ISettings
	{
		internal DotNetSettings(Settings settings, DirectoryInfo defaultDiagramLocation)
			: this()
		{
			_settings = settings;

			LastDiagramLocation = String.IsNullOrEmpty(_settings.LastPath)
				? defaultDiagramLocation
				: new DirectoryInfo(_settings.LastPath);

			RememberOpenFiles = settings.RememberOpenFiles;
			OpenFiles = settings.OpenFiles == null ? 
				Enumerable.Empty<FileInfo>() :
				settings.OpenFiles.Cast<string>().Select(fileName => new FileInfo(fileName)).ToList();

			_recentFiles.MaximumCount = settings.MaximumRecentFiles;
			if (settings.RecentFiles != null)
			{
				var recentFiles = settings.RecentFiles.Cast<string>().Reverse().Select(fileName => new FileInfo(fileName));
				foreach (var recentFile in recentFiles)
					_recentFiles.Add(recentFile);
			}

			AutoSaveEnabled = settings.AutoSaveEnabled;
			AutoSaveInterval = settings.AutoSaveInterval;

			GraphVizExecutable = new FileInfo(_settings.GraphVizLocation);
			PlantUmlJar = new FileInfo(_settings.PlantUmlLocation);

			GraphVizLocalVersionPattern = new Regex(settings.GraphVizLocalVersionPattern);
			PlantUmlDownloadLocation = settings.DownloadUrl;
			PlantUmlVersionSource = settings.PlantUmlVersionSource;
			PlantUmlRemoteVersionPattern = new Regex(settings.PlantUmlRemoteVersionPattern);
			PlantUmlLocalVersionPattern = new Regex(settings.PlantUmlLocalVersionPattern);
			DiagramFileExtension = settings.PlantUmlFileExtension;
			PlantUmlHighlightingDefinition = new FileInfo(settings.PlantUmlHighlightingDefinition);
		}

		private DotNetSettings()
		{
			_lastDiagramLocation = Property.New(this, p => p.LastDiagramLocation, OnPropertyChanged)
										   .EqualWhen((oldValue, newValue) => CheckEquality(oldValue, newValue, 
											   (x, y) => x.FullName.Equals(y.FullName, StringComparison.OrdinalIgnoreCase)));

			_rememberOpenFiles = Property.New(this, p => p.RememberOpenFiles, OnPropertyChanged);
			_openFiles = Property.New(this, p => p.OpenFiles, OnPropertyChanged)
								 .EqualWhen((oldValue, newValue) => CheckEquality(oldValue, newValue, 
									 (x, y) => x.SequenceEqual(y, FileInfoPathEqualityComparer.Instance)));

			_recentFiles = new RecentFilesCollection();
			_recentFiles.PropertyChanged += recentFiles_PropertyChanged;

			_autoSaveEnabled = Property.New(this, p => p.AutoSaveEnabled, OnPropertyChanged);
			_autoSaveInterval = Property.New(this, p => p.AutoSaveInterval, OnPropertyChanged);
		}

		/// <see cref="ISettings.LastDiagramLocation"/>
		public DirectoryInfo LastDiagramLocation
		{
			get { return _lastDiagramLocation.Value; }
			set { _lastDiagramLocation.Value = value; }
		}

		/// <see cref="ISettings.RememberOpenFiles"/>
		public bool RememberOpenFiles
		{
			get { return _rememberOpenFiles.Value; }
			set { _rememberOpenFiles.Value = value; }
		}

		/// <see cref="ISettings.OpenFiles"/>
		public IEnumerable<FileInfo> OpenFiles
		{ 
			get { return _openFiles.Value; }
			set { _openFiles.Value = value; }
		}

		/// <see cref="ISettings.RecentFiles"/>
		public ICollection<FileInfo> RecentFiles 
		{
			get { return _recentFiles; }
		}

		/// <see cref="ISettings.MaximumRecentFiles"/>
		public int MaximumRecentFiles
		{
			get { return _recentFiles.MaximumCount; }
			set { _recentFiles.MaximumCount = value; }
		}

		/// <see cref="ISettings.AutoSaveEnabled"/>
		public bool AutoSaveEnabled
		{ 
			get { return _autoSaveEnabled.Value; }
			set { _autoSaveEnabled.Value = value; }
		}

		/// <see cref="ISettings.AutoSaveInterval"/>
		public TimeSpan AutoSaveInterval
		{ 
			get { return _autoSaveInterval.Value; }
			set { _autoSaveInterval.Value = value; }
		}

		/// <see cref="ISettings.GraphVizExecutable"/>
		public FileInfo GraphVizExecutable { get; set; }

		/// <see cref="ISettings.GraphVizLocalVersionPattern"/>
		public Regex GraphVizLocalVersionPattern { get; private set; }

		/// <see cref="ISettings.PlantUmlJar"/>
		public FileInfo PlantUmlJar { get; set; }

		/// <see cref="ISettings.PlantUmlDownloadLocation"/>
		public Uri PlantUmlDownloadLocation { get; private set; }

		/// <see cref="ISettings.PlantUmlVersionSource"/>
		public Uri PlantUmlVersionSource { get; private set; }

		/// <see cref="ISettings.PlantUmlRemoteVersionPattern"/>
		public Regex PlantUmlRemoteVersionPattern { get; private set; }

		/// <see cref="ISettings.PlantUmlLocalVersionPattern"/>
		public Regex PlantUmlLocalVersionPattern { get; private set; }

		/// <see cref="ISettings.DiagramFileExtension"/>
		public string DiagramFileExtension { get; private set; }

		/// <see cref="ISettings.PlantUmlHighlightingDefinition"/>
		public FileInfo PlantUmlHighlightingDefinition { get; private set; }

		/// <see cref="ISettings.Save"/>
		public void Save()
		{
			_settings.LastPath = LastDiagramLocation.FullName;
			_settings.GraphVizLocation = GraphVizExecutable.FullName;
			_settings.PlantUmlLocation = PlantUmlJar.FullName;

			_settings.RememberOpenFiles = RememberOpenFiles;
			var openFiles = new StringCollection();
			openFiles.AddRange(OpenFiles.Select(file => file.FullName).ToArray());
			_settings.OpenFiles = openFiles;

			_settings.MaximumRecentFiles = MaximumRecentFiles;
			var recentFiles = new StringCollection();
			recentFiles.AddRange(RecentFiles.Select(file => file.FullName).ToArray());
			_settings.RecentFiles = recentFiles;

			_settings.AutoSaveEnabled = AutoSaveEnabled;
			_settings.AutoSaveInterval = AutoSaveInterval;

			_settings.Save();
		}

		/// <summary>
		/// Performs an equality check between two referene objects. This method handles reference
		/// equality and null checks and then defers to the given custom comparison.
		/// </summary>
		private static bool CheckEquality<T>(T x, T y, Func<T, T, bool> comparison)
			where T : class
		{
			if (ReferenceEquals(x, y))
				return true;

			if (x == null || y == null)
				return false;

			return comparison(x, y);
		}

		void recentFiles_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// Relay the property change.
			if (e.PropertyName == maxRecentFilesCountName)
				OnPropertyChanged(p => p.MaximumRecentFiles);
		}

		private readonly Property<DirectoryInfo> _lastDiagramLocation;
		private readonly Property<bool> _rememberOpenFiles;
		private readonly Property<IEnumerable<FileInfo>> _openFiles;
		private readonly RecentFilesCollection _recentFiles;
		private readonly Property<bool> _autoSaveEnabled;
		private readonly Property<TimeSpan> _autoSaveInterval;

		private readonly Settings _settings;

		private static readonly string maxRecentFilesCountName = Reflect.PropertyOf<RecentFilesCollection>(p => p.MaximumCount).Name;
	}
}