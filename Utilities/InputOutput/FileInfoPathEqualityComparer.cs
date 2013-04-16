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

using System;
using System.Collections.Generic;
using System.IO;

namespace Utilities.InputOutput
{
	/// <summary>
	/// An equality comparer that checks whether two files have the same path and filename.
	/// </summary>
	public class FileInfoPathEqualityComparer : IEqualityComparer<FileInfo>
	{
		/// <summary>
		/// Determines whether two files have the same path and filename.
		/// </summary>
		/// <param name="x">The first file to compare</param>
		/// <param name="y">The second file to compare</param>
		/// <returns>Whether the files are considered equal</returns>
		public bool Equals(FileInfo x, FileInfo y)
		{
			if (x == y)
				return true;

			if (x == null || y == null)
				return false;

			return String.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns a file's hashcode.
		/// </summary>
		public int GetHashCode(FileInfo obj)
		{
			return obj.FullName.ToLowerInvariant().GetHashCode();
		}

		/// <summary>
		/// Gets a FileInfo equality comparer.
		/// </summary>
		public static IEqualityComparer<FileInfo> Instance
		{
			get { return instance; }
		}

		private static readonly IEqualityComparer<FileInfo> instance = new FileInfoPathEqualityComparer(); 
	}
}