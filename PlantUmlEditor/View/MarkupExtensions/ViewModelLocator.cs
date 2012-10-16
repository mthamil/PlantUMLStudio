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
using System.Windows.Markup;
using PlantUmlEditor.ViewModel;
using Utilities.Controls.Markup;

namespace PlantUmlEditor.View.MarkupExtensions
{
	/// <summary>
	/// A XAML markup extension to allow definition of a generic view-model locator.
	/// </summary>
	[MarkupExtensionReturnType(typeof(object))]
	public class ViewModelLocator : ActivatorExtension
	{
		/// <summary>
		/// Default constructor required by XAML.
		/// </summary>
		public ViewModelLocator()
			: base(viewModelLocatorType) { }

		/// <summary>
		/// Default constructor required by XAML.
		/// </summary>
		public ViewModelLocator(Type viewModelType)
			: this()
		{
			TypeArgument = viewModelType;
		}

		/// <summary>
		/// The type of the view-model.
		/// </summary>
		public Type TypeArgument { get; set; }

		/// <summary>
		/// The object config name of a container-defined view-model.
		/// </summary>
		public string Name { get; set; }

		#region Overrides of MarkupExtension

		/// <see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			TypeArguments.Add(TypeArgument);
			ConstructorArguments.Add(App.Container);
			ConstructorArguments.Add(Name);
			return base.ProvideValue(serviceProvider);
		}

		#endregion

		private static readonly Type viewModelLocatorType = typeof(AutofacViewModelLocator<>);
	}
}