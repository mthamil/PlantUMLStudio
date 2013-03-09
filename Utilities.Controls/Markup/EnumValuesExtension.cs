//  PlantUML Editor
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
using System.Collections;
using System.Windows.Markup;

namespace Utilities.Controls.Markup
{
	/// <summary>
	/// Markup Extension for binding a combo box to enum type
	/// </summary>
	[MarkupExtensionReturnType(typeof(IList))]
	public class EnumValuesExtension : MarkupExtension
	{
		/// <summary>
		/// Initializes a new instance of <see cref="EnumValuesExtension"/>.
		/// </summary>
		/// <param name="enumType">The type of the enum</param>
		public EnumValuesExtension(Type enumType)
		{
			if (enumType == null)
				throw new ArgumentNullException("enumType");

			if (!enumType.IsEnum)
				throw new ArgumentException("enumType must derive from type Enum.");

			_enumType = enumType;
		}

		///<see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Enum.GetValues(_enumType);
		}

		private readonly Type _enumType;
	} 
}