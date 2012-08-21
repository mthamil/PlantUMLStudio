﻿using System;
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
		/// <param name="fileSystemWatcher">Monitors the file system</param>
		public DiagramIOService(TaskScheduler scheduler, IFileSystemWatcher fileSystemWatcher)
		{
			_scheduler = scheduler;
			_fileSystemWatcher = fileSystemWatcher;

			_fileSystemWatcher.Created += fileSystemWatcher_Created;
			_fileSystemWatcher.Deleted += fileSystemWatcher_Deleted;
		}

		#region Implementation of IDiagramIOService

		/// <see cref="IDiagramIOService.ReadDiagramsAsync"/>
		public Task<IEnumerable<Diagram>> ReadDiagramsAsync(DirectoryInfo directory, IProgress<Tuple<int, int>> progress)
		{
			return Task<IEnumerable<Diagram>>.Factory.StartNew(() =>
			{
				var diagrams = new List<Diagram>();

				FileInfo[] files = directory.GetFiles("*.puml");
				int numberOfFiles = files.Length;
				int processed = 0;
				foreach (FileInfo file in files)
				{
					var diagram = ReadImpl(file);
					if (diagram != null)
						diagrams.Add(diagram);
					//Thread.Sleep(500);

					processed++;
					if (progress != null)
						progress.Report(Tuple.Create(processed, numberOfFiles));
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
			_fileSystemWatcher.Path = directory.FullName;
			_fileSystemWatcher.EnableRaisingEvents = true;
		}

		/// <see cref="IDiagramIOService.StopMonitoring"/>
		public void StopMonitoring()
		{
			_fileSystemWatcher.EnableRaisingEvents = false;
		}

		/// <see cref="IDiagramIOService.DiagramAdded"/>
		public event EventHandler<DiagramAddedEventArgs> DiagramAdded;

		private void OnDiagramAdded(Diagram newDiagram)
		{
			var localEvent = DiagramAdded;
			if (localEvent != null)
				localEvent(this, new DiagramAddedEventArgs(newDiagram));
		}

		/// <see cref="IDiagramIOService.DiagramDeleted"/>
		public event EventHandler<DiagramDeletedEventArgs> DiagramDeleted;

		private void OnDiagramDeleted(FileInfo deletedDiagramFile)
		{
			var localEvent = DiagramDeleted;
			if (localEvent != null)
				localEvent(this, new DiagramDeletedEventArgs(deletedDiagramFile));
		}

		#endregion

		void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			OnDiagramDeleted(new FileInfo(e.FullPath));
		}

		void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
		{
			var newDiagram = ReadImpl(new FileInfo(e.FullPath));
			OnDiagramAdded(newDiagram);
		}

		private readonly TaskScheduler _scheduler;
		private readonly IFileSystemWatcher _fileSystemWatcher;

		private static readonly Regex diagramStartPattern = new Regex(@"@startuml\s*(?:"")*([^\r\n""]*)", 
			RegexOptions.IgnoreCase |
			RegexOptions.Multiline |
			RegexOptions.IgnorePatternWhitespace |
			RegexOptions.Compiled);
	}
}