using System;
using System.Windows.Markup;
using PlantUmlEditor.ViewModel;
using Utilities.Controls.Markup;

namespace PlantUmlEditor.View.MarkupExtensions
{
	/// <summary>
	/// A XAML markup extension to allow definition of a generic view-model locator.
	/// </summary>
	public class ViewModelLocator : ActivatorExtension
	{
		#region Overrides of MarkupExtension

		/// <see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			Type = viewModelLocatorType;
			TypeArguments.Add(TypeArgument);
			ConstructorArguments.Add(App.Container);
			ConstructorArguments.Add(Name);
			return base.ProvideValue(serviceProvider);
		}

		#endregion

		/// <summary>
		/// The type of the view-model.
		/// </summary>
		public Type TypeArgument { get; set; }

		/// <summary>
		/// The object config name of a container-defined view-model.
		/// </summary>
		public string Name { get; set; }

		private static readonly Type viewModelLocatorType = typeof(AutofacViewModelLocator<>);
	}
}