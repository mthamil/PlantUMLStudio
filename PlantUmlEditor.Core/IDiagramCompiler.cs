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

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using PlantUmlEditor.Core.Imaging;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Interface for a service that compiles diagrams to images.
	/// </summary>
	public interface IDiagramCompiler
	{
		/// <summary>
		/// Creates an in-memory bitmap image from diagram code.
		/// No external files are created.
		/// </summary>
		/// <param name="diagramCode">The diagram code to compile</param>
		/// <param name="imageFormat">The desired image format</param>
		/// <param name="cancellationToken">An optional cancellation token</param>
		/// <returns>A Task representing the compilation operation</returns>
		Task<ImageSource> CompileToImageAsync(string diagramCode, ImageFormat imageFormat, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Reads the code from a diagram file, compiles it to an image, and saves the output to a file.
		/// </summary>
		/// <param name="diagramFile">The diagram file to compile</param>
		/// <param name="imageFormat">The desired image format</param>
		/// <returns>A Task representing the compilation operation</returns>
		Task CompileToFileAsync(FileInfo diagramFile, ImageFormat imageFormat);
	}
}