using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using PlantUmlEditor.Model;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Interface for a diagram editor.
	/// </summary>
	public interface IDiagramEditor : INotifyPropertyChanged
	{
		/// <summary>
		/// Whether an editor is currently busy with some task.
		/// </summary>
		bool IsIdle { get; set; }

		/// <summary>
		/// Whether to automatically save a diagram's changes and regenerate its image.
		/// </summary>
		bool AutoSave { get; set; }

		/// <summary>
		/// The auto-save internval.
		/// </summary>
		TimeSpan AutoSaveInterval { get; set; }

		/// <summary>
		/// The underlying diagram.
		/// </summary>
		Diagram Diagram { get; }

		/// <summary>
		/// The rendered diagram image.
		/// </summary>
		ImageSource DiagramImage { get; }

		/// <summary>
		/// Whether an editor's content can currently be saved.
		/// </summary>
		bool CanSave { get; }

		/// <summary>
		/// Command that saves a diagram's changes.
		/// </summary>
		ICommand SaveCommand { get; }

		/// <summary>
		/// Asynchronously saves a diagram editor.
		/// </summary>
		/// <returns>A task representing the save operation</returns>
		Task Save();

		/// <summary>
		/// Whether a diagram's image can currently be refreshed.
		/// </summary>
		bool CanRefresh { get; }

		/// <summary>
		/// Refreshes a diagram's image without saving.
		/// </summary>
		ICommand RefreshCommand { get; }

		/// <summary>
		/// Command that closes a diagram editor.
		/// </summary>
		ICommand CloseCommand { get; }

		/// <summary>
		/// The code editor.
		/// </summary>
		ICodeEditor CodeEditor { get; }

		/// <summary>
		/// Event raised when a diagram editor indicates it will be closing.
		/// </summary>
		event CancelEventHandler Closing;

		/// <summary>
		/// Event raised when a diagram editor has been closed.
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// Event raised when a diagram editor saves.
		/// </summary>
		event EventHandler Saved;
	}
}