using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Utilities.InputOutput;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.Configuration
{
	/// <summary>
	/// Manages a collection of recent files.
	/// </summary>
	public class RecentFilesCollection : PropertyChangedNotifier, INotifyCollectionChanged, ICollection<FileInfo>
	{
		/// <summary>
		/// Initializes a new recent files collection.
		/// </summary>
		public RecentFilesCollection()
		{
			_maximumCount = Property.New(this, p => p.MaximumCount, OnPropertyChanged);
			_recentFiles.CollectionChanged += (o, e) => OnCollectionChanged(e);	// Relay event.
		}

		/// <summary>
		/// The maximum number of recent files to keep.
		/// </summary>
		public int MaximumCount
		{
			get { return _maximumCount.Value; }
			set 
			{
				if (_maximumCount.TrySetValue(value))
				{
					while (_recentFiles.Count > MaximumCount)
						_recentFiles.RemoveAt(_recentFiles.Count - 1);
				}
			}
		}

		/// <see cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<FileInfo> GetEnumerator()
		{
			return _recentFiles.GetEnumerator();
		}

		/// <see cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds a new recent file.
		/// </summary>
		/// <param name="item">The file to add</param>
		public void Add(FileInfo item)
		{
			_recentFiles.Insert(0, item);
			if (_recentFiles.Count > MaximumCount)
				_recentFiles.RemoveAt(_recentFiles.Count - 1);
		}

		/// <summary>
		/// Clears the recent files collection.
		/// </summary>
		public void Clear()
		{
			_recentFiles.Clear();
		}

		/// <summary>
		/// Returns whether a file in contained in the collection.
		/// </summary>
		/// <param name="item">The file to search for</param>
		/// <returns>True if the file is contained</returns>
		public bool Contains(FileInfo item)
		{
			return _recentFiles.Contains(item, FileInfoPathEqualityComparer.Instance);
		}

		/// <summary>
		/// Copies the recent files to an array.
		/// </summary>
		/// <param name="array">The array to copy to</param>
		/// <param name="arrayIndex">The index in the destination array to start copying</param>
		public void CopyTo(FileInfo[] array, int arrayIndex)
		{
			_recentFiles.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes a recent file.
		/// </summary>
		/// <param name="item">The file to remove</param>
		/// <returns>True if the file existed and was removed</returns>
		public bool Remove(FileInfo item)
		{
			return _recentFiles.Remove(item);
		}

		/// <summary>
		/// The number of recent files.
		/// </summary>
		public int Count
		{
			get { return _recentFiles.Count; }
		}

		/// <summary>
		/// Always returns false.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <see cref="INotifyCollectionChanged.CollectionChanged"/>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			var localEvent = CollectionChanged;
			if (localEvent != null)
				localEvent(this, args);
		}

		private readonly Property<int> _maximumCount;
		private readonly ObservableCollection<FileInfo> _recentFiles = new ObservableCollection<FileInfo>();
	}
}