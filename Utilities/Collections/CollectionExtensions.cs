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

using System.Collections.Generic;

namespace Utilities.Collections
{
	/// <summary>
	/// Contains extension methods pertaining to collections.
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		/// Adds the elements of the specified sequence to a collection.
		/// </summary>
		/// <typeparam name="T">The type of items in the collection</typeparam>
		/// <param name="collection">The collection to add to</param>
		/// <param name="newElements">The items to add to the collection</param>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newElements)
		{
			foreach (var element in newElements)
				collection.Add(element);
		}

		/// <summary>
		/// Adds the given elements to a collection.
		/// </summary>
		/// <typeparam name="T">The type of items in the collection</typeparam>
		/// <param name="collection">The collection to add to</param>
		/// <param name="firstElement">The first item to add</param>
		/// <param name="secondElement">The second item to add</param>
		/// <param name="remainingElements">The rest of the items to add to the collection</param>
		public static void AddRange<T>(this ICollection<T> collection, T firstElement, T secondElement, params T[] remainingElements)
		{
			collection.Add(firstElement);
			collection.Add(secondElement);

			foreach (var element in remainingElements)
				collection.Add(element);
		}
	}
}