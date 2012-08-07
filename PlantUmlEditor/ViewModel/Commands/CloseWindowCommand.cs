using System.Windows;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel.Commands
{
	/// <summary>
	/// Closes a window.
	/// </summary>
	public class CloseWindowCommand : RelayCommand<Window>
	{
		public CloseWindowCommand() 
			: base(w => w.Close()) { }
	}
}