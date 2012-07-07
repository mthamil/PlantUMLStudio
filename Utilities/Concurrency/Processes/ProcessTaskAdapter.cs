﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Concurrency.Processes
{
	/// <summary>
	/// Executes Processes and returns Task representations of them.
	/// </summary>
	public class ProcessTaskAdapter : IProcessTaskAdapter
	{
		/// <summary>
		/// Initializes an adapter.
		/// </summary>
		/// <param name="taskScheduler">Used for any asynchronous operations other than a Process itself</param>
		public ProcessTaskAdapter(TaskScheduler taskScheduler)
		{
			_taskScheduler = taskScheduler;
		}

		/// <summary>
		/// Executes a Process that does not return results or take input.
		/// </summary>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="cancellationToken">Allows termination of the process</param>
		/// <returns>A Task that can be used to wait for the Process to complete</returns>
		public Task Execute(ProcessStartInfo processInfo, CancellationToken cancellationToken)
		{
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = processInfo
			};

			Stream errorStream = new MemoryStream();
			DataReceivedEventHandler errorHandler = (o, e) => WriteErrorData(errorStream, e.Data);
			cancellationToken.Register(() => CancelProcess(process), true);

			var tcs = new TaskCompletionSource<object>();
			EventHandler exitedHandler = null;
			exitedHandler = (o, e) =>
			{
				process.Exited -= exitedHandler;
				process.ErrorDataReceived -= errorHandler;

				if (cancellationToken.IsCancellationRequested)
				{
					tcs.TrySetCanceled();
				}
				else if (errorStream.Length > 0)
				{
					tcs.TrySetException(CreateExceptionFromErrorStream(errorStream));
				}
				else
				{
					tcs.TrySetResult(null);
				}

				process.Dispose();
			};

			process.Exited += exitedHandler;
			process.ErrorDataReceived += errorHandler;
			if (process.Start())
				process.BeginErrorReadLine();

			return tcs.Task;
		}

		/// <summary>
		/// Executes a Process that takes data written to its input stream
		/// and returns data read from its output stream.  Note that the entire stream 
		/// is read into memory so for large outputs this method may not be appropriate.
		/// </summary>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="input">The data to write to the Process's input stream</param>
		/// <param name="cancellationToken">Allows termination of the process</param>
		/// <returns> A Task that, when completed successfully, contains a Process's output</returns>
		public Task<Stream> Execute(ProcessStartInfo processInfo, Stream input, CancellationToken cancellationToken)
		{
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = processInfo
			};

			Stream outputStream = new MemoryStream();
			Stream errorStream = new MemoryStream();

			DataReceivedEventHandler errorHandler = (o, e) => WriteErrorData(errorStream, e.Data);
			cancellationToken.Register(() => CancelProcess(process), true);

			var tcs = new TaskCompletionSource<Stream>();
			EventHandler exitedHandler = null;
			exitedHandler = (o, e) =>
			{
				process.Exited -= exitedHandler;
				process.ErrorDataReceived -= errorHandler;

				if (cancellationToken.IsCancellationRequested)
				{
					tcs.TrySetCanceled();
				}
				else if (errorStream.Length > 0)
				{
					tcs.TrySetException(CreateExceptionFromErrorStream(errorStream));
				}
				else
				{
					outputStream.Position = 0;
					tcs.TrySetResult(outputStream);
				}

				process.Dispose();
			};

			process.Exited += exitedHandler;
			process.ErrorDataReceived += errorHandler;
			if (process.Start())
			{
				// Launch a task to read output and error streams.
				Task.Factory.StartNew(() =>
				{
					process.BeginErrorReadLine();
					process.StandardOutput.BaseStream.CopyTo(outputStream);
				}, cancellationToken, TaskCreationOptions.None, _taskScheduler);

				// Launch another task to write input.
				Task.Factory.StartNew(() =>
				{
					input.CopyTo(process.StandardInput.BaseStream);
					process.StandardInput.Close();
				}, cancellationToken, TaskCreationOptions.None, _taskScheduler);
			}

			return tcs.Task;
		}

		private static void WriteErrorData(Stream stream, string errorData)
		{
			if (errorData != null)
			{
				var bytes = Encoding.Default.GetBytes(errorData);
				stream.Write(bytes, 0, bytes.Length);
			}
		}

		private static ProcessErrorException CreateExceptionFromErrorStream(Stream errorStream)
		{
			errorStream.Position = 0;
			string error;
			using (var reader = new StreamReader(errorStream))
				error = reader.ReadToEnd();

			return new ProcessErrorException(error);
		}

		private static void CancelProcess(Process process)
		{
			try
			{
				process.Kill();
			}
			catch (InvalidOperationException)
			{
				// This may happen if the process already exited.
				// Calling Process.HasExited doesn't seem to be any help.
			}
		}

		private readonly TaskScheduler _taskScheduler;
	}
}