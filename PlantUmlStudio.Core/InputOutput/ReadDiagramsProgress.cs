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

using SharpEssentials;

namespace PlantUmlStudio.Core.InputOutput
{
	/// <summary>
	/// Contains information about progress made in reading diagrams.
	/// </summary>
	public class ReadDiagramsProgress
	{
		/// <summary>
		/// Creates new progress data.
		/// </summary>
		/// <param name="processedDiagramCount">The number of diagrams read so far</param>
		/// <param name="totalDiagramCount">The total number of diagrams to read</param>
		/// <param name="diagram">The last read diagram</param>
		public ReadDiagramsProgress(int processedDiagramCount, int totalDiagramCount, Option<Diagram> diagram)
		{
			Diagram = diagram;
			TotalDiagramCount = totalDiagramCount;
			ProcessedDiagramCount = processedDiagramCount;
		}

		/// <summary>
		/// The number of diagrams read so far.
		/// </summary>
		public int ProcessedDiagramCount { get; private set; }

		/// <summary>
		/// The total number of diagrams to read.
		/// </summary>
		public int TotalDiagramCount { get; private set; }

		/// <summary>
		/// The last read diagram.
		/// </summary>
		public Option<Diagram> Diagram { get; private set; }
	}
}