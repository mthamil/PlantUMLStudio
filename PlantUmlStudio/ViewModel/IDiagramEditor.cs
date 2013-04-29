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
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.Imaging;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Interface for a diagram editor.
	/// </summary>
	public interface IDiagramEditor : INotifyPropertyChanged, IDisposable
	{
		/// <summary>
		/// Whether an editor is currently busy with some task.
		/// </summary>
		bool IsIdle { get; set; }

		/// <summary>
		/// Whether to automatically save a diagram's changes and regenerate its image.
		/// </summary>
		bool AutoSave { get; set; }

		/// <summary>
		/// The auto-save internval.
		/// </summary>
		TimeSpan AutoSaveInterval { get; set; }

		/// <summary>
		/// The underlying diagram.
		/// </summary>
		Diagram Diagram { get; }

		/// <summary>
		/// The rendered diagram image.
		/// </summary>
		ImageSource DiagramImage { get; set; }

		/// <summary>
		/// The desired diagram image format.
		/// </summary>
		ImageFormat ImageFormat { get; set; }

		/// <summary>
		/// Whether an editor's content can currently be saved.
		/// </summary>
		bool CanSave { get; }

		/// <summary>
		/// Asynchronously saves a diagram editor.
		/// </summary>
		/// <returns>A task representing the save operation</returns>
		Task SaveAsync();

		/// <summary>
		/// Event raised when a diagram editor saves.
		/// </summary>
		event EventHandler Saved;

		/// <summary>
		/// Forces a re-render of a diagram's image.
		/// </summary>
		/// <returns>A task representing the refresh operation</returns>
		Task RefreshAsync();

		/// <summary>
		/// Closes a diagram editor.
		/// </summary>
		void Close();

		/// <summary>
		/// Event raised when a diagram editor indicates it will be closing.
		/// </summary>
		event CancelEventHandler Closing;

		/// <summary>
		/// Event raised when a diagram editor has been closed.
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// The code editor.
		/// </summary>
		ICodeEditor CodeEditor { get; }
	}
}