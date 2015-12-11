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

using System;
using System.ComponentModel;
using System.Windows;
using Autofac;
using SharpEssentials.Observable;

namespace PlantUmlStudio.ViewModel
{
	/// <summary>
	/// Used to retrieve specific view models from an application container. 
	/// </summary>
	/// <typeparam name="TViewModel">The specific type of ViewModel to access</typeparam>
	public class AutofacViewModelLocator<TViewModel> : ObservableObject where TViewModel : class
	{
		public AutofacViewModelLocator(Autofac.IContainer container, string name)
		{
			_name = name;
			_container = container;

		    _runTimeViewModel = Property.New(this, p => p.RuntimeViewModel)
		                                .AlsoChanges(p => p.ViewModel);

		    _designTimeViewModel = Property.New(this, p => p.DesigntimeViewModel)
		                                   .AlsoChanges(p => p.ViewModel);
		}

		/// <summary>
		/// Gets a value indicating whether the control is in design mode
		/// (running in Blend or Visual Studio).
		/// </summary>
		public static bool IsInDesignMode => _isInDesignMode.Value;

	    /// <summary>
		/// Holds the intance of the runtime version of the ViewModel that is instantiated only when application is really running by retrieving the instance from IOC container
		/// </summary>
		protected TViewModel RuntimeViewModel
		{
			get
			{
				if (_runTimeViewModel.Value == null)
				{
					RuntimeViewModel = String.IsNullOrEmpty(_name) 
						? _container.Resolve<TViewModel>() 
						: _container.ResolveNamed<TViewModel>(_name);
				}
				return _runTimeViewModel.Value;
			}

			set
			{
				_runTimeViewModel.Value = value;
			}
		}

		/// <summary>
		/// Gets current ViewModel instance so if we are in designer its <see cref="DesigntimeViewModel"/> and if its runtime then its <see cref="RuntimeViewModel"/>.
		/// </summary>
		public TViewModel ViewModel => IsInDesignMode ? DesigntimeViewModel : RuntimeViewModel;

	    /// <summary>
		/// Holds the instance of the design-time version of the ViewModel that is instantiated only when application is opened in IDE designer (VisualStudio, Blend etc).
		/// </summary>
		public TViewModel DesigntimeViewModel
		{
			get { return _designTimeViewModel.Value; }
			set { _designTimeViewModel.Value = value; }
		}

		private static readonly Lazy<bool> _isInDesignMode = new Lazy<bool>(() => (bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue));
		private readonly Property<TViewModel> _runTimeViewModel;
		private readonly Property<TViewModel> _designTimeViewModel;
		private readonly string _name;
		private readonly Autofac.IContainer _container;
	}
}