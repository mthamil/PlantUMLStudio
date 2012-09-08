using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Utilities.InputOutput;

namespace PlantUmlEditor.Core.InputOutput
{
	/// <summary>
	/// Provides diagram reading/writing operations.
	/// </summary>
	public class DiagramIOService : IDiagramIOService
	{
		/// <summary>
		/// Initializes the service.
		/// </summary>
		/// <param name="scheduler">The scheduler to use for executing tasks</param>
		/// <param name="monitor">Monitors the file system</param>
		public DiagramIOService(TaskScheduler scheduler, IDirectoryMonitor monitor)
		{
			_scheduler = scheduler;
			_monitor = monitor;

			_monitor.Created += monitor_Created;
			_monitor.Deleted += monitor_Deleted;
		}

		/// <summary>
		/// The diagram file filter.
		/// </summary>
		public string FileFilter { get; set; }

		#region Implementation of IDiagramIOService

		/// <see cref="IDiagramIOService.ReadDiagramsAsync"/>
		public Task<IEnumerable<Diagram>> ReadDiagramsAsync(DirectoryInfo directory, CancellationToken cancellationToken, IProgress<ReadDiagramsProgress> progress)
		{
			return Task<IEnumerable<Diagram>>.Factory.StartNew(() =>
			{
				var diagrams = new List<Diagram>();

				FileInfo[] files = directory.GetFiles(FileFilter);
				int numberOfFiles = files.Length;
				int processed = 0;
				foreach (FileInfo file in files)
				{
					if (cancellationToken.IsCancellationRequested)
						break;

					var diagram = ReadImpl(file);
					
					if (diagram != null)
						diagrams.Add(diagram);
					//Thread.Sleep(500);

					processed++;
					if (progress != null)
						progress.Report(new ReadDiagramsProgress(processed, numberOfFiles, diagram));
				}

				return diagrams;

			}, CancellationToken.None, TaskCreationOptions.None, _scheduler);
		}

		/// <see cref="IDiagramIOService.ReadAsync"/>
		public Task<Diagram> ReadAsync(FileInfo file)
		{
			return Task.Factory.StartNew(() => ReadImpl(file),
				CancellationToken.None,
				TaskCreationOptions.None,
				_scheduler);
		}

		private static Diagram ReadImpl(FileInfo file)
		{
			string content;
			using (var reader = new StreamReader(file.OpenRead()))
				content = reader.ReadToEnd();

			if (!String.IsNullOrWhiteSpace(content))
			{
				// Check that the given file's content appears to be a plantUML diagram.
				Match match = diagramStartPattern.Match(content);
				if (match.Success && match.Groups.Count > 1)
				{
					string imageFileName = match.Groups[1].Value;
					var imageFilePath = Path.IsPathRooted(imageFileName)
											? Path.GetFullPath(imageFileName)
											: Path.GetFullPath(Path.Combine(file.DirectoryName, imageFileName));

					return new Diagram
					{
						Content = content,
						File = file,
						ImageFilePath = imageFilePath
					};
				}
			}

			return null;
		}

		/// <see cref="IDiagramIOService.SaveAsync"/>
		public Task SaveAsync(Diagram diagram, bool makeBackup)
		{
			return Task.Factory.StartNew(() =>
			{
				if (makeBackup)
					diagram.File.CopyTo(diagram.File.FullName + ".bak", true);

				//Thread.Sleep(4000);
				// Save the diagram content using UTF-8 encoding to support 
				// various international characters, which ASCII won't support
				// and Unicode won't make it cross platform
				File.WriteAllText(diagram.File.FullName, diagram.Content, Encoding.UTF8);

			}, CancellationToken.None, TaskCreationOptions.None, _scheduler);
		}

		/// <see cref="IDiagramIOService.DeleteAsync"/>
		public Task DeleteAsync(Diagram diagram)
		{
			return Task.Factory.StartNew(() =>
			{
				diagram.File.Delete();
				if (File.Exists(diagram.ImageFilePath))
					File.Delete(diagram.ImageFilePath);
			}, CancellationToken.None, TaskCreationOptions.None, _scheduler);
		}

		/// <see cref="IDiagramIOService.StartMonitoring"/>
		public void StartMonitoring(DirectoryInfo directory)
		{
			_monitor.StartMonitoring(directory);
		}

		/// <see cref="IDiagramIOService.StopMonitoring"/>
		public void StopMonitoring()
		{
			_monitor.StopMonitoring();
		}

		/// <see cref="IDiagramIOService.DiagramFileAdded"/>
		public event EventHandler<DiagramFileAddedEventArgs> DiagramFileAdded;

		private void OnDiagramFileAdded(FileInfo newDiagramFile)
		{
			var localEvent = DiagramFileAdded;
			if (localEvent != null)
				localEvent(this, new DiagramFileAddedEventArgs(newDiagramFile));
		}

		/// <see cref="IDiagramIOService.DiagramFileDeleted"/>
		public event EventHandler<DiagramFileDeletedEventArgs> DiagramFileDeleted;

		private void OnDiagramFileDeleted(FileInfo deletedDiagramFile)
		{
			var localEvent = DiagramFileDeleted;
			if (localEvent != null)
				localEvent(this, new DiagramFileDeletedEventArgs(deletedDiagramFile));
		}

		#endregion

		void monitor_Deleted(object sender, FileSystemEventArgs e)
		{
			OnDiagramFileDeleted(new FileInfo(e.FullPath));
		}

		void monitor_Created(object sender, FileSystemEventArgs e)
		{
			OnDiagramFileAdded(new FileInfo(e.FullPath));
		}

		private readonly TaskScheduler _scheduler;
		private readonly IDirectoryMonitor _monitor;

		private static readonly Regex diagramStartPattern = new Regex(@"@startuml\s*(?:"")*([^\r\n""]*)", 
			RegexOptions.IgnoreCase |
			RegexOptions.Multiline |
			RegexOptions.IgnorePatternWhitespace |
			RegexOptions.Compiled);
	}
}