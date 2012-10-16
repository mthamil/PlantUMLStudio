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
using System.Collections;
using System.Collections.Generic;
using PlantUmlEditor.Model.Snippets;

namespace PlantUmlEditor.ViewModel
{
	public class SnippetsMenu : IEnumerable<MenuViewModel>
	{
		public SnippetsMenu(IEnumerable<CodeSnippet> snippets)
		{
			_snippets = BuildTree(snippets);
		}

		/// <summary>
		/// Constructs a hierarchical tree from a collection of snippets.
		/// </summary>
		/// <param name="snippets">The snippets to build the tree with</param>
		/// <returns>A tree of snippets</returns>
		private static IEnumerable<MenuViewModel> BuildTree(IEnumerable<CodeSnippet> snippets)
		{
			var categories = new SortedDictionary<string, MenuViewModel>();
			foreach (var snippet in snippets)
			{
				MenuViewModel category;
				if (!categories.TryGetValue(snippet.Category, out category))
				{
					category = new MenuViewModel { Name = snippet.Category };
					categories[category.Name] = category;
				}

				category.SubMenu.Add(new SnippetViewModel(snippet));
			}
			return categories.Values;
		}

		#region Implementation of IEnumerable

		/// <see cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<MenuViewModel> GetEnumerator()
		{
			return _snippets.GetEnumerator();
		}

		/// <see cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private readonly IEnumerable<MenuViewModel> _snippets;
	}
}