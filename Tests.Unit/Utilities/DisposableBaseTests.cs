using System;
using Utilities;
using Xunit;

namespace Tests.Unit.Utilities
{
	public class DisposableBaseTests
	{
		[Fact]
		public void Test_Dispose()
		{
			// Arrange.
			var test = new TestDisposable();

			// Act.
			test.Dispose();

			// Assert.
			Assert.True(test.IsDisposed);
			Assert.True(test.OnDisposingCalled);
			Assert.True(test.OnDisposeCalled);
		}

		[Fact]
		public void Test_ThrowIfDisposed()
		{
			// Arrange.
			var test = new TestDisposable();
			test.Dispose();

			// Act/Assert.
			Assert.Throws<ObjectDisposedException>(() => test.Do());
		}

		private class TestDisposable : DisposableBase
		{
			public void Do()
			{
				ThrowIfDisposed();
			}

			public bool OnDisposingCalled { get; private set; }
			public bool OnDisposeCalled { get; private set; }

			#region Overrides of DisposableBase

			protected override void OnDisposing()
			{
				OnDisposingCalled = true;
			}

			protected override void OnDispose()
			{
				OnDisposeCalled = true;
			}

			#endregion
		}
	}
}