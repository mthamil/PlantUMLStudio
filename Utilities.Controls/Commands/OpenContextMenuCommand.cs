using System.Windows;
using Utilities.Mvvm.Commands;

namespace Utilities.Controls.Commands
{
	/// <summary>
	/// Command that opens a context menu.
	/// </summary>
	public class OpenContextMenuCommand : RelayCommand<FrameworkElement>
	{
		public OpenContextMenuCommand()
			: base(OpenContextMenu) { }

		private static void OpenContextMenu(FrameworkElement element)
		{
			element.ContextMenu.PlacementTarget = element;
			element.ContextMenu.IsOpen = true;
		}
	}
}