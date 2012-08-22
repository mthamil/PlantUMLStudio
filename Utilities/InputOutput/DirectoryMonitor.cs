using System;
using System.IO;

namespace Utilities.InputOutput
{
	/// <summary>
	/// Monitors a directory for changes.  Provides
	/// a higher-level of abstraction over a FileSystemWatcher.
	/// </summary>
	public class DirectoryMonitor : IDirectoryMonitor
	{
		public DirectoryMonitor(IFileSystemWatcher fileSystemWatcher)
		{
			_fileSystemWatcher = fileSystemWatcher;

			_fileSystemWatcher.Created += fileSystemWatcher_Created;
			_fileSystemWatcher.Deleted += fileSystemWatcher_Deleted;
			_fileSystemWatcher.Changed += fileSystemWatcher_Changed;
			_fileSystemWatcher.Renamed += fileSystemWatcher_Renamed;
		}

		/// <see cref="IDirectoryMonitor.Filter"/>
		public string Filter 
		{
			get { return _fileSystemWatcher.Filter; }
			set { _fileSystemWatcher.Filter = value; }
		}

		/// <see cref="IDirectoryMonitor.MonitoredDirectory"/>
		public DirectoryInfo MonitoredDirectory { get; private set; }

		/// <see cref="IDirectoryMonitor.StartMonitoring"/>
		public void StartMonitoring(DirectoryInfo directory)
		{
			_fileSystemWatcher.Path = directory.FullName;
			_fileSystemWatcher.EnableRaisingEvents = true;
			MonitoredDirectory = directory;
		}

		/// <see cref="IDirectoryMonitor.RestartMonitoring"/>
		public void RestartMonitoring()
		{
			if (MonitoredDirectory == null)
				throw new InvalidOperationException("No directory was previously monitored.");

			StartMonitoring(MonitoredDirectory);
		}

		/// <see cref="IDirectoryMonitor.StopMonitoring"/>
		public void StopMonitoring()
		{
			_fileSystemWatcher.EnableRaisingEvents = false;
		}

		/// <see cref="IDirectoryMonitor.Changed"/>
		public event EventHandler<FileSystemEventArgs> Changed;

		void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			OnChanged(e);
		}

		private void OnChanged(FileSystemEventArgs args)
		{
			var localEvent = Changed;
			if (localEvent != null)
				localEvent(this, args);
		}

		/// <see cref="IDirectoryMonitor.Created"/>
		public event EventHandler<FileSystemEventArgs> Created;

		void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
		{
			OnCreated(e);
		}

		private void OnCreated(FileSystemEventArgs args)
		{
			var localEvent = Created;
			if (localEvent != null)
				localEvent(this, args);
		}

		/// <see cref="IDirectoryMonitor.Deleted"/>
		public event EventHandler<FileSystemEventArgs> Deleted;

		void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			OnDeleted(e);
		}

		private void OnDeleted(FileSystemEventArgs args)
		{
			var localEvent = Deleted;
			if (localEvent != null)
				localEvent(this, args);
		}

		/// <see cref="IDirectoryMonitor.Renamed"/>
		public event EventHandler<RenamedEventArgs> Renamed;

		void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			OnRenamed(e);
		}

		private void OnRenamed(RenamedEventArgs args)
		{
			var localEvent = Renamed;
			if (localEvent != null)
				localEvent(this, args);
		}

		#region IDisposable Members

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Implements the actual disposal logic.  Subclasses should
		/// override this method to clean up resources.
		/// </summary>
		/// <param name="disposing">Whether the class is disposing from the Dispose() method</param>
		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_fileSystemWatcher.Created -= fileSystemWatcher_Created;
					_fileSystemWatcher.Deleted -= fileSystemWatcher_Deleted;
					_fileSystemWatcher.Changed -= fileSystemWatcher_Changed;
					_fileSystemWatcher.Renamed -= fileSystemWatcher_Renamed;
				}

				_fileSystemWatcher.Dispose();

				_disposed = true;
			}
		}

		/// <summary>
		/// Use C# destructor syntax for finalization code.
		/// This destructor will run only if the Dispose method
		/// does not get called.
		/// It gives your base class the opportunity to finalize.
		/// Do not provide destructors in types derived from this class.
		/// </summary>
		~DirectoryMonitor()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			Dispose(false);
		}

		private bool _disposed;

		#endregion

		private readonly IFileSystemWatcher _fileSystemWatcher;
	}
}