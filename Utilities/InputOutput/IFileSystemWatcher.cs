using System;
using System.IO;

namespace Utilities.InputOutput
{
	/// <summary>
	/// Interface for a an object that monitors file system changes.
	/// </summary>
	public interface IFileSystemWatcher : IDisposable
	{
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
		string Path { get; set; }

		/// <summary>
		/// Gets or sets the filter string used to determine what files are monitored in a directory.
		/// </summary>
		/// <returns>
		/// The filter string. The default is "*.*" (Watches all files.)
		/// </returns>
		string Filter { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the component is enabled.
		/// </summary>
		bool EnableRaisingEvents { get; set; }

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is changed.
		/// </summary>
		event FileSystemEventHandler Changed;

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is created.
		/// </summary>
		event FileSystemEventHandler Created;

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is deleted.
		/// </summary>
		event FileSystemEventHandler Deleted;

		/// <summary>
		/// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path"/> is renamed.
		/// </summary>
		event RenamedEventHandler Renamed;

		/// <summary>
		/// Occurs when the internal buffer overflows.
		/// </summary>
		event ErrorEventHandler Error;
	}
}