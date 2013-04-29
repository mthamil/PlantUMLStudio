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

namespace Utilities
{
	/// <summary>
	/// Contains extensions methods for IDisposable.
	/// </summary>
	public static class DisposableExtensions
	{
		/// <summary>
		/// Disposes of a resource in a null-safe way.
		/// </summary>
		/// <param name="disposable">A disposable resource</param>
		public static void DisposeSafely(this IDisposable disposable)
		{
			if (disposable != null)
				disposable.Dispose();
		}
	}
}