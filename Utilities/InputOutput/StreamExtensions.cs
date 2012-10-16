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
using System.Collections.Generic;
using System.IO;

namespace Utilities.InputOutput
{
	/// <summary>
	/// Contains extensions for the Stream class.
	/// </summary>
	public static class StreamExtensions
	{
		/// <summary>
		/// Returns a stream as a sequence of lines.
		/// </summary>
		/// <param name="stream">The stream to iterate over</param>
		/// <returns>A sequence of lines from the stream</returns>
		public static IEnumerable<string> Lines(this Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
					yield return reader.ReadLine();
			}
		}
	}
}