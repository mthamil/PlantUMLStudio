using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel.Commands
{
	/// <summary>
	/// Copies an image to the clipboard.
	/// </summary>
	public class CopyImageCommand : RelayCommand<BitmapSource>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CopyImageCommand()
			: base(bitmap => Clipboard.SetImage(bitmap)) { }
	}

	/// <summary>
	/// Copies text to the clipboard.
	/// </summary>
	public class CopyTextCommand : RelayCommand<string>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CopyTextCommand()
			: base(text => Clipboard.SetText(text)) { }
	}

	/// <summary>
	/// Opens a path in Explorer.
	/// </summary>
	public class BrowseToCommand : RelayCommand<string>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public BrowseToCommand()
			: base(path => Process.Start("explorer.exe", "/select," + path).Dispose()) { }
	}
}