using System;
using System.Windows.Markup;
using PlantUmlEditor.ViewModel;
using Utilities.Mvvm;

namespace PlantUmlEditor.View.MarkupExtensions
{
	/// <summary>
	/// A XAML markup extension to allow definition of a generic view-model locator.
	/// </summary>
	public class ViewModelLocator : MarkupExtension
	{
		public ViewModelLocator(Type typeArgument, string name)
		{
			TypeArgument = typeArgument;
			Name = name;
		}

		public ViewModelLocator() : this(typeof(ViewModelBase), "") { }

		#region Overrides of MarkupExtension

		/// <see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Locator;
		}

		#endregion

		/// <summary>
		/// The actual value returned from ProvideValue.
		/// </summary>
		public object Locator
		{
			get
			{
				if (_viewModelLocator == null)
					_viewModelLocator = CreateViewModelLocator();

				return _viewModelLocator;
			}
		}

		/// <summary>
		/// The type of the view-model.
		/// </summary>
		public Type TypeArgument { get; set; }

		/// <summary>
		/// The object config name of a Spring-defined view-model.
		/// </summary>
		public string Name { get; set; }

		private object CreateViewModelLocator()
		{
			return Activator.CreateInstance(ConstructViewModelLocatorType(), App.Container, Name);
		}

		private Type ConstructViewModelLocatorType()
		{
			return viewModelLocatorType.MakeGenericType(TypeArgument);
		}

		private object _viewModelLocator;
		private static readonly Type viewModelLocatorType = typeof(AutofacViewModelLocator<>);
	}
}