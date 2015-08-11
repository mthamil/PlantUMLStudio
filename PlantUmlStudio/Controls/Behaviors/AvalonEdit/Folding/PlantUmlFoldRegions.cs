//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Collections;
using System.Collections.Generic;

namespace PlantUmlStudio.Controls.Behaviors.AvalonEdit.Folding
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
			return Regions.GetEnumerator();
		}

		/// <see cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private static readonly IList<FoldedRegionDefinition> Regions = new List<FoldedRegionDefinition>
		{
			{ new FoldedRegionDefinition(@"(^|\s+)note (left|right|top|bottom|over)([^\S\n]+.*)?$",													@"(^|\s+)end note($|\s+)") },
			{ new FoldedRegionDefinition(@"(^|\s+)package[^\S\n]+[^{]+$",																			@"(^|\s+)end package($|\s+)") },
			{ new FoldedRegionDefinition(@"(^|\s+)activate\s+(?<id>\w+)[^\S\n]*$",																	@"(^|\s+)deactivate +") },
			{ new FoldedRegionDefinition(@"(^|\s+)if.+then[^\S\n]*$",																				@"(^|\s+)endif($|\s+)") },
			{ new FoldedRegionDefinition(@"^[^\S\n]*title[^\S\n]*$",																				@"(^|\s+)end title($|\s+)") },
			{ new FoldedRegionDefinition(@"^[^\S\n]*box[^\S\n]+.*$",																				@"(^|\s+)end box($|\s+)") },
			{ new FoldedRegionDefinition(@"(^|\s+)(partition|package|namespace|abstract class|class|interface|enum|state)[^\S\n]+.+{[^\S\n]*$",		@"(^|\s+)}.*$") },
		};
	}
}