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

namespace PlantUmlStudio.Controls.Behaviors.AvalonEdit.Folding
{
	/// <summary>
	/// Defines the start and end patterns of a fold region.
	/// </summary>
	public sealed class FoldedRegionDefinition
	{
		/// <summary>
		/// Creates a new fold region definition.
		/// </summary>
		/// <param name="startPattern">The fold region start pattern</param>
		/// <param name="endPattern">The fold region end pattern</param>
		public FoldedRegionDefinition(string startPattern, string endPattern)
		{
			StartPattern = startPattern;
			EndPattern = endPattern;
		}

		/// <summary>
		/// The fold region start pattern.
		/// </summary>
		public string StartPattern { get; }

		/// <summary>
		/// The fold region end pattern.
		/// </summary>
		public string EndPattern { get; }
	}
}