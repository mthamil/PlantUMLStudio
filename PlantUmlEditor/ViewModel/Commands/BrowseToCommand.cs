using System.Diagnostics;
using System.Windows.Documents;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel.Commands
{
	/// <summary>
	/// Launches a hyperlink in the default browser.
	/// </summary>
	public class BrowseToCommand : RelayCommand<Hyperlink>
	{
		public BrowseToCommand()
			: base(BrowseTo) { }

		private static void BrowseTo(Hyperlink link)
		{
			Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
		}
	}
}