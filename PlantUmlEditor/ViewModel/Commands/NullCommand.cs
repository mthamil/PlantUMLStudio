using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel.Commands
{
	/// <summary>
	/// A command with no action and never executes.
	/// </summary>
	public class NullCommand : RelayCommand
	{
		public NullCommand()
			: base(() => { }, () => false) { }
	}
}