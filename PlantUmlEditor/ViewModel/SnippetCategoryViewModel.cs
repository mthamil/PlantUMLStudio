using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PlantUmlEditor.Model.Snippets;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a snippet category.
	/// </summary>
	public class SnippetCategoryViewModel : ViewModelBase
	{
		public SnippetCategoryViewModel(string name)
		{
			Name = name;
			Snippets = new ObservableCollection<SnippetCategoryViewModel>();
		}

		/// <summary>
		/// The name of the category/snippet.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The snippets/sub-categories in the category.
		/// </summary>
		public ICollection<SnippetCategoryViewModel> Snippets { get; private set; }

		/// <see cref="object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			var other = obj as SnippetCategoryViewModel;
			if (other == null)
				return false;

			return Equals(Name, other.Name);
		}

		/// <see cref="object.GetHashCode"/>
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		/// <summary>
		/// Constructs a hierarchical tree from a collection of snippets.
		/// </summary>
		/// <param name="snippets">The snippets to build the tree with</param>
		/// <returns>A tree of snippets</returns>
		public static IEnumerable<SnippetCategoryViewModel> BuildTree(IEnumerable<Snippet> snippets)
		{
			var categories = new SortedDictionary<string, SnippetCategoryViewModel>();
			foreach (var snippet in snippets)
			{
				SnippetCategoryViewModel category;
				if (!categories.TryGetValue(snippet.Category, out category))
				{
					category = new SnippetCategoryViewModel(snippet.Category);
					categories[category.Name] = category;
				}

				category.Snippets.Add(new SnippetViewModel(snippet));
			}
			return categories.Values;
		}
	}

	/// <summary>
	/// Represents a code snippet.
	/// </summary>
	public class SnippetViewModel : SnippetCategoryViewModel
	{
		public SnippetViewModel(Snippet snippet)
			: base(snippet.Name)
		{
			_snippet = snippet;
			SelectCommand = new RelayCommand<CodeEditorViewModel>(Select);
		}

		/// <summary>
		/// The code snippet.
		/// </summary>
		public string Code { get { return _snippet.Code; } }

		/// <summary>
		/// The command to execute to select a snippet for use.
		/// </summary>
		public ICommand SelectCommand { get; private set; }

		private void Select(CodeEditorViewModel editor)
		{
			var formattedCode = Code.Replace("\\r", Environment.NewLine);
			editor.Content = editor.Content.Insert(editor.ContentIndex, formattedCode);
		}

		private readonly Snippet _snippet;
	}
}