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
	/// A markup extension that allows definition of a style selector that is composed of multiple individual
	/// styles and is keyed off of the type of a component's content.
	/// </summary>
	public class TypeBasedStyleSelector : MarkupExtension
	{
		/// <summary>
		/// Initializes a new selector.
		/// </summary>
		public TypeBasedStyleSelector()
		{
			_selector = new Lazy<StyleSelector>(() => 
				new TypeMapStyleSelector(Styles.ToDictionary(k => k.DataType, v => v.Style)));
		}

		#region Overrides of MarkupExtension

		/// <see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _selector.Value;
		}

		#endregion

		/// <summary>
		/// The styles to select from.
		/// </summary>
		public Collection<StyleMapEntry> Styles
		{
			get { return new Collection<StyleMapEntry>(_styles); }
		}

		private readonly IList<StyleMapEntry> _styles = new List<StyleMapEntry>();
		private readonly Lazy<StyleSelector> _selector;
	}

	/// <summary>
	/// An entry in a style map.
	/// </summary>
	public class StyleMapEntry
	{
		/// <summary>
		/// The target data type.
		/// </summary>
		public Type DataType { get; set; }

		/// <summary>
		/// The style to use.
		/// </summary>
		public Style Style { get; set; }
	}

	/// <summary>
	/// A style selector that is composed of multiple individual styles and is 
	/// keyed off of the type of a component's content.
	/// </summary>
	public class TypeMapStyleSelector : StyleSelector
	{
		/// <summary>
		/// Initializes a new style selector.
		/// </summary>
		/// <param name="styleMap">The styles to select from</param>
		public TypeMapStyleSelector(IDictionary<Type, Style> styleMap)
		{
			_styleMap = styleMap;
		}

		/// <see cref="StyleSelector.SelectStyle"/>
		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item == null) 
				throw new ArgumentNullException("item");

			Style style;
			if (_styleMap.TryGetValue(item.GetType(), out style))
				return style;

			return null;
		}

		private readonly IDictionary<Type, Style> _styleMap;
	}
}