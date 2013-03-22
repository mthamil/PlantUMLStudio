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
using System.Windows;
using System.Windows.Media;

namespace Utilities.Controls
{
	/// <summary>
	/// Provides methods that help navigate visual trees.
	/// </summary>
	public static class VisualTreeExtensions
	{
		/// <summary>
		/// Returns an enumerable over a <see cref="DependencyObject"/>'s visual children.
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <returns>An enumerable over immediate child visual tree elements</returns>
		public static IEnumerable<DependencyObject> VisualChildren(this DependencyObject parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			return parent.EnumerateVisualTreeChildren();
		}

		private static IEnumerable<DependencyObject> EnumerateVisualTreeChildren(this DependencyObject parent)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
				yield return VisualTreeHelper.GetChild(parent, i);
		}
	}
}