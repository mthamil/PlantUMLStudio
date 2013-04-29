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

using System;
using System.Threading;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Methods allowing use of Read/Write/Upgradeable locks with a using block.
	/// </summary>
	public static class ReaderWriterLockSlimExtensions
	{
		/// <summary>
		/// Enters a read lock.
		/// </summary>
		/// <param name="readWriteLock">The reader/writer lock to lock</param>
		/// <returns>A token that, when disposed, releases the read lock</returns>
		public static IDisposable ReadLock(this ReaderWriterLockSlim readWriteLock)
		{
			return new ReadLock(readWriteLock);
		}

		/// <summary>
		/// Enters a write lock.
		/// </summary>
		/// <param name="readWriteLock">The reader/writer lock to lock</param>
		/// <returns>A token that, when disposed, releases the write lock</returns>
		public static IDisposable WriteLock(this ReaderWriterLockSlim readWriteLock)
		{
			return new WriteLock(readWriteLock);
		}

		/// <summary>
		/// Enters an upgradeable read lock.
		/// </summary>
		/// <param name="readWriteLock">The reader/writer lock to lock</param>
		/// <returns>A token that, when disposed, releases the upgradeable read lock</returns>
		public static IDisposable UpgradeableReadLock(this ReaderWriterLockSlim readWriteLock)
		{
			return new UpgradeableReadLock(readWriteLock);
		}
	}

	/// <summary>
	/// A class representing a disposable read lock.
	/// </summary>
	internal class ReadLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _readWriteLock;

		/// <summary>
		/// Acquires a new read lock.
		/// </summary>
		/// <param name="readWriteLock">The wrapped lock</param>
		public ReadLock(ReaderWriterLockSlim readWriteLock)
		{
			_readWriteLock = readWriteLock;
			_readWriteLock.EnterReadLock();
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Exits the read lock.
		/// </summary>
		public void Dispose()
		{
			_readWriteLock.ExitReadLock();
			GC.SuppressFinalize(this);
		}

		#endregion
	}

	/// <summary>
	/// A class representing a disposable write lock.
	/// </summary>
	internal class WriteLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _readWriteLock;

		/// <summary>
		/// Acquires a new write lock.
		/// </summary>
		/// <param name="readWriteLock">The wrapped lock</param>
		public WriteLock(ReaderWriterLockSlim readWriteLock)
		{
			_readWriteLock = readWriteLock;
			_readWriteLock.EnterWriteLock();
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Exits the write lock.
		/// </summary>
		public void Dispose()
		{
			_readWriteLock.ExitWriteLock();
			GC.SuppressFinalize(this);
		}

		#endregion
	}

	/// <summary>
	/// A class representing a disposable upgradeable read lock.
	/// </summary>
	internal class UpgradeableReadLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _readWriteLock;

		/// <summary>
		/// Acquires a new upgradeable read lock.
		/// </summary>
		/// <param name="readWriteLock">The wrapped lock</param>
		public UpgradeableReadLock(ReaderWriterLockSlim readWriteLock)
		{
			_readWriteLock = readWriteLock;
			_readWriteLock.EnterUpgradeableReadLock();
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Exits the upgradeable read lock.
		/// </summary>
		public void Dispose()
		{
			_readWriteLock.ExitUpgradeableReadLock();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}