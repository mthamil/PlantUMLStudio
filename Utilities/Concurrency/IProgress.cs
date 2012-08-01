using System;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Defines a provider for progress updates.
	/// </summary>
	/// <typeparam name="T">The type of progress update value</typeparam>
	/// <remarks>This class is a copy of the .NET v4.5 IProgress interface and becomes obsolete on that framework</remarks>
	public interface IProgress<in T>
	{
		/// <summary>
		/// Reports a progress update.
		/// </summary>
		/// <param name="value">The value of the updated progress</param>
		void Report(T value);
	}

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
			child.ProgressChanged += (o, e) => parent.Report(mapper(e.Value));
		}
	}
}