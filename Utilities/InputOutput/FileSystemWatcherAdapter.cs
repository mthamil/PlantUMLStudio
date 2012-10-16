//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
using System;
using System.IO;

namespace Utilities.InputOutput
{
	/// <summary>
	/// A wrapper around FileSystemWatcher.
	/// </summary>
	public class FileSystemWatcherAdapter : IFileSystemWatcher
	{
		/// <summary>
		/// Creates a file system watcher.
		/// </summary>
		public FileSystemWatcherAdapter()
			: this(new FileSystemWatcher()) { }

		/// <summary>
		/// Creates a file system watcher using an existing watcher.
		/// </summary>
		/// <param name="fileSystemWatcher">An existing file system watcher</param>
		public FileSystemWatcherAdapter(FileSystemWatcher fileSystemWatcher)
		{
			_fileSystemWatcher = fileSystemWatcher;
		}

		/// <summary>
		/// Gets or sets the path of the directory to watch.
		/// </summary>
		/// <returns>
		/// The path to monitor. The default is an empty string ("").
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// The specified path does not exist or could not be found.-or- 
		/// The specified path contains wildcard characters.-or- The specified 
		/// path contains invalid path characters.</exception>
		public string Path
		{
			get { return _fileSystemWatcher.Path; }
			set { _fileSystemWatcher.Path = value; }
		}

		/// <summary>
		/// Gets or sets the filter string used to determine what files are monitored in a directory.
		/// </summary>
		/// <returns>
		/// The filter string. The default is "*.*" (Watches all files.)
		/// </returns>
		public string Filter
		{
			get { return _fileSystemWatcher.Filter; }
			set { _fileSystemWatcher.Filter = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the component is enabled.
		/// </summary>
		public bool EnableRaisingEvents
		{
			get { return _fileSystemWatcher.EnableRaisingEvents; }
			set { _fileSystemWatcher.EnableRaisingEvents = value; }
		}

		/// <summary>
		/// Gets or sets the type of changes to watch for.
		/// </summary>
		public NotifyFilters NotifyFilter
		{
			get { return _fileSystemWatcher.NotifyFilter; }
			set { _fileSystemWatcher.NotifyFilter = value; }
		}

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is changed.
		/// </summary>
		public event FileSystemEventHandler Changed
		{
			add { _fileSystemWatcher.Changed += value; }
			remove { _fileSystemWatcher.Changed -= value; }
		}

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is created.
		/// </summary>
		public event FileSystemEventHandler Created
		{
			add { _fileSystemWatcher.Created += value; }
			remove { _fileSystemWatcher.Created -= value; }
		}

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is deleted.
		/// </summary>
		public event FileSystemEventHandler Deleted
		{
			add { _fileSystemWatcher.Deleted += value; }
			remove { _fileSystemWatcher.Deleted -= value; }
		}

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is renamed.
		/// </summary>
		public event RenamedEventHandler Renamed
		{
			add { _fileSystemWatcher.Renamed += value; }
			remove { _fileSystemWatcher.Renamed -= value; }
		}

		/// <summary>
		/// Occurs when the internal buffer overflows.
		/// </summary>
		public event ErrorEventHandler Error
		{
			add { _fileSystemWatcher.Error += value; }
			remove { _fileSystemWatcher.Error -= value; }
		}

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			_fileSystemWatcher.Dispose();
		}

		private readonly FileSystemWatcher _fileSystemWatcher;
	}
}