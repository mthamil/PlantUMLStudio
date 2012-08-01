namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Defines the start and end patterns of a fold region.
	/// </summary>
	public sealed class FoldedRegionDefinition
	{
		public FoldedRegionDefinition(string startPattern, string endPattern)
		{
			StartPattern = startPattern;
			EndPattern = endPattern;
		}

		/// <summary>
		/// The fold region start pattern.
		/// </summary>
		public string StartPattern { get; private set; }

		/// <summary>
		/// The fold region end pattern.
		/// </summary>
		public string EndPattern { get; private set; }
	}
}