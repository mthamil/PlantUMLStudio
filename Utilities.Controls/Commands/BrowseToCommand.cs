using System.Diagnostics;
using System.Windows.Documents;
using Utilities.Mvvm.Commands;

namespace Utilities.Controls.Commands
{
	/// <summary>
	/// Launches a hyperlink in the default browser.
	/// </summary>
	public class BrowseToCommand : RelayCommand<Hyperlink>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public BrowseToCommand()
			: base(BrowseTo) { }

		private static void BrowseTo(Hyperlink link)
		{
			Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
		}
	}
}