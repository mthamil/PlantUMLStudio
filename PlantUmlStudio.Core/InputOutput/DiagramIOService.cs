//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.InputOutput;

namespace PlantUmlStudio.Core.InputOutput
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
		public async Task<IEnumerable<Diagram>> ReadDiagramsAsync(DirectoryInfo directory, CancellationToken cancellationToken, IProgress<ReadDiagramsProgress> progress)
		{
			var files = await Task.Factory.StartNew(() => directory.GetFiles(FileFilter),
				cancellationToken, TaskCreationOptions.None, _scheduler).ConfigureAwait(false);

			int numberOfFiles = files.Length;
			int processed = 0;
			var diagrams = new List<Diagram>(numberOfFiles);
			foreach (var file in files)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var diagram = await ReadFileAsync(file).ConfigureAwait(false);
				diagram.Apply(diagrams.Add);
				//Thread.Sleep(500);

				processed++;
				if (progress != null)
					progress.Report(new ReadDiagramsProgress(processed, numberOfFiles, diagram));
			}

			return diagrams;
		}

		/// <see cref="IDiagramIOService.ReadAsync"/>
		public async Task<Diagram> ReadAsync(FileInfo file)
		{
			var diagram = await ReadFileAsync(file).ConfigureAwait(false);
			return diagram.GetOrElse(() => { throw new InvalidDiagramFileException(file); });
		}

		private static async Task<Option<Diagram>> ReadFileAsync(FileInfo file)
		{
			string content;
			using (var reader = new StreamReader(file.OpenRead()))
				content = await reader.ReadToEndAsync().ConfigureAwait(false);

			if (!String.IsNullOrWhiteSpace(content))
			{
				// Check that the given file's content appears to be a plantUML diagram.
				Match match = diagramStartPattern.Match(content);
				if (match.Success && match.Groups.Count > 1)
				{
					var diagram = new Diagram
					{
						Content = content,
						File = file
					};
					if (diagram.TryDeduceImageFile())
						return diagram;
				}
			}

			return Option<Diagram>.None();
		}

		/// <see cref="IDiagramIOService.SaveAsync"/>
		public async Task SaveAsync(Diagram diagram, bool makeBackup)
		{
			if (makeBackup)
				await diagram.File.CopyToAsync(diagram.File.FullName + ".bak", true).ConfigureAwait(false);

			//Thread.Sleep(4000);
			// Save the diagram content using UTF-8 encoding to support 
			// various international characters, which ASCII won't support
			// and Unicode won't make it cross platform.
			using (var writer = new StreamWriter(diagram.File.Open(FileMode.Create, FileAccess.Write), Encoding.UTF8))
				await writer.WriteAsync(diagram.Content).ConfigureAwait(false);
		}

		/// <see cref="IDiagramIOService.DeleteAsync"/>
		public Task DeleteAsync(Diagram diagram)
		{
			return Task.Factory.StartNew(() =>
			{
				diagram.File.Delete();
				if (diagram.ImageFile.Exists)
					diagram.ImageFile.Delete();
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