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
using System.Diagnostics;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace PlantUmlStudio.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable content index property.
	/// </summary>
	public class ContentIndexBehavior : Behavior<TextEditor>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.TextArea.Caret.PositionChanged += caret_PositionChanged;
			AssociatedObject.DataContextChanged += editor_DataContextChanged;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.TextArea.Caret.PositionChanged -= caret_PositionChanged;
			AssociatedObject.DataContextChanged -= editor_DataContextChanged;
		}

		internal void UpdateIndex(int index)
		{
			Debug.WriteLine("<=== UpdateIndex ===>");

			if (_wasNullContext)
			{
				Debug.WriteLine("Not updating, null data context.");
				return;
			}

			//// If the change came from the editor itself, don't update.
			//if (!_lastIndexUpdateFromControl)
			//{
			//	if (0 <= index && index <= AssociatedObject.Text.Length) // TODO: Really fix this.
			//	{
					//AssociatedObject.Select(index, 0);
					Debug.WriteLine("Caret.Offset = " + AssociatedObject.TextArea.Caret.Offset);
					Debug.WriteLine("index = " + index);
					if (AssociatedObject.TextArea.Caret.Offset != index)
					{
						Debug.WriteLine("Values differ");
						AssociatedObject.TextArea.Caret.Offset = index;
					}
			//	}

			//	if (!_isFirstUpdate)
			//		_lastIndexUpdateFromBinding = true;
			//}
			//else
			//{
			//	_lastIndexUpdateFromControl = false;
			//}

			//_isFirstUpdate = false;
		}

		void caret_PositionChanged(object sender, EventArgs e)
		{
			var caret = sender as Caret;
			if (caret == null)
				return;

			Debug.WriteLine("<=== caret_PositionChanged ===>");

			if (_wasNullContext)
			{
				Debug.WriteLine("Not updating, null data context.");
				return;
			}

			if (!_dataContextChanged)
			{
			//	if (!_lastIndexUpdateFromBinding)
			//	{
			//		_lastIndexUpdateFromControl = true;
					Debug.WriteLine("ContentIndex = " + ContentIndex);
					Debug.WriteLine("caret.Offset = " + caret.Offset);
					ContentIndex = caret.Offset;
			//	}
			//	else
			//	{
			//		_lastIndexUpdateFromBinding = false;
			//	}
			}
			else
			{
				Debug.WriteLine("Not Updating");
				_dataContextChanged = false;
			}
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Debug.WriteLine("<=== editor_DataContextChanged ===>");
			if (AssociatedObject.DataContext != null)
			{
				Debug.WriteLine("Non-null context.");
				if (!_isFirstUpdate)
				{
					_dataContextChanged = true;
					_lastIndexUpdateFromControl = false;
					_lastIndexUpdateFromBinding = false;
				}
				else
				{
					Debug.WriteLine("Not setting values, is first update.");
				}

				_wasNullContext = false;
			}
			else
			{
				Debug.WriteLine("NULL context!");
				_wasNullContext = true;
			}

			_isFirstUpdate = false;
		}

		/// <summary>
		/// Gets or sets the content index.
		/// </summary>
		public int ContentIndex
		{
			get { return (int)GetValue(ContentIndexProperty); }
			set { SetValue(ContentIndexProperty, value); }
		}

		/// <summary>
		/// The ContentIndex property.
		/// </summary>
		public static readonly DependencyProperty ContentIndexProperty =
			DependencyProperty.Register(
			"ContentIndex",
			typeof(int),
			typeof(ContentIndexBehavior),
			new UIPropertyMetadata(0, OnContentIndexChanged));

		private static void OnContentIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (ContentIndexBehavior)dependencyObject;
			behavior.UpdateIndex((int)e.NewValue);
		}

		private bool _isFirstUpdate = true;

		private bool _lastIndexUpdateFromControl;
		private bool _lastIndexUpdateFromBinding;
		private bool _dataContextChanged;
		private bool _wasNullContext;
	}
}