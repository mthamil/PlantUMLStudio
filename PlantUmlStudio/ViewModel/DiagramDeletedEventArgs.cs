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

using System;
using PlantUmlStudio.Core;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Event args containing information about when a diagram's underlying file has been deleted.
	/// </summary>
	public class DiagramDeletedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="deletedDiagram">The deleted diagram</param>
		public DiagramDeletedEventArgs(Diagram deletedDiagram)
		{
			DeletedDiagram = deletedDiagram;
		}

		/// <summary>
		/// The deleted diagram.
		/// </summary>
		public Diagram DeletedDiagram { get; }
	}
}