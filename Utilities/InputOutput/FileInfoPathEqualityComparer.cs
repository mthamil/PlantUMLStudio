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