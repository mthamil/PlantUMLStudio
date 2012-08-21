using System;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Event args containing information about when a diagram should be opened for editing.
	/// </summary>
	public class OpenPreviewRequestedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="requestedPreview">The preview to open for editing</param>
		public OpenPreviewRequestedEventArgs(PreviewDiagramViewModel requestedPreview)
		{
			RequestedPreview = requestedPreview;
		}

		/// <summary>
		/// The preview to open for editing.
		/// </summary>
		public PreviewDiagramViewModel RequestedPreview { get; private set; }
	}
}