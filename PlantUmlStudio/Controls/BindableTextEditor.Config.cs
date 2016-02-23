//  PlantUML Studio
//  Copyright 2016 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - omaralzabir@gmail.com (original author)
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
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using SharpEssentials.Collections;

namespace PlantUmlStudio.Controls
{
    [ContentProperty(nameof(Text))]
    public partial class BindableTextEditor : TextEditor
    {
        public Brush CurrentLineBackground
        {
            get { return TextArea.TextView.CurrentLineBackground; }
            set { TextArea.TextView.CurrentLineBackground = value; }
        }

        public Pen CurrentLineBorder
        {
            get { return TextArea.TextView.CurrentLineBorder; }
            set { TextArea.TextView.CurrentLineBorder = value; }
        }

        /// <summary>
		/// Gets/Sets whether the user can set the caret behind the line ending
		/// (into "virtual space").
		/// Note that virtual space is always used (independent from this setting)
		/// when doing rectangle selections.
		/// </summary>
		public bool EnableVirtualSpace
        {
            get { return (bool)GetValue(EnableVirtualSpaceProperty); }
            set { SetValue(EnableVirtualSpaceProperty, value); }
        }

        /// <summary>
        /// The <see cref="EnableVirtualSpace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableVirtualSpaceProperty =
            DependencyProperty.Register(nameof(EnableVirtualSpace),
                typeof(bool),
                typeof(BindableTextEditor),
                new FrameworkPropertyMetadata(default(bool),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    CreateCallback<bool>((options, value) => options.EnableVirtualSpace = value)));

        /// <summary>
        /// Gets/Sets whether copying without a selection copies the whole current line.
        /// </summary>
        public bool CutCopyWholeLine
        {
            get { return (bool)GetValue(CutCopyWholeLineProperty); }
            set { SetValue(CutCopyWholeLineProperty, value); }
        }

        /// <summary>
        /// The <see cref="CutCopyWholeLine"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CutCopyWholeLineProperty =
            DependencyProperty.Register(nameof(CutCopyWholeLine),
                typeof(bool),
                typeof(BindableTextEditor),
                new FrameworkPropertyMetadata(default(bool),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    CreateCallback<bool>((options, value) => options.CutCopyWholeLine = value)));

        /// <summary>
        /// Gets/Sets whether the user can scroll below the bottom of the document.
        /// The default value is false; but it a good idea to set this property to true when using folding.
        /// </summary>
        public bool AllowScrollBelowDocument
        {
            get { return (bool)GetValue(AllowScrollBelowDocumentProperty); }
            set { SetValue(AllowScrollBelowDocumentProperty, value); }
        }

        /// <summary>
        /// The <see cref="AllowScrollBelowDocument"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowScrollBelowDocumentProperty =
            DependencyProperty.Register(nameof(AllowScrollBelowDocument),
                typeof(bool),
                typeof(BindableTextEditor),
                new FrameworkPropertyMetadata(default(bool),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    CreateCallback<bool>((options, value) => options.AllowScrollBelowDocument = value)));

        /// <summary>
		/// Gets/Sets whether the current line should be highlighted.
		/// </summary>
		public bool HighlightCurrentLine
        {
            get { return (bool)GetValue(HighlightCurrentLineProperty); }
            set { SetValue(HighlightCurrentLineProperty, value); }
        }

        /// <summary>
        /// The <see cref="HighlightCurrentLine"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightCurrentLineProperty =
            DependencyProperty.Register(nameof(HighlightCurrentLine),
                typeof(bool),
                typeof(BindableTextEditor),
                new FrameworkPropertyMetadata(default(bool),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    CreateCallback<bool>((options, value) => options.HighlightCurrentLine = value)));

        private static PropertyChangedCallback CreateCallback<TValue>(Action<TextEditorOptions, TValue> update)
        {
            return (dependencyObject, e) => update(((BindableTextEditor)dependencyObject).Options, (TValue)e.NewValue);
        }

        protected override void OnOptionChanged(PropertyChangedEventArgs e)
        {
            base.OnOptionChanged(e);
            OptionsUpdateMap.TryGetValue(e.PropertyName ?? string.Empty)
                            .Apply(update => update(this, Options));
        }

        private static readonly IDictionary<string, Action<BindableTextEditor, TextEditorOptions>> OptionsUpdateMap = new Dictionary<string, Action<BindableTextEditor, TextEditorOptions>>
        {
            { nameof(EnableVirtualSpace), (bo, to) => bo.EnableVirtualSpace = to.EnableVirtualSpace },
            { nameof(CutCopyWholeLine), (bo, to) => bo.CutCopyWholeLine = to.CutCopyWholeLine },
            { nameof(AllowScrollBelowDocument), (bo, to) => bo.AllowScrollBelowDocument = to.AllowScrollBelowDocument },
            { nameof(HighlightCurrentLine), (bo, to) => bo.HighlightCurrentLine = to.HighlightCurrentLine }
        };
    }
}