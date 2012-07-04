using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Implementation of a task scheduler that always executes tasks synchronously
	/// </summary>
	public class SynchronousTaskScheduler : TaskScheduler
	{
		/// <see cref="TaskScheduler.GetScheduledTasks"/>
		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return Enumerable.Empty<Task>();
		}

		/// <see cref="TaskScheduler.MaximumConcurrencyLevel"/>
		public override int MaximumConcurrencyLevel { get { return 1; } }

		/// <see cref="TaskScheduler.QueueTask"/>
		protected override void QueueTask(Task task)
		{
			TryExecuteTask(task);
		}

		/// <see cref="TaskScheduler.TryExecuteTaskInline"/>
		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			return TryExecuteTask(task);
		}
	}
}
