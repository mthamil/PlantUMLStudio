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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Utilities.Controls.Selectors
{
	/// <summary>
	/// A markup extension that allows definition of a data template selector that is composed of multiple individual
	/// data templates and is keyed off of the templates' data types.
	/// </summary>
	public class TypeBasedDataTemplateSelector : MarkupExtension
	{
		/// <summary>
		/// Initializes a new selector.
		/// </summary>
		public TypeBasedDataTemplateSelector()
		{
			_selector = new Lazy<DataTemplateSelector>(() => new TypeMapDataTemplateSelector(DataTemplates));
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
	public class TypeMapDataTemplateSelector : DataTemplateSelector
	{
		/// <summary>
		/// Initializes a new data template selector.
		/// </summary>
		/// <param name="dataTemplates">The data templates to select from</param>
		public TypeMapDataTemplateSelector(IEnumerable<DataTemplate> dataTemplates)
		{
			int order = 0;
			var templateOrder = new Dictionary<Type, int>();
			_dataTemplateMap = dataTemplates.ToDictionary(k =>
			{
				templateOrder[(Type)k.DataType] = order++;
				return (Type)k.DataType;
			}, v => v);

			_orderedTemplates = _dataTemplateMap.OrderBy(dt => templateOrder[dt.Key]).ToList();
		}

		/// <see cref="DataTemplateSelector.SelectTemplate"/>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) 
				throw new ArgumentNullException("item");

			var dataType = item.GetType();

			DataTemplate template;
			if (_dataTemplateMap.TryGetValue(dataType, out template))
				return template;

			// Fallback to assignability.
			foreach (var dataTemplate in _orderedTemplates)
			{
				if (dataTemplate.Key.IsAssignableFrom(dataType))
					return dataTemplate.Value;
			}

			return null;
		}

		private readonly IDictionary<Type, DataTemplate> _dataTemplateMap;
		private readonly IList<KeyValuePair<Type, DataTemplate>> _orderedTemplates;
	}
}