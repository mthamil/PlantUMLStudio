using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Concurrency;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Provides diagram reading/writing operations.
	/// </summary>
	public class DiagramIOService : IDiagramIOService
	{
		/// <summary>
		/// Initializes the service.
		/// </summary>
		/// <param name="diagramCompiler">Compiles diagram code to an image</param>
		/// <param name="scheduler">The scheduler to use for executing tasks</param>
		public DiagramIOService(IDiagramCompiler diagramCompiler, TaskScheduler scheduler)
		{
			_diagramCompiler = diagramCompiler;
			_scheduler = scheduler;
		}

		#region Implementation of IDiagramIOService

		/// <see cref="IDiagramIOService.ReadDiagramsAsync"/>
		public Task<IList<Diagram>> ReadDiagramsAsync(DirectoryInfo directory, IProgress<Tuple<int?, string>> progress)
		{
			return Task<IList<Diagram>>.Factory.StartNew(() =>
			{
				var diagrams = new List<Diagram>();

				FileInfo[] files = directory.GetFiles("*.txt");
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
					{
						progress.Report(Tuple.Create(
							(int?)(processed/(double)numberOfFiles*100),
							String.Format("Loading {0} of {1}", processed, numberOfFiles)));
					}
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
				Match match = Regex.Match(content, @"@startuml\s*(?:"")*([^\r\n""]*)",
										  RegexOptions.IgnoreCase
										  | RegexOptions.Multiline
										  | RegexOptions.IgnorePatternWhitespace
										  | RegexOptions.Compiled
					);

				if (match.Success && match.Groups.Count > 1)
				{
					string imageFileName = match.Groups[1].Value;
					var imageFilePath = Path.IsPathRooted(imageFileName)
											? Path.GetFullPath(imageFileName)
											: Path.GetFullPath(Path.Combine(file.DirectoryName, imageFileName));

					return new Diagram
					{
						Content = content,
						DiagramFilePath = file.FullName,
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
				var diagramFile = new FileInfo(diagram.DiagramFilePath);

				if (makeBackup)
					diagramFile.CopyTo(diagramFile.FullName + ".bak", true);

				//Thread.Sleep(4000);
				// Save the diagram content using UTF-8 encoding to support 
				// various international characters, which ASCII won't support
				// and Unicode won't make it cross platform
				File.WriteAllText(diagramFile.FullName, diagram.Content, Encoding.UTF8);

				_diagramCompiler.Compile(diagram);

			}, CancellationToken.None, TaskCreationOptions.None, _scheduler);
		}

		#endregion

		private readonly IDiagramCompiler _diagramCompiler;
		private readonly TaskScheduler _scheduler;
	}
}