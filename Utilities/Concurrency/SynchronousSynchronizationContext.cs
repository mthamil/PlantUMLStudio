using System.Threading;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Synchronization context that executes all actions synchronously instead
	/// of asynchronously.
	/// </summary>
	public class SynchronousSynchronizationContext : SynchronizationContext
	{
		/// <see cref="SynchronizationContext.Post"/>
		public override void Post(SendOrPostCallback d, object state)
		{
			d(state);
		}
	}
}
