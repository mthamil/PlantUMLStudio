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