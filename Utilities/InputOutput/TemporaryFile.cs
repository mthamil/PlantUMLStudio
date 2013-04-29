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

using System.IO;

namespace Utilities.InputOutput
{
	/// <summary>
	/// Class that manages a temporary file and aids in clean up
	/// after its use.
	/// </summary>
	public class TemporaryFile : DisposableBase
	{
		/// <summary>
		/// Initializes a new <see cref="TemporaryFile"/>.
		/// </summary>
		public TemporaryFile()
		{
			File = new FileInfo(Path.GetTempFileName());
		}

		/// <summary>
		/// Creates an empty temporary file for the file path represented by this <see cref="TemporaryFile"/>.
		/// </summary>
		/// <remarks>This object is returned to enable a more fluent syntax.</remarks>
		public TemporaryFile Touch()
		{
			File.Create().Close();
			return this;
		}

		/// <summary>
		/// Populates a temporary file with the given content.
		/// </summary>
		/// <param name="content">The content to write to the temporary file</param>
		/// <remarks>This object is returned to enable a more fluent syntax.</remarks>
		public TemporaryFile WithContent(string content)
		{
			using (var writer = File.CreateText())
				writer.Write(content);

			return this;
		}

		/// <summary>
		/// The actual temporary file.
		/// </summary>
		public FileInfo File { get; private set; }

		/// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing() { }

		/// <see cref="DisposableBase.OnDispose"/>
		protected override void OnDispose()
		{
			if (File.Exists)
				File.Delete();
		}
	}
}