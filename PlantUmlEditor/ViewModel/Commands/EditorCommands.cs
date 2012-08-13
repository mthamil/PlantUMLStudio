using ICSharpCode.AvalonEdit;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel.Commands
{
	/// <summary>
	/// Copies text in an AvalonEdit text editor.
	/// </summary>
	public class CopyCommand : RelayCommand<TextEditor>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CopyCommand()
			: base(editor => editor.Copy(), editor => editor != null) { }
	}

	/// <summary>
	/// Cuts text in an AvalonEdit text editor.
	/// </summary>
	public class CutCommand : RelayCommand<TextEditor>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CutCommand()
			: base(editor => editor.Cut(), editor => editor != null) { }
	}

	/// <summary>
	/// Pastes text in an AvalonEdit text editor.
	/// </summary>
	public class PasteCommand : RelayCommand<TextEditor>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public PasteCommand()
			: base(editor => editor.Paste(), editor => editor != null) { }
	}
}