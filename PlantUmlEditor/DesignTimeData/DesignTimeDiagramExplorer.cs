using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlantUmlEditor.Core;
using PlantUmlEditor.Model;
using PlantUmlEditor.ViewModel;

namespace PlantUmlEditor.DesignTimeData
{
	/// <summary>
	/// Used for design-time data.
	/// </summary>
	public class DesignTimeDiagramExplorer : IDiagramExplorer
	{
		public DesignTimeDiagramExplorer()
		{
			DiagramLocation = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
		}

		#region Implementation of IDiagramExplorer

		/// <see cref="IDiagramExplorer.DiagramLocation"/>
		public DirectoryInfo DiagramLocation { get; set; }

		/// <see cref="IDiagramExplorer.PreviewDiagrams"/>
		public ICollection<PreviewDiagramViewModel> PreviewDiagrams
		{
			get { return _diagrams.Select(d => new PreviewDiagramViewModel(d) { ImagePreview = _renderer.Render(d) }).ToList(); }
		}

		/// <see cref="IDiagramExplorer.OpenPreviewRequested"/>
		public event EventHandler<OpenPreviewRequestedEventArgs> OpenPreviewRequested;

		#endregion

		private readonly ICollection<Diagram> _diagrams = new DiagramFiles();
		private readonly IDiagramRenderer _renderer = new DiagramBitmapRenderer();
	}
}