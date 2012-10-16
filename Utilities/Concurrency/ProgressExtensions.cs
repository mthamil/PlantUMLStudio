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

namespace Utilities.Concurrency
{
	/// <summary>
	/// Contains utility methods for IProgress.
	/// </summary>
	public static class ProgressExtensions
	{
		/// <summary>
		/// Maps one Progress object to another.
		/// </summary>
		/// <typeparam name="TParent">The first type of progress update</typeparam>
		/// <typeparam name="TChild">The second type of progress update</typeparam>
		/// <param name="parent">The parent progress</param>
		/// <param name="child">The child progress being wrapped</param>
		/// <param name="mapper">The progress update mapping function</param>
		public static void Wrap<TParent, TChild>(this IProgress<TParent> parent, Progress<TChild> child, Func<TChild, TParent> mapper)
		{
			child.ProgressChanged += (o, e) => parent.Report(mapper(e));
		}
	}
}