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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Utilities.Controls.MultiKey
{
	/// <summary>
	/// A multi-key input gesture.
	/// </summary>
	[TypeConverter(typeof(MultiKeyGestureConverter))]
	public class MultiKeyGesture : KeyGesture
	{
		/// <summary>
		/// Creates a gesture with no display string.
		/// </summary>
		/// <param name="keys">The key gesture</param>
		public MultiKeyGesture(IEnumerable<KeyInput> keys)
			: this(keys, string.Empty) { }

		/// <summary>
		/// Creates a gesture with a display string.
		/// </summary>
		/// <param name="keys">The key gesture</param>
		/// <param name="displayString">A display string</param>
		public MultiKeyGesture(IEnumerable<KeyInput> keys, string displayString)
			: base(Key.None, ModifierKeys.None, displayString)
		{
			if (!keys.Any())
				throw new ArgumentException(@"At least one key must be specified.", "keys");

			_keys = new List<KeyInput>(keys);

			Keys = new ReadOnlyCollection<Key>(_keys.Select(k => k.Key).ToList());
			Modifiers = new ReadOnlyCollection<ModifierKeys>(_keys.Select(k => k.Modifier).ToList());
		}

		/// <summary>
		/// The keys used for a gesture.
		/// </summary>
		public ICollection<Key> Keys
		{
			get;
			private set;
		}

		/// <summary>
		/// The modifiers used for a gesture.
		/// </summary>
		public new ICollection<ModifierKeys> Modifiers
		{
			get;
			private set;
		}

		/// <see cref="KeyGesture.Matches"/>
		public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
		{
			var args = inputEventArgs as KeyEventArgs;
			if ((args == null))
				return false;

			var keyboardDevice = inputEventArgs.Device as KeyboardDevice;
			if (keyboardDevice == null)
				return false;

			if (_currentKeyIndex != 0 && ((DateTime.Now - _lastKeyPressTime) > MaximumDelayBetweenKeyPresses))
			{
				// The key gesture timed-out.
				_currentKeyIndex = 0;
				return false;
			}
			
			// Check for the correct modifier key.)
			if (_keys[_currentKeyIndex].Modifier != keyboardDevice.Modifiers)
			{
				// The wrong modifier was used.
				_currentKeyIndex = 0;
				return false;
			}

			if (_keys[_currentKeyIndex].Key != args.Key)
			{
				// wrong key
				_currentKeyIndex = 0;
				return false;
			}

			++_currentKeyIndex;

			if (_currentKeyIndex != _keys.Count)
			{
				// Still looking for a key sequence match.
				_lastKeyPressTime = DateTime.Now;
				inputEventArgs.Handled = true;
				return false;
			}

			// The key gesture matched.
			_currentKeyIndex = 0;
			return true;
		}

		private readonly IList<KeyInput> _keys;
		private int _currentKeyIndex;
		private DateTime _lastKeyPressTime;

		/// <summary>
		/// The maximum delay allowed between keys of a sequence.
		/// </summary>
		internal static TimeSpan MaximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(1);
	}

	/// <summary>
	/// Represents a key and an optional modifier.
	/// </summary>
	public class KeyInput
	{
		/// <summary>
		/// Initializes a new key input pair.
		/// </summary>
		public KeyInput()
		{
			Key = Key.None;
			Modifier = ModifierKeys.None;
		}

		/// <summary>
		/// The key to use.  This is required.
		/// </summary>
		public Key Key { get; set; }

		/// <summary>
		/// The key modifier.  This is optional.
		/// </summary>
		public ModifierKeys Modifier { get; set; }
	}
}