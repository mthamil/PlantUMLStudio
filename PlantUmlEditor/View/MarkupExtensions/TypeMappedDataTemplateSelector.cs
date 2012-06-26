using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PlantUmlEditor.View.MarkupExtensions
{
	/// <summary>
	/// A markup extension that allows definition of a data template selector that is composed of multiple individual
	/// data templates and is keyed off of the templates' data types.
	/// </summary>
	public class TypeMappedDataTemplateSelector : MarkupExtension
	{
		/// <summary>
		/// Initializes a new selector.
		/// </summary>
		public TypeMappedDataTemplateSelector()
		{
			_selector = new Lazy<DataTemplateSelector>(() => new DataTemplateTypeMapSelector(DataTemplates));
		}

		#region Overrides of MarkupExtension

		/// <see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _selector.Value;
		}

		#endregion

		/// <summary>
		/// The data templates to select from.
		/// </summary>
		public Collection<DataTemplate> DataTemplates
		{
			get { return new Collection<DataTemplate>(_dataTemplates); }
		}

		private readonly IList<DataTemplate> _dataTemplates = new List<DataTemplate>();
		private readonly Lazy<DataTemplateSelector> _selector;
	}

	/// <summary>
	/// A data template selector that is composed of multiple individual data templates and is keyed off of
	/// the templates' data types.
	/// </summary>
	public class DataTemplateTypeMapSelector : DataTemplateSelector
	{
		/// <summary>
		/// Initializes a new data template selector.
		/// </summary>
		/// <param name="dataTemplates">The data templates to select from</param>
		public DataTemplateTypeMapSelector(IEnumerable<DataTemplate> dataTemplates)
		{
			_dataTemplateMap = dataTemplates.ToDictionary(k => (Type)k.DataType, v => v);
		}

		/// <see cref="DataTemplateSelector.SelectTemplate"/>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return _dataTemplateMap[item.GetType()];
		}

		private readonly IDictionary<Type, DataTemplate> _dataTemplateMap;
	}
}