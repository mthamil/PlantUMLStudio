using ICSharpCode.AvalonEdit.Document;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Interface for an object that provides an undo stack.
	/// </summary>
	public interface IUndoProvider
	{
		/// <summary>
		/// The provided undo stack.
		/// </summary>
		UndoStack UndoStack { get; }
	}
}