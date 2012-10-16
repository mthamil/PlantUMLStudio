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
using System.Collections.Generic;

namespace Utilities.Collections
{
	/// <summary>
	/// A wrapper around a comparer that reverses the comparison results.
	/// </summary>
	public class ReverseComparer<T> : IComparer<T>
	{
		private readonly IComparer<T> baseComparer;

		/// <summary>
		/// Creates a new ReverseComparer for the given IComparer.
		/// </summary>
		/// <param name="baseComparer">The original comparer</param>
		public ReverseComparer(IComparer<T> baseComparer)
		{
			if (baseComparer == null)
				throw new ArgumentNullException("baseComparer");

			this.baseComparer = baseComparer;
		}

		/// <see cref="IComparer{T}.Compare" />
		public int Compare(T x, T y)
		{
			return baseComparer.Compare(y, x);
		}
	}
}