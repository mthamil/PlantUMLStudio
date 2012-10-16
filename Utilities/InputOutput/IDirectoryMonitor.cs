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
	/// Interface for a file system monitor.
	/// </summary>
	public interface IDirectoryMonitor : IDisposable
	{
		/// <summary>
		/// Gets or sets the filter string used to determine what files are monitored in a directory.
		/// </summary>
		/// <returns>
		/// The filter string. The default is "*.*" (Watches all files.)
		/// </returns>
		string Filter { get; set; }

		/// <summary>
		/// The directory being monitored for changes.
		/// </summary>
		DirectoryInfo MonitoredDirectory { get; }

		/// <summary>
		/// Begins monitoring a directory for changes.
		/// </summary>
		/// <param name="directory">The directory to monitor</param>
		void StartMonitoring(DirectoryInfo directory);

		/// <summary>
		/// Begins monitoring the last monitored directory again.
		/// </summary>
		void RestartMonitoring();

		/// <summary>
		/// Stops monitoring file system changes.
		/// </summary>
		void StopMonitoring();

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is changed.
		/// </summary>
		event EventHandler<FileSystemEventArgs> Changed;

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is created.
		/// </summary>
		event EventHandler<FileSystemEventArgs> Created;

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is deleted.
		/// </summary>
		event EventHandler<FileSystemEventArgs> Deleted;

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is renamed.
		/// </summary>
		event EventHandler<RenamedEventArgs> Renamed;
	}
}