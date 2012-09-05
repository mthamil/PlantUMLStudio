using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Concurrency.Processes
{
	/// <summary>
	/// Interface for an object that executes asynchronous operations through external Processes.
	/// </summary>
	public interface IProcessTaskAdapter
	{
		/// <summary>
		/// Executes a Process that does not return results or take input.
		/// </summary>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="cancellationToken">Allows termination of the process</param>
		/// <returns>A Task that can be used to wait for the Process to complete</returns>
		Task StartNew(ProcessStartInfo processInfo, CancellationToken cancellationToken);

		/// <summary>
		/// Executes a Process that takes data written to its input stream
		/// and returns data read from its output stream.
		/// </summary>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="input">The data to write to the Process's input stream</param>
		/// <param name="cancellationToken">Allows termination of the process</param>
		/// <returns> A Task that, when completed successfully, contains a Process's output</returns>
		Task<Stream> StartNew(ProcessStartInfo processInfo, Stream input, CancellationToken cancellationToken);
	}
}