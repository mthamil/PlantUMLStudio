using System.ComponentModel;
using Utilities.Controls.Behaviors.AvalonEdit;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Interface for code editor content.
	/// </summary>
	public interface ICodeEditor : INotifyPropertyChanged, IUndoProvider
	{
		/// <summary>
		/// The content being edited.
		/// </summary>
		string Content { get; set; }

		/// <summary>
		/// The current index into the content.
		/// </summary>
		int ContentIndex { get; set; }

		/// <summary>
		/// Whether content has been modified since the last save.
		/// </summary>
		bool IsModified { get; set; }
	}
}