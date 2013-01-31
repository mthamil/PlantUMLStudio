using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xunit;

namespace Tests.Unit
{
	/// <summary>
	/// Allows execution of a unit test with specific culture information.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class CultureAttribute : BeforeAfterTestAttribute
	{
		/// <summary>
		/// Specifies that a specific culture should be used during execution of a test.
		/// </summary>
		/// <param name="cultureName">The name of the culture that should be used for a test.</param>
		public CultureAttribute(string cultureName)
		{
			_newCulture = CultureInfo.GetCultureInfo(cultureName);
		}

		/// <summary>
		/// Stores the current thread's culture info and sets the thread's culture to the culture
		/// specified by the culture string.
		/// </summary>
		public override void Before(MethodInfo methodUnderTest)
		{
			_originalCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = _newCulture;
		}

		/// <summary>
		/// Restores a thread's original culture info.
		/// </summary>
		public override void After(MethodInfo methodUnderTest)
		{
			Thread.CurrentThread.CurrentCulture = _originalCulture;
		}

		private CultureInfo _originalCulture;
		private readonly CultureInfo _newCulture;
	}
}