namespace Utilities.Clipboard
{
	/// <summary>
	/// Adapts System.Windows.Clipboard to an interface.
	/// </summary>
	public class ClipboardWrapper : IClipboard
	{
		/// <see cref="IClipboard.ContainsText"/>
		public bool ContainsText 
		{
			get { return System.Windows.Clipboard.ContainsText(); }
		}

		/// <see cref="IClipboard.GetText"/>
		public string GetText()
		{
			return System.Windows.Clipboard.GetText();
		}

		/// <see cref="IClipboard.SetText"/>
		public void SetText(string text)
		{
			System.Windows.Clipboard.SetText(text);
		}
	}
}