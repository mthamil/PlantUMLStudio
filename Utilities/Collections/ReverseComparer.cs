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