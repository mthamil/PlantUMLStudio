using System;
using System.Collections.Generic;
using System.IO;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Provides browsing and previewing of diagram files.
	/// </summary>
	public interface IDiagramExplorer
	{
		/// <summary>
		/// The current explorer directory.
		/// </summary>
		DirectoryInfo DiagramLocation { get; }

		/// <summary>
		/// The currently available diagrams.
		/// </summary>
		ICollection<PreviewDiagramViewModel> PreviewDiagrams { get; }

		/// <summary>
		/// Event raised when a preview diagram should be opened for editing.
		/// </summary>
		event EventHandler<OpenPreviewRequestedEventArgs> OpenPreviewRequested;

		/// <summary>
		/// Event raised when a diagram has been deleted.
		/// </summary>
		event EventHandler<DiagramDeletedEventArgs> DiagramDeleted;
	}
}