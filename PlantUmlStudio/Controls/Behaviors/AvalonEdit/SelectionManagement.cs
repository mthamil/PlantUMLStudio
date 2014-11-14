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
	/// Manages a TextEditor's bindable selection-related properties.
	/// </summary>
	public class SelectionManagement : Behavior<TextEditor>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.TextArea.SelectionChanged += textArea_SelectionChanged;
			AssociatedObject.TextArea.Caret.PositionChanged += caret_PositionChanged;
			AssociatedObject.DataContextChanged += editor_DataContextChanged;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.TextArea.SelectionChanged -= textArea_SelectionChanged;
			AssociatedObject.TextArea.Caret.PositionChanged -= caret_PositionChanged;
			AssociatedObject.DataContextChanged -= editor_DataContextChanged;
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Debug.WriteLine("<=== editor_DataContextChanged ===>");

			if (e.OldValue != null && e.NewValue != null)
			{
				Debug.WriteLine("Non-null => Non-null");
				_canUpdateIndex = false;
				_canUpdateSelection = false;
			}
			else if (e.OldValue != null && e.NewValue == null)
			{
				Debug.WriteLine("Non-null => Null");
				_canUpdateIndex = false;
				_canUpdateSelection = false;
			}
			else if (e.OldValue == null && e.NewValue != null)
			{
				Debug.WriteLine("Null => Non-null");
				_canUpdateIndex = true;
				_canUpdateSelection = true;
			}
			else
			{
				Debug.WriteLine("Null => Null: IMPOSSIBLE");
				_canUpdateIndex = false;
				_canUpdateSelection = false;
			}

			Debug.WriteLine("");
		}

		void textArea_SelectionChanged(object sender, EventArgs e)
		{
			Debug.WriteLine("<=== textArea_SelectionChanged ===>");

			if (_canUpdateSelection)
			{
				Debug.WriteLine("SelectionStart = " + SelectionStart);
				Debug.WriteLine("AssociatedObject.SelectionStart = " + AssociatedObject.SelectionStart);

				//if (!_lastStartUpdateFromBinding)
				//{
				//	_lastStartUpdateFromControl = true;
					SelectionStart = AssociatedObject.SelectionStart;
				//}
				//else
				//{
				//	_lastStartUpdateFromBinding = false;
				//}

				Debug.WriteLine("SelectionLength = " + SelectionLength);
				Debug.WriteLine("AssociatedObject.SelectionLength = " + AssociatedObject.SelectionLength);

				//if (!_lastLengthUpdateFromBinding)
				//{
				//	_lastLengthUpdateFromControl = true;
					SelectionLength = AssociatedObject.SelectionLength;
				//}
				//else
				//{
				//	_lastLengthUpdateFromBinding = false;
				//}
			}
			else
			{
				Debug.WriteLine("Not Updating: _canUpdateSelection = FALSE.");
				_canUpdateSelection = true;
			}
			Debug.WriteLine("");
		}

		void caret_PositionChanged(object sender, EventArgs e)
		{
			var caret = sender as Caret;
			if (caret == null)
				return;

			Debug.WriteLine("<=== caret_PositionChanged ===>");

			if (_canUpdateIndex)
			{
				//	if (!_lastIndexUpdateFromBinding)
				//	{
				//		_lastIndexUpdateFromControl = true;
				Debug.WriteLine("CaretIndex = " + CaretIndex);
				Debug.WriteLine("caret.Offset = " + caret.Offset);
				CaretIndex = caret.Offset;
				//	}
				//	else
				//	{
				//		_lastIndexUpdateFromBinding = false;
				//	}
			}
			else
			{
				Debug.WriteLine("Not Updating: _canUpdateIndex = FALSE.");
				_canUpdateIndex = true;
			}
			Debug.WriteLine("");
		}

		private void UpdateSelectionStart(int selectionStart)
		{
			Debug.WriteLine("<=== UpdateSelectionStart ===>");

			if (!_canUpdateSelection)
			{
				Debug.WriteLine("Not updating.");
				return;
			}

			// If the change came from the editor itself, don't update.
			//if (!_lastStartUpdateFromControl)
			//{
			//	_lastStartUpdateFromBinding = true;
			Debug.WriteLine("AssociatedObject.SelectionStart = " + AssociatedObject.SelectionStart);
			Debug.WriteLine("selectionStart = " + selectionStart);
				if (selectionStart > -1 && selectionStart != AssociatedObject.SelectionStart)
					AssociatedObject.SelectionStart = selectionStart;
			//}
			//else
			//{
			//	_lastStartUpdateFromControl = false;
			//}
			Debug.WriteLine("");
		}

		/// <summary>
		/// Gets or sets the selection start.
		/// </summary>
		public int SelectionStart
		{
			get { return (int)GetValue(SelectionStartProperty); }
			set { SetValue(SelectionStartProperty, value); }
		}

		/// <summary>
		/// The SelectionStart property.
		/// </summary>
		public static readonly DependencyProperty SelectionStartProperty =
			DependencyProperty.Register(
			"SelectionStart",
			typeof(int),
			typeof(SelectionManagement),
			new UIPropertyMetadata(0, OnSelectionStartChanged));

		private static void OnSelectionStartChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (SelectionManagement)dependencyObject;
			behavior.UpdateSelectionStart((int)e.NewValue);
		}

		private void UpdateSelectionLength(int selectionLength)
		{
			Debug.WriteLine("<=== UpdateSelectionLength ===>");

			if (!_canUpdateSelection)
			{
				Debug.WriteLine("Not updating.");
				return;
			}

			// If the change came from the editor itself, don't update.
			//if (!_lastLengthUpdateFromControl)
			//{
			//	_lastLengthUpdateFromBinding = true;

			Debug.WriteLine("AssociatedObject.SelectionLength = " + AssociatedObject.SelectionLength);
			Debug.WriteLine("selectionLength = " + selectionLength);
				if (selectionLength > -1 && selectionLength != AssociatedObject.SelectionLength)
					AssociatedObject.SelectionLength = selectionLength;
			//}
			//else
			//{
			//	_lastLengthUpdateFromControl = false;
			//}
			Debug.WriteLine("");
		}

		/// <summary>
		/// Gets or sets the selection length.
		/// </summary>
		public int SelectionLength
		{
			get { return (int)GetValue(SelectionLengthProperty); }
			set { SetValue(SelectionLengthProperty, value); }
		}

		/// <summary>
		/// The SelectionLength property.
		/// </summary>
		public static readonly DependencyProperty SelectionLengthProperty =
			DependencyProperty.Register(
			"SelectionLength",
			typeof(int),
			typeof(SelectionManagement),
			new UIPropertyMetadata(0, OnSelectionLengthChanged));

		private static void OnSelectionLengthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (SelectionManagement)dependencyObject;
			behavior.UpdateSelectionLength((int)e.NewValue);
		}

		private void UpdateIndex(int index)
		{
			Debug.WriteLine("<=== UpdateIndex ===>");

			if (!_canUpdateIndex)
			{
				Debug.WriteLine("Not updating.");
				return;
			}

			//// If the change came from the editor itself, don't update.
			//if (!_lastIndexUpdateFromControl)
			//{
				if (0 <= index && index <= AssociatedObject.Text.Length)
				{
					//AssociatedObject.Select(index, 0);
					Debug.WriteLine("Caret.Offset = " + AssociatedObject.TextArea.Caret.Offset);
					Debug.WriteLine("index = " + index);
					if (AssociatedObject.TextArea.Caret.Offset != index)
					{
						Debug.WriteLine("Values differ");
						AssociatedObject.TextArea.Caret.Offset = index;
					}
				}

			//	if (!_isFirstUpdate)
			//		_lastIndexUpdateFromBinding = true;
			//}
			//else
			//{
			//	_lastIndexUpdateFromControl = false;
			//}

			//_isFirstUpdate = false;
			Debug.WriteLine("");
		}

		/// <summary>
		/// Gets or sets the caret index.
		/// </summary>
		public int CaretIndex
		{
			get { return (int)GetValue(CaretIndexProperty); }
			set { SetValue(CaretIndexProperty, value); }
		}

		/// <summary>
		/// The CaretIndex property.
		/// </summary>
		public static readonly DependencyProperty CaretIndexProperty =
			DependencyProperty.Register(
			"CaretIndex",
			typeof(int),
			typeof(SelectionManagement),
			new UIPropertyMetadata(0, OnCaretIndexChanged));

		private static void OnCaretIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (SelectionManagement)dependencyObject;
			behavior.UpdateIndex((int)e.NewValue);
		}

        //private bool _lastStartUpdateFromControl;
        //private bool _lastStartUpdateFromBinding;

        //private bool _lastLengthUpdateFromControl;
        //private bool _lastLengthUpdateFromBinding;

        //private bool _lastIndexUpdateFromControl;
        //private bool _lastIndexUpdateFromBinding;

        //private bool _dataContextChanged;
		private bool _canUpdateIndex;
		private bool _canUpdateSelection;
	}
}