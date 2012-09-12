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

			foreach (var component in Components)
				component.AnalyzeAsync();
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