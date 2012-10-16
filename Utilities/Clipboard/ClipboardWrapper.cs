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
namespace Utilities.Clipboard
{
	/// <summary>
	/// Adapts System.Windows.Clipboard to an interface.
	/// </summary>
	public class ClipboardWrapper : IClipboard
	{
		/// <see cref="IClipboard.ContainsText"/>
		public bool ContainsText 
		{
			get { return System.Windows.Clipboard.ContainsText(); }
		}

		/// <see cref="IClipboard.GetText"/>
		public string GetText()
		{
			return System.Windows.Clipboard.GetText();
		}

		/// <see cref="IClipboard.SetText"/>
		public void SetText(string text)
		{
			System.Windows.Clipboard.SetText(text);
		}
	}
}