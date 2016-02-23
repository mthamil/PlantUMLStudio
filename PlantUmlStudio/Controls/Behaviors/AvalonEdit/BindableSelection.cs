//  PlantUML Studio
//  Copyright 2016 Matthew Hamilton - matthamilton@live.com
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
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace PlantUmlStudio.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable selection-related properties.
	/// </summary>
	public class BindableSelection : Behavior<TextEditor>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.TextArea.SelectionChanged += TextArea_SelectionChanged;
			AssociatedObject.TextArea.Caret.PositionChanged += Caret_PositionChanged;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.TextArea.SelectionChanged -= TextArea_SelectionChanged;
			AssociatedObject.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
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
			DependencyProperty.Register(nameof(SelectionStart),
			    typeof(int),
			    typeof(BindableSelection),
			    new UIPropertyMetadata(0, OnSelectionStartChanged));

		private static void OnSelectionStartChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
            var behavior = (BindableSelection)dependencyObject;
		    if (behavior.AssociatedObject.Document == null)
		        return;

            var selectionStart = (int)e.NewValue;
            if (0 <= selectionStart && selectionStart <= behavior.AssociatedObject.Document.TextLength)
                behavior.AssociatedObject.SelectionStart = selectionStart;
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
			DependencyProperty.Register(nameof(SelectionLength),
			    typeof(int),
			    typeof(BindableSelection),
			    new UIPropertyMetadata(0, OnSelectionLengthChanged));

		private static void OnSelectionLengthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
            var behavior = (BindableSelection)dependencyObject;
            if (behavior.AssociatedObject.Document == null)
                return;

            var selectionLength = (int)e.NewValue;
            if (selectionLength > -1)
                behavior.AssociatedObject.SelectionLength = selectionLength;
        }

        private void TextArea_SelectionChanged(object sender, EventArgs e)
        {
            if (AssociatedObject.DataContext == null)
                return;

            if (AssociatedObject.TextArea.TextView.DataContext != AssociatedObject.DataContext)
                return;

            SelectionStart = AssociatedObject.SelectionStart;
            SelectionLength = AssociatedObject.SelectionLength;
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            var caret = (Caret)sender;
            if (AssociatedObject.DataContext == null)
                return;

            if (AssociatedObject.TextArea.TextView.DataContext != AssociatedObject.DataContext)
                return;

            if (AssociatedObject.SelectionLength == 0)
                SelectionStart = caret.Offset;
        }
    }
}