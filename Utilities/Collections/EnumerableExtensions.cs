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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Utilities.Collections
{
	/// <summary>
	/// Contains extension methods pertaining to enumerables.
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Determines whether an enumerable contains all items of another enumerable.
		/// This does not test for proper subsets, so if the compared enumerables 
		/// contain exactly the same items, true is returned.
		/// </summary>
		/// <param name="subset">The supposed subset of items</param>
		/// <param name="superset">The supposed superset of items</param>
		/// <returns>Whether superset contains all elements of the enumerable</returns>
		public static bool IsSubsetOf<T>(this IEnumerable<T> subset, IEnumerable<T> superset)
		{
			// Optimize for ISet which already contains a method to perform
			// this operation.
			var subsetSet = subset as ISet<T>;
			if (subsetSet != null)
				return subsetSet.IsSubsetOf(superset);

			// If subtracting the superset from the subset does not remove all items, then
			// the superset does not actually contain everything.
			IEnumerable<T> subtractedItems = subset.Except(superset);
			if (subtractedItems.Any())
				return false;

			return true;
		}

		/// <summary>
		/// Returns an enumerable of items grouped into sliceSize number of items.
		/// If the number of items remaining is less than the slice size, the last slice
		/// will be the size of just the remaining items.
		/// </summary>
		/// <param name="items">The items to slice</param>
		/// <param name="sliceSize">The number of items per slice</param>
		/// <returns>An enumerable of items grouped into slizeSize number of items</returns>
		public static IEnumerable<IEnumerable<T>> Slices<T>(this IEnumerable<T> items, int sliceSize)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			if (sliceSize < 1)
				return Enumerable.Empty<IEnumerable<T>>();

			return new SliceEnumerable<T>(items, sliceSize);
		}

		/// <summary>
		/// Generates an IEnumerable from an IEnumerator.
		/// </summary>
		/// <param name="enumerator">The enumerator to convert</param>
		/// <returns>An enumerable for the enumerator</returns>
		/// <remarks>Borrowed from Igor Ostrovsky: 
		/// http://igoro.com/archive/extended-linq-additional-operators-for-linq-to-objects/
		/// </remarks>
		[DebuggerStepThrough]
		public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
		{
			if (enumerator == null)
				throw new ArgumentNullException("enumerator");

			return enumerator.ToEnumerableImpl();
		}

		/// <summary>
		/// Implementation method that allows for eager argument validation.
		/// </summary>
		private static IEnumerable<T> ToEnumerableImpl<T>(this IEnumerator<T> enumerator)
		{
			while (enumerator.MoveNext())
				yield return enumerator.Current;
		}

		/// <summary>
		/// Generates an IEnumerable from a single value.
		/// </summary>
		/// <param name="value">The value to create an enumerable for</param>
		/// <returns>An enumerable for the single value</returns>
		[DebuggerStepThrough]
		public static IEnumerable<T> ToEnumerable<T>(this T value)
		{
			yield return value;
		}

		/// <summary>
		/// Returns the item from an enumerable that has the maximum value according
		/// to a key.  The default comparer is used.
		/// </summary>
		/// <typeparam name="TSource">The type of items in the source enumerable</typeparam>
		/// <typeparam name="TKey">The type of value being compared</typeparam>
		/// <param name="source">The source enumerable</param>
		/// <param name="selector">Determines the value from each item to use for comparison</param>
		/// <returns>The item from the source that has the maximum value according to its key</returns>
		/// <exception cref="ArgumentNullException">If source or selector are null</exception>
		/// <exception cref="InvalidOperationException">If the source sequence contains no elements</exception>
		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			return source.MaxBy(selector, Comparer<TKey>.Default);
		}

		/// <summary>
		/// Returns the item from an enumerable that has the maximum value according
		/// to a key.
		/// </summary>
		/// <typeparam name="TSource">The type of items in the source enumerable</typeparam>
		/// <typeparam name="TKey">The type of value being compared</typeparam>
		/// <param name="source">The source enumerable</param>
		/// <param name="selector">Determines the value from each item to use for comparison</param>
		/// <param name="comparer">The comparer to use on keys</param>
		/// <returns>The item from the source that has the maximum value according to its key</returns>
		/// <exception cref="ArgumentNullException">If source or selector are null</exception>
		/// <exception cref="InvalidOperationException">If the source sequence contains no elements</exception>
		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			IComparer<TKey> actualComparer = comparer ?? Comparer<TKey>.Default;
			return source.ExtremumBy(selector, actualComparer);
		}

		/// <summary>
		/// Returns the item from an enumerable that has the minimum value according
		/// to a key.  The default comparer is used.
		/// </summary>
		/// <typeparam name="TSource">The type of items in the source enumerable</typeparam>
		/// <typeparam name="TKey">The type of value being compared</typeparam>
		/// <param name="source">The source enumerable</param>
		/// <param name="selector">Determines the value from each item to use for comparison</param>
		/// <returns>The item from the source that has the minimum value according to its key</returns>
		/// <exception cref="ArgumentNullException">If source or selector are null</exception>
		/// <exception cref="InvalidOperationException">If the source sequence contains no elements</exception>
		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			return source.MinBy(selector, Comparer<TKey>.Default);
		}

		/// <summary>
		/// Returns the item from an enumerable that has the minimum value according
		/// to a key.
		/// </summary>
		/// <typeparam name="TSource">The type of items in the source enumerable</typeparam>
		/// <typeparam name="TKey">The type of value being compared</typeparam>
		/// <param name="source">The source enumerable</param>
		/// <param name="selector">Determines the value from each item to use for comparison</param>
		/// <param name="comparer">The comparer to use on keys</param>
		/// <returns>The item from the source that has the minimum value according to its key</returns>
		/// <exception cref="ArgumentNullException">If source or selector are null</exception>
		/// <exception cref="InvalidOperationException">If the source sequence contains no elements</exception>
		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			IComparer<TKey> actualComparer = comparer ?? Comparer<TKey>.Default;
			return source.ExtremumBy(selector, new ReverseComparer<TKey>(actualComparer));
		}

		private static TSource ExtremumBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (selector == null)
				throw new ArgumentNullException("selector");

			TSource extremum = source.Aggregate((currentExtremum, element) =>
			{
				TKey extremumKey = selector(currentExtremum);
				TKey elementKey = selector(element);

				int result = comparer.Compare(elementKey, extremumKey);
				return result > 0 ? element : currentExtremum;
			});

			return extremum;
		}

		/// <summary>
		/// Allows await-ing on an enumerable of Tasks.
		/// </summary>
		/// <param name="tasks">The tasks to await</param>
		/// <returns>An awaiter for the completion of the tasks</returns>
		public static TaskAwaiter GetAwaiter(this IEnumerable<Task> tasks)
		{
			return Task.WhenAll(tasks).GetAwaiter();
		}

		/// <summary>
		/// Allows await-ing on an enumerable of Task&lt;T&gt;s.
		/// </summary>
		/// <param name="tasks">The tasks to await</param>
		/// <returns>An awaiter for the completion of the tasks</returns>
		public static TaskAwaiter<T[]> GetAwaiter<T>(this IEnumerable<Task<T>> tasks)
		{
			return Task.WhenAll(tasks).GetAwaiter();
		}

		/// <summary>
		/// Lazily pipes the output of an enumerable to an action.
		/// </summary>
		/// <typeparam name="T">The type of items</typeparam>
		/// <param name="items">The items to pipe</param>
		/// <param name="action">The action to apply</param>
		/// <returns>The source enumerable</returns>
		public static IEnumerable<T> Tee<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items)
			{
				action(item);
				yield return item;
			}
		}

		/// <summary>
		/// Iterates over an enumerable and adds each item to an existing collection.
		/// </summary>
		/// <remarks>
		/// This method may serve as an alternative to <see cref="Enumerable.ToList{TSource}"/> when
		/// an existing collection must be used instead of creating a new list.
		/// </remarks>
		/// <typeparam name="T">The type of items</typeparam>
		/// <param name="source">The items to iterate over</param>
		/// <param name="destination">An existing collection to add items to</param>
		public static void AddTo<T>(this IEnumerable<T> source, ICollection<T> destination)
		{
			foreach (var item in source)
				destination.Add(item);
		}

		/// <summary>
		/// Private class that provides the Slices enumerator.
		/// </summary>
		private class SliceEnumerable<T> : IEnumerable<IEnumerable<T>>
		{
			private readonly IEnumerable<T> items;
			private readonly int sliceSize;

			/// <summary>
			/// Creates a new Slice enumerable.
			/// </summary>
			public SliceEnumerable(IEnumerable<T> items, int sliceSize)
			{
				this.items = items;
				this.sliceSize = sliceSize;
			}

			#region IEnumerable Members

			/// <see cref="System.Collections.IEnumerable.GetEnumerator"/>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

			#region IEnumerable<IEnumerable<T>> Members

			/// <see cref="System.Collections.Generic.IEnumerable{T}.GetEnumerator"/>
			public IEnumerator<IEnumerable<T>> GetEnumerator()
			{
				IList<T> group = new List<T>();
				int itemCounter = 1;
				foreach (T item in items)
				{
					group.Add(item);
					if (itemCounter % sliceSize == 0)
					{
						yield return new List<T>(group);
						group.Clear();
					}

					itemCounter++;
				}

				if (group.Count > 0)
					yield return new List<T>(group);
			}

			#endregion
		}
	}
}