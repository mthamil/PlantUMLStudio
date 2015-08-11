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
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Represents a menu item.
	/// </summary>
	public class MenuViewModel : ViewModelBase
	{
		public MenuViewModel(string name)
		{
			_name = Property.New(this, p => p.Name, OnPropertyChanged);
		    Name = name;

			SubMenu = new ObservableCollection<MenuViewModel>();
		}

		/// <summary>
		/// A menu item's name.
		/// </summary>
		public string Name 
		{
			get { return _name.Value; }
			private set { _name.Value = value; }
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
		public ICollection<MenuViewModel> SubMenu { get; }

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