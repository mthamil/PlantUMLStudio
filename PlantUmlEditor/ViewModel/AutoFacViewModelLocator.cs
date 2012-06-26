using System;
using System.ComponentModel;
using System.Windows;
using Autofac;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Factory for locator classes used for accessing specific view models. 
	/// </summary>
	/// <typeparam name="TViewModel">The specific type of ViewModel to access</typeparam>
	public class AutofacViewModelLocator<TViewModel> : INotifyPropertyChanged where TViewModel : class
	{
		public AutofacViewModelLocator(Autofac.IContainer container, string name)
		{
			_name = name;
			_container = container;
		}

		/// <summary>
		/// Gets a value indicating whether the control is in design mode
		/// (running in Blend or Visual Studio).
		/// </summary>
		public static bool IsInDesignMode
		{
			get
			{
				if (!_isInDesignMode.HasValue)
				{
					_isInDesignMode =
						((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue));
				}

				return _isInDesignMode.Value;
			}
		}

		/// <summary>
		/// Holds the intance of the runtime version of the ViewModel that is instantiated only when application is really running by retrieving the instance from IOC container
		/// </summary>
		protected TViewModel RuntimeViewModel
		{
			get
			{
				if (_runtimeViewModel == null)
				{
					RuntimeViewModel = String.IsNullOrEmpty(_name) 
						? _container.Resolve<TViewModel>() 
						: _container.ResolveNamed<TViewModel>(_name);
				}
				return _runtimeViewModel;
			}

			set
			{
				_runtimeViewModel = value;
				OnPropertyChanged("ViewModel");
			}
		}

		/// <summary>
		/// Gets current ViewModel instance so if we are in designer its <see cref="DesigntimeViewModel"/> and if its runtime then its <see cref="RuntimeViewModel"/>.
		/// </summary>
		public TViewModel ViewModel
		{
			get
			{
				return IsInDesignMode ? DesigntimeViewModel : RuntimeViewModel;
			}
		}

		/// <summary>
		/// Holds the intance of the designtime version of the ViewModel that is instantiated only when application is opened in IDE designer (VisualStudio, Blend etc).
		/// </summary>
		public TViewModel DesigntimeViewModel
		{
			get
			{
				return _designtimeViewModel;
			}

			set
			{
				_designtimeViewModel = value;
				OnPropertyChanged("ViewModel");
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private static bool? _isInDesignMode;
		private TViewModel _runtimeViewModel;
		private TViewModel _designtimeViewModel;
		private readonly string _name;
		private readonly Autofac.IContainer _container;
	}
}