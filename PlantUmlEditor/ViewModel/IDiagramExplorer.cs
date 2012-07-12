using System;
using System.Collections.Generic;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Provides browsing and previewing of diagram files.
	/// </summary>
	public interface IDiagramExplorer
	{
		/// <summary>
		/// Event raised whena  new diagram is created.
		/// </summary>
		event EventHandler<NewDiagramCreatedEventArgs> NewDiagramCreated;

		/// <summary>
		/// The currently available diagrams.
		/// </summary>
		ICollection<PreviewDiagramViewModel> PreviewDiagrams { get; }
	}
}