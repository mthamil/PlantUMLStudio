using System.Windows;
using Utilities.Mvvm.Commands;

namespace Utilities.Controls.Commands
{
	/// <summary>
	/// Closes a window.
	/// </summary>
	public class CloseWindowCommand : RelayCommand<Window>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CloseWindowCommand() 
			: base(w => w.Close()) { }
	}
}