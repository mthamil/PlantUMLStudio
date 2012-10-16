//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
using System;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Event args containing information about when a diagram should be opened for editing.
	/// </summary>
	public class OpenPreviewRequestedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates new event args.
		/// </summary>
		/// <param name="requestedPreview">The preview to open for editing</param>
		public OpenPreviewRequestedEventArgs(PreviewDiagramViewModel requestedPreview)
		{
			RequestedPreview = requestedPreview;
		}

		/// <summary>
		/// The preview to open for editing.
		/// </summary>
		public PreviewDiagramViewModel RequestedPreview { get; private set; }
	}
}