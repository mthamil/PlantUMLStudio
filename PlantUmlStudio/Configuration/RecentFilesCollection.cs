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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using SharpEssentials.InputOutput;
using SharpEssentials.Observable;

namespace PlantUmlStudio.Configuration
{
	/// <summary>
	/// Manages a collection of recent files.
	/// </summary>
	public class RecentFilesCollection : ObservableObject, INotifyCollectionChanged, ICollection<FileInfo>
	{
		/// <summary>
		/// Initializes a new recent files collection.
		/// </summary>
		public RecentFilesCollection()
		{
			_maximumCount = Property.New(this, p => p.MaximumCount);
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
			var existingFile = _recentFiles.SingleOrDefault(f => FileSystemInfoPathEqualityComparer.Instance.Equals(f, item));
			if (existingFile != null)
				_recentFiles.Remove(existingFile);

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
			return _recentFiles.Contains(item, FileSystemInfoPathEqualityComparer.Instance);
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
		public int Count => _recentFiles.Count;

	    /// <summary>
		/// Always returns false.
		/// </summary>
		public bool IsReadOnly => false;

	    /// <see cref="INotifyCollectionChanged.CollectionChanged"/>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
            CollectionChanged?.Invoke(this, args);
		}

		private readonly Property<int> _maximumCount;
		private readonly ObservableCollection<FileInfo> _recentFiles = new ObservableCollection<FileInfo>();
	}
}