using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a menu item.
	/// </summary>
	public class MenuViewModel : ViewModelBase
	{
		public MenuViewModel()
		{
			_name = Property.New(this, p => p.Name, OnPropertyChanged);

			SubMenu = new ObservableCollection<MenuViewModel>();
		}

		/// <summary>
		/// A menu item's name.
		/// </summary>
		public string Name 
		{
			get { return _name.Value; }
			set { _name.Value = value; }
		}

		/// <summary>
		/// A menu item's icon.
		/// </summary>
		public ImageSource Icon { get; set; }

		/// <summary>
		/// A menu item's command.
		/// </summary>
		public ICommand Command { get; set; }

		/// <summary>
		/// Any menu children.
		/// </summary>
		public ICollection<MenuViewModel> SubMenu { get; private set; }

		/// <see cref="object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			var other = obj as MenuViewModel;
			if (other == null)
				return false;

			return Equals(Name, other.Name);
		}

		/// <see cref="object.GetHashCode"/>
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		private readonly Property<string> _name;
	}
}