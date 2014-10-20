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

using SharpEssentials.Observable;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Contains various text editor options.
	/// </summary>
	public class EditorOptions : ObservableObject
	{
		/// <summary>
		/// Initializes new <see cref="EditorOptions"/>.
		/// </summary>
		public EditorOptions()
		{
			_highlightCurrentLine = Property.New(this, p => p.HighlightCurrentLine, OnPropertyChanged);
			_showLineNumbers = Property.New(this, p => p.ShowLineNumbers, OnPropertyChanged);
			_enableVirtualSpace = Property.New(this, p => p.EnableVirtualSpace, OnPropertyChanged);
			_enableWordWrap = Property.New(this, p => p.EnableWordWrap, OnPropertyChanged);
			_emptySelectionCopiesEntireLine = Property.New(this, p => p.EmptySelectionCopiesEntireLine, OnPropertyChanged);
			_allowScrollingBelowContent = Property.New(this, p => p.AllowScrollingBelowContent, OnPropertyChanged);
		}

		/// <summary>
		/// Whether to highlight the current line.
		/// </summary>
		public bool HighlightCurrentLine 
		{
			get { return _highlightCurrentLine.Value; }
			set { _highlightCurrentLine.Value = value; }
		}

		/// <summary>
		/// Whether to display line numbers.
		/// </summary>
		public bool ShowLineNumbers
		{
			get { return _showLineNumbers.Value; }
			set { _showLineNumbers.Value = value; }
		}

		/// <summary>
		/// Whether virtual space is enabled. That is, can editing occur beyond the end of a line.
		/// </summary>
		public bool EnableVirtualSpace
		{
			get { return _enableVirtualSpace.Value; }
			set { _enableVirtualSpace.Value = value; }
		}

		/// <summary>
		/// Whether word wrap is enabled.
		/// </summary>
		public bool EnableWordWrap
		{
			get { return _enableWordWrap.Value; }
			set { _enableWordWrap.Value = value; }
		}

		/// <summary>
		/// Whether a cut or copy operation with no text selected copies the entire current line.
		/// </summary>
		public bool EmptySelectionCopiesEntireLine
		{
			get { return _emptySelectionCopiesEntireLine.Value; }
			set { _emptySelectionCopiesEntireLine.Value = value; }
		}

		/// <summary>
		/// Whether to allow vertical srolling past the end of an editor's content.
		/// </summary>
		public bool AllowScrollingBelowContent
		{
			get { return _allowScrollingBelowContent.Value; }
			set { _allowScrollingBelowContent.Value = value; }
		}

		private readonly Property<bool> _highlightCurrentLine;
		private readonly Property<bool> _showLineNumbers;
		private readonly Property<bool> _enableVirtualSpace;
		private readonly Property<bool> _enableWordWrap;
		private readonly Property<bool> _emptySelectionCopiesEntireLine;
		private readonly Property<bool> _allowScrollingBelowContent;
	}
}