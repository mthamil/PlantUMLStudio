using System.Threading;
using Utilities.Concurrency;
using Xunit;

namespace Unit.Tests
{
	/// <summary>
	/// Sets the current synchronization context to a context that is synchronous.
	/// </summary>
	public class SynchronousAttribute : BeforeAfterTestAttribute
	{
		/// <summary>
		/// Stores the current synchronization context and sets the current
		/// context to be synchronous.
		/// </summary>
		/// <param name="methodUnderTest"></param>
		public override void Before(System.Reflection.MethodInfo methodUnderTest)
		{
			_originalContext = SynchronizationContext.Current;
			SynchronizationContext.SetSynchronizationContext(new SynchronousSynchronizationContext());
		}

		/// <summary>
		/// Restores the original synchronization context.
		/// </summary>
		/// <param name="methodUnderTest"></param>
		public override void After(System.Reflection.MethodInfo methodUnderTest)
		{
			SynchronizationContext.SetSynchronizationContext(_originalContext);
		}

		private SynchronizationContext _originalContext;
	}
}