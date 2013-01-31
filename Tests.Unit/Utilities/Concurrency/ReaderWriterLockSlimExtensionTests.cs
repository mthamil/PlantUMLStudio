using System;
using System.Threading;
using Utilities.Concurrency;
using Xunit;

namespace Tests.Unit.Utilities.Concurrency
{
	public class ReaderWriterLockSlimExtensionTests : IDisposable
	{
		[Fact]
		public void Test_ReadLock()
		{
			var readLock = readWriteLock.ReadLock();

			Assert.True(readWriteLock.IsReadLockHeld);
			Assert.False(readWriteLock.IsUpgradeableReadLockHeld);
			Assert.False(readWriteLock.IsWriteLockHeld);

			readLock.Dispose();

			Assert.False(readWriteLock.IsReadLockHeld);
			Assert.False(readWriteLock.IsUpgradeableReadLockHeld);
			Assert.False(readWriteLock.IsWriteLockHeld);
		}

		[Fact]
		public void Test_WriteLock()
		{
			var writeLock = readWriteLock.WriteLock();

			Assert.False(readWriteLock.IsReadLockHeld);
			Assert.False(readWriteLock.IsUpgradeableReadLockHeld);
			Assert.True(readWriteLock.IsWriteLockHeld);

			writeLock.Dispose();

			Assert.False(readWriteLock.IsReadLockHeld);
			Assert.False(readWriteLock.IsUpgradeableReadLockHeld);
			Assert.False(readWriteLock.IsWriteLockHeld);
		}

		[Fact]
		public void Test_UpgradeableReadLock()
		{
			var upgradeableReadLock = readWriteLock.UpgradeableReadLock();

			Assert.False(readWriteLock.IsReadLockHeld);
			Assert.True(readWriteLock.IsUpgradeableReadLockHeld);
			Assert.False(readWriteLock.IsWriteLockHeld);

			upgradeableReadLock.Dispose();

			Assert.False(readWriteLock.IsReadLockHeld);
			Assert.False(readWriteLock.IsUpgradeableReadLockHeld);
			Assert.False(readWriteLock.IsWriteLockHeld);
		}

		[Fact]
		public void Test_UpgradeableReadLock_Upgrade()
		{
			using (readWriteLock.UpgradeableReadLock())
			{
				var upgradedLock = readWriteLock.WriteLock();

				Assert.False(readWriteLock.IsReadLockHeld);
				Assert.True(readWriteLock.IsUpgradeableReadLockHeld);
				Assert.True(readWriteLock.IsWriteLockHeld);

				upgradedLock.Dispose();

				Assert.False(readWriteLock.IsReadLockHeld);
				Assert.True(readWriteLock.IsUpgradeableReadLockHeld);
				Assert.False(readWriteLock.IsWriteLockHeld);
			}
		}

		private readonly ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();

		#region Implementation of IDisposable

		public void Dispose()
		{
			readWriteLock.Dispose();
		}

		#endregion
	}
}
