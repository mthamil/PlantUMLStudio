using System;
using System.ComponentModel;
using System.Windows;
using Autofac;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Used to retrieve specific view models from an application container. 
	/// </summary>
	/// <typeparam name="TViewModel">The specific type of ViewModel to access</typeparam>
	public class AutofacViewModelLocator<TViewModel> : INotifyPropertyChanged where TViewModel : class
	{
		public AutofacViewModelLocator(Autofac.IContainer container, string name)
		{
			_name = name;
			_container = container;

			_runtimeViewModel = Property.New(this, p => p.RuntimeViewModel, OnPropertyChanged)
				.AlsoChanges(p => p.ViewModel);

			_designtimeViewModel = Property.New(this, p => p.DesigntimeViewModel, OnPropertyChanged)
				.AlsoChanges(p => p.ViewModel);
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
				if (_runtimeViewModel.Value == null)
				{
					RuntimeViewModel = String.IsNullOrEmpty(_name) 
						? _container.Resolve<TViewModel>() 
						: _container.ResolveNamed<TViewModel>(_name);
				}
				return _runtimeViewModel.Value;
			}

			set
			{
				_runtimeViewModel.Value = value;
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
			get { return _designtimeViewModel.Value; }
			set { _designtimeViewModel.Value = value; }
		}

		#region INotifyPropertyChanged Members

		/// <see cref="INotifyPropertyChanged.PropertyChanged"/>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			var localEvent = PropertyChanged;
			if (localEvent != null)
				localEvent(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private static bool? _isInDesignMode;
		private readonly Property<TViewModel> _runtimeViewModel;
		private readonly Property<TViewModel> _designtimeViewModel;
		private readonly string _name;
		private readonly Autofac.IContainer _container;
	}
}