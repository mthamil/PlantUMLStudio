using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PlantUmlEditor.Model;
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
			Snippets = new ObservableCollection<SnippetViewModel>();
		}

		/// <summary>
		/// The name of the category.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The snippets in the category.
		/// </summary>
		public ICollection<SnippetViewModel> Snippets { get; private set; }

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
	}

	/// <summary>
	/// Represents a code snippet.
	/// </summary>
	public class SnippetViewModel : ViewModelBase
	{
		public SnippetViewModel(Snippet snippet)
		{
			_snippet = snippet;
			SelectCommand = new RelayCommand(editor => 
				Select((DiagramEditorViewModel)editor));
		}

		/// <summary>
		/// The name of the snippet.
		/// </summary>
		public string Name { get { return _snippet.Name; } }

		/// <summary>
		/// The code snippet.
		/// </summary>
		public string Code { get { return _snippet.Code; } }

		/// <summary>
		/// The command to execute to select a snippet for use.
		/// </summary>
		public ICommand SelectCommand { get; private set; }

		private void Select(DiagramEditorViewModel editor)
		{
			var formattedCode = Code.Replace("\\r", Environment.NewLine);
			editor.Content = editor.Content.Insert(editor.ContentIndex, formattedCode);
		}

		private readonly Snippet _snippet;
	}
}