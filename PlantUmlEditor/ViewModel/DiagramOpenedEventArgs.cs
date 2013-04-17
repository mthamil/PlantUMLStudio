//  PlantUML Editor
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
using PlantUmlEditor.Core;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Event args for when a diagram is opened for editing.
	/// </summary>
	public class DiagramOpenedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes new event args.
		/// </summary>
		/// <param name="diagram">The diagram that was opened</param>
		public DiagramOpenedEventArgs(Diagram diagram)
		{
			Diagram = diagram;
		}

		/// <summary>
		/// The diagram that was opened.
		/// </summary>
		public Diagram Diagram { get; private set; }
	}
}