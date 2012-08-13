using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	public class MenuViewModel : ViewModelBase
	{
		public MenuViewModel()
		{
			_name = Property.New(this, p => p.Name, OnPropertyChanged);

			SubMenu = new ObservableCollection<MenuViewModel>();
		}

		public string Name 
		{
			get { return _name.Value; }
			set { _name.Value = value; }
		}

		public ICommand Command { get; set; }

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