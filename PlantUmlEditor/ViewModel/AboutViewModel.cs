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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using PlantUmlEditor.Core.Dependencies;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents information about the application.
	/// </summary>
	public class AboutViewModel : ViewModelBase
	{
		public AboutViewModel(IEnumerable<IExternalComponent> components)
		{
			_components = Property.New(this, p => p.Components, OnPropertyChanged);
			Components = new ObservableCollection<ComponentViewModel>(components.Select(d => new ComponentViewModel(d)));
		}

		public void LoadComponents()
		{
			foreach (var component in Components)
				component.LoadAsync();
		}

		/// <summary>
		/// The application version.
		/// </summary>
		public Version ApplicationVersion
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}

		/// <summary>
		/// All components the application depends on.
		/// </summary>
		public ICollection<ComponentViewModel> Components
		{
			get { return _components.Value; }
			private set { _components.Value = value; }
		}

		private readonly Property<ICollection<ComponentViewModel>> _components;
	}
}