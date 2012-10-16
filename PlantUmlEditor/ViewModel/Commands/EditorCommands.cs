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
using ICSharpCode.AvalonEdit;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel.Commands
{
	/// <summary>
	/// Copies text in an AvalonEdit text editor.
	/// </summary>
	public class CopyCommand : RelayCommand<TextEditor>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CopyCommand()
			: base(editor => editor.Copy(), editor => editor != null) { }
	}

	/// <summary>
	/// Cuts text in an AvalonEdit text editor.
	/// </summary>
	public class CutCommand : RelayCommand<TextEditor>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CutCommand()
			: base(editor => editor.Cut(), editor => editor != null) { }
	}

	/// <summary>
	/// Pastes text in an AvalonEdit text editor.
	/// </summary>
	public class PasteCommand : RelayCommand<TextEditor>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public PasteCommand()
			: base(editor => editor.Paste(), editor => editor != null) { }
	}
}