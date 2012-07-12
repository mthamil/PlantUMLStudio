﻿using System;
using System.Collections.Generic;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents diagram previews.
	/// </summary>
	public interface IPreviewDiagrams
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