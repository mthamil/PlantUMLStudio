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
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel.Commands
{
	/// <summary>
	/// Copies an image to the clipboard.
	/// </summary>
	public class CopyImageCommand : RelayCommand<BitmapSource>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CopyImageCommand()
			: base(bitmap => Clipboard.SetImage(bitmap)) { }
	}

	/// <summary>
	/// Copies text to the clipboard.
	/// </summary>
	public class CopyTextCommand : RelayCommand<string>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CopyTextCommand()
			: base(text => Clipboard.SetText(text)) { }
	}

	/// <summary>
	/// Opens a path in Explorer.
	/// </summary>
	public class BrowseToCommand : RelayCommand<string>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public BrowseToCommand()
			: base(path => Process.Start("explorer.exe", "/select," + path).Dispose()) { }
	}
}