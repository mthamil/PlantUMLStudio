namespace Utilities.Clipboard
{
	/// <summary>
	/// Interface for a clipboard.
	/// </summary>
	public interface IClipboard
	{
		/// <summary>
		/// Whether a clipboard contains text data.
		/// </summary>
		bool ContainsText { get; }

		/// <summary>
		/// Retrieves text data from a clipboard.
		/// </summary>
		/// <returns>Any stored text</returns>
		string GetText();

		/// <summary>
		/// Stores text on a clipboard.
		/// </summary>
		/// <param name="text">The text to store</param>
		void SetText(string text);
	}
}