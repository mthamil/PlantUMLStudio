using System.Collections.Generic;
using PlantUmlEditor.Model.Snippets;
using PlantUmlEditor.Properties;

namespace PlantUmlEditor.ViewModel
{
	public class SnippetsMenu : MenuViewModel
	{
		public SnippetsMenu(IEnumerable<CodeSnippet> snippets)
		{
			foreach (var snippet in BuildTree(snippets))
				SubMenu.Add(snippet);
			Name = Resources.ContextMenu_Code_Snippets;
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
	}
}