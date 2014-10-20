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
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Snippets;
using PlantUmlStudio.Model.Snippets;
using SharpEssentials.Controls.Mvvm.Commands;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Represents a code snippet.
	/// </summary>
	public class SnippetViewModel : MenuViewModel
	{
		public SnippetViewModel(CodeSnippet codeSnippet)
		{
			if (codeSnippet == null) 
				throw new ArgumentNullException("codeSnippet");

			_snippet = codeSnippet.Code;
			Name = codeSnippet.Name;
			Command = new RelayCommand<TextEditor>(Insert);
		}

		private void Insert(TextEditor editor)
		{
			_snippet.Insert(editor.TextArea);
		}

		private readonly Snippet _snippet;
	}
}