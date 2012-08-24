using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using PlantUmlEditor.Core;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Interface for a diagram editor.
	/// </summary>
	public interface IDiagramEditor : INotifyPropertyChanged, IDisposable
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
		/// Asynchronously saves a diagram editor.
		/// </summary>
		/// <returns>A task representing the save operation</returns>
		Task SaveAsync();

		/// <summary>
		/// Event raised when a diagram editor saves.
		/// </summary>
		event EventHandler Saved;

		/// <summary>
		/// Closes a diagram editor.
		/// </summary>
		void Close();

		/// <summary>
		/// Event raised when a diagram editor indicates it will be closing.
		/// </summary>
		event CancelEventHandler Closing;

		/// <summary>
		/// Event raised when a diagram editor has been closed.
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// The code editor.
		/// </summary>
		ICodeEditor CodeEditor { get; }
	}
}