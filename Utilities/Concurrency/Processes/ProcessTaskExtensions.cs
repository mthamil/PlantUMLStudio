using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Concurrency.Processes
{
	/// <summary>
	/// Provides methods that enable using a Process as a Task.
	/// </summary>
	public static class ProcessTaskExtensions
	{
		/// <summary>
		/// Creates a Task from a ProcessStartInfo that describes a process that takes a stream
		/// as input and returns its output.
		/// </summary>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="input">The process input stream</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task<Stream> ToTask(this ProcessStartInfo processInfo, Stream input, CancellationToken cancellationToken)
		{
			return ProcessAdapterFactory().StartNew(processInfo, input, cancellationToken);
		}

		/// <summary>
		/// Creates a Task from a ProcessStartInfo that describes a process that takes
		/// no input and returns no output.
		/// </summary>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task ToTask(this ProcessStartInfo processInfo, CancellationToken cancellationToken)
		{
			return ProcessAdapterFactory().StartNew(processInfo, cancellationToken);
		}

		internal static Func<IProcessTaskAdapter> ProcessAdapterFactory = () => new ProcessTaskAdapter(TaskScheduler.Default);
	}
}