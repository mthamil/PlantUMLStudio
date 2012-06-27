using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Compiles a diagram into it's visual representation.
	/// </summary>
	public class PlantUmlDiagramCompiler : IDiagramCompiler
	{
		/// <see cref="IDiagramCompiler.Compile"/>
		public void Compile(Diagram diagram)
		{
			if (!PlantUmlExecutable.Exists)
			{
				throw new FileNotFoundException(String.Format("Cannot find PlantUML executable!{0}{1}{2}",
					Environment.NewLine, PlantUmlExecutable.FullName, Environment.NewLine));
			}

			// Use plantuml to generate the graph.               
			using (var process = new Process())
			{
				var startInfo = new ProcessStartInfo
				{
					FileName = PlantUmlExecutable.FullName,
					Arguments = "-graphvizdot \"" + GraphVizLocation + "\" \"" + diagram.DiagramFilePath + "\"",
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true
				};
				process.StartInfo = startInfo;
				if (process.Start())
				{
					process.WaitForExit(10000);
				}
			}
		}

		/// <summary>
		/// The PlantUML executable.
		/// </summary>
		public FileInfo PlantUmlExecutable { get; set; }

		/// <summary>
		/// The location of GraphViz.
		/// </summary>
		public string GraphVizLocation { get; set; }
	}
}