using System.Collections;
using System.Collections.Generic;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Contains PlantUML syntax fold region definitions.
	/// </summary>
	public class PlantUmlFoldRegions : IEnumerable<FoldedRegionDefinition>
	{
		#region Implementation of IEnumerable

		/// <see cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<FoldedRegionDefinition> GetEnumerator()
		{
			return regions.GetEnumerator();
		}

		/// <see cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private static readonly IList<FoldedRegionDefinition> regions = new List<FoldedRegionDefinition>
		{
			{ new FoldedRegionDefinition(@"(^|\s+)note (left|right|top|bottom|over)([^\S\n]+.*)?$",									@"(^|\s+)end note($|\s+)") },
			{ new FoldedRegionDefinition(@"(^|\s+)package[^\S\n]+[^{]+$",															@"(^|\s+)end package($|\s+)") },
			{ new FoldedRegionDefinition(@"(^|\s+)activate\s+(?<id>\w+)[^\S\n]*$",													@"(^|\s+)deactivate +") },
			{ new FoldedRegionDefinition(@"(^|\s+)if.+then[^\S\n]*$",																@"(^|\s+)endif($|\s+)") },
			{ new FoldedRegionDefinition(@"^[^\S\n]*title[^\S\n]*$",																@"(^|\s+)end title($|\s+)") },
			{ new FoldedRegionDefinition(@"^[^\S\n]*box[^\S\n]*.*$",																@"(^|\s+)end box($|\s+)") },
			{ new FoldedRegionDefinition(@"(^|\s+)(partition|package|namespace|abstract class|class|enum)[^\S\n]+.+{[^\S\n]*$",		@"(^|\s+)}.*$") },
		};
	}
}