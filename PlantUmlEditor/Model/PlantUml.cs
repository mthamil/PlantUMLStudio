using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Provides an interface to PlantUML.
	/// </summary>
	public class PlantUml : IDiagramCompiler
	{
		/// <summary>
		/// Initializes PlantUML.
		/// </summary>
		/// <param name="taskScheduler">The task scheduler to use</param>
		public PlantUml(TaskScheduler taskScheduler)
		{
			_taskScheduler = taskScheduler;
		}

		/// <see cref="IDiagramCompiler.CompileToImage"/>
		public Task<BitmapSource> CompileToImage(string diagramCode, CancellationToken cancellationToken)
		{
			return ExecuteProcessAsync(new ProcessStartInfo
			{
				FileName = "java",
				Arguments = String.Format(@"-jar ""{0}"" -graphvizdot ""{1}"" -pipe", PlantUmlJar.FullName, GraphVizExecutable.FullName),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}, new MemoryStream(Encoding.Default.GetBytes(diagramCode)), cancellationToken)
			.ContinueWith(t =>
			{
				var bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.StreamSource = t.Result.Item1;
				bitmap.EndInit();
				bitmap.Freeze();
				return (BitmapSource)bitmap;
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
		}
		
		/// <see cref="IDiagramCompiler.CompileToFile"/>
		public Task CompileToFile(Diagram diagram)
		{          
			return ExecuteProcessAsync(new ProcessStartInfo
			{
				FileName = "java",
				Arguments = String.Format(@"-jar ""{0}"" -graphvizdot ""{1}"" ""{2}""", PlantUmlJar.FullName, GraphVizExecutable.FullName, diagram.DiagramFilePath),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardError = true,
				UseShellExecute = false
			});
		}

		private static Task ExecuteProcessAsync(ProcessStartInfo processInfo)
		{
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = processInfo
			};

			var tcs = new TaskCompletionSource<object>();
			EventHandler exitedHandler = null;
			exitedHandler = (o, e) =>
			{
				process.Exited -= exitedHandler;
				process.Dispose();
				tcs.SetResult(null);
			};

			process.Exited += exitedHandler;
			process.Start();

			return tcs.Task;
		}

		private Task<Tuple<Stream, Stream>> ExecuteProcessAsync(ProcessStartInfo processInfo, Stream input, CancellationToken cancellationToken)
		{
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = processInfo
			};

			Stream outputStream = new MemoryStream();
			Stream errorStream = new MemoryStream();

			DataReceivedEventHandler errorHandler = CreateStreamHandler(errorStream);

			cancellationToken.Register(() =>
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
				
			}, true);

			var tcs = new TaskCompletionSource<Tuple<Stream, Stream>>();
			EventHandler exitedHandler = null;
			exitedHandler = (o, e) =>
			{
				process.Exited -= exitedHandler;
				process.ErrorDataReceived -= errorHandler;

				//int exitCode = process.ExitCode;
				process.Dispose();

				if (cancellationToken.IsCancellationRequested)
				{
					tcs.SetCanceled();
				}
				else
				{
					outputStream.Position = 0;
					errorStream.Position = 0;
					tcs.SetResult(Tuple.Create(outputStream, errorStream));
				}
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
				},
				cancellationToken, TaskCreationOptions.None, _taskScheduler);
			}

			return tcs.Task;
		}

		private static DataReceivedEventHandler CreateStreamHandler(Stream stream)
		{
			return (o, e) =>
			{
				if (e.Data != null)
				{
					var bytes = Encoding.Default.GetBytes(e.Data);
					stream.Write(bytes, 0, bytes.Length);
				}
			};
		}

		/// <summary>
		/// The location of the GraphViz executable.
		/// </summary>
		public FileInfo GraphVizExecutable { get; set; }

		/// <summary>
		/// The location of the plantuml.jar file.
		/// </summary>
		public FileInfo PlantUmlJar { get; set; }

		private readonly TaskScheduler _taskScheduler;
	}
}