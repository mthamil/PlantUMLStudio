using System.Collections;
using System.Collections.Generic;
using PlantUmlEditor.Properties;
using PlantUmlEditor.ViewModel.Commands;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Contains operations available on a diagram editor.
	/// </summary>
	public class EditorContextMenu : IEnumerable<MenuViewModel>
	{
		public EditorContextMenu()
		{
			_items = new List<MenuViewModel>
			{
				new MenuViewModel
				{
					Name = Resources.ContextMenu_Code_Cut,
					Command = new CutCommand()
				},

				new MenuViewModel
				{
					Name = Resources.ContextMenu_Code_Copy,
					Command = new CopyCommand()
				},

				new MenuViewModel
				{
					Name = Resources.ContextMenu_Code_Paste,
					Command = new PasteCommand()
				}
			};
		}

		#region Implementation of IEnumerable

		/// <see cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<MenuViewModel> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		/// <see cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private readonly IEnumerable<MenuViewModel> _items;
	}
}