using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Unit.Tests
{
	/// <summary>
	/// Ensures that Task exceptions do not go unobserved.
	/// </summary>
	public class FailOnUnobservedTaskExceptionAttribute : BeforeAfterTestAttribute
	{
		/// <summary>
		/// Stores the current synchronization context and sets the current
		/// context to be synchronous.
		/// </summary>
		/// <param name="methodUnderTest"></param>
		public override void Before(MethodInfo methodUnderTest)
		{
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
		}

		/// <summary>
		/// Restores the original synchronization context.
		/// </summary>
		/// <param name="methodUnderTest"></param>
		public override void After(MethodInfo methodUnderTest)
		{
			if (unobservedException != null)
				ExceptionUtility.RethrowWithNoStackTraceLoss(unobservedException.InnerException);
		}

		void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			unobservedException = e.Exception;
		}

		private AggregateException unobservedException;
	}
}