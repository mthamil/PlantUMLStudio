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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace Utilities.Controls.MultiKey
{
	/// <summary>
	/// Converts a string into a multi-key gesture.
	/// </summary>
	public class MultiKeyGestureConverter : TypeConverter
	{
		/// <summary>
		/// Initializes a new converter.
		/// </summary>
		public MultiKeyGestureConverter()
		{
			_keyConverter = new KeyConverter();
			_modifierKeysConverter = new ModifierKeysConverter();
		}

		/// <see cref="TypeConverter.CanConvertFrom(System.ComponentModel.ITypeDescriptorContext,System.Type)"/>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		/// <see cref="TypeConverter.ConvertFrom(System.ComponentModel.ITypeDescriptorContext,System.Globalization.CultureInfo,object)"/>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var gestureString = value as string;
			if (gestureString == null)
				throw new ArgumentException(@"Invalid gesture string.", "value");

			// Parse the gesture string.
			var keys = new List<KeyInput>();
			var keyStrokes = gestureString.Split(',');
			foreach (var keyStroke in keyStrokes)
			{
				var keyStrokeParts = keyStroke.Split('+');

				KeyInput keyInput = null; 
				if (keyStrokeParts.Length == 2)
				{
					keyInput = new KeyInput
					{
						Key = (Key)_keyConverter.ConvertFrom(keyStrokeParts.Last()),
						Modifier = (ModifierKeys)_modifierKeysConverter.ConvertFrom(keyStrokeParts.First())
					};
				}
				else if (keyStrokeParts.Length == 1)
				{
					keyInput = new KeyInput
					{
						Key = (Key)_keyConverter.ConvertFrom(keyStrokeParts.Single()),
						Modifier = ModifierKeys.None
					};
				}

				if (keyInput != null)
					keys.Add(keyInput);
			}

			return new MultiKeyGesture(keys);
		}

		private readonly KeyConverter _keyConverter;
		private readonly ModifierKeysConverter _modifierKeysConverter;
	}
}