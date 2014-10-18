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
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using Utilities.Collections;
using Utilities.Reflection;

namespace PlantUmlStudio.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Provides bindable properties for <see cref="TextEditorOptions"/>.
	/// </summary>
	public class BindableOptions : Behavior<TextEditor>
	{
		protected override void OnAttached()
		{
			AssociatedObject.OptionChanged += TextEditor_OptionChanged;
		}

		protected override void OnDetaching()
		{
			AssociatedObject.OptionChanged -= TextEditor_OptionChanged;
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
		/// The EnableVirtualSpace dependency property.
		/// </summary>
		public static readonly DependencyProperty EnableVirtualSpaceProperty =
			DependencyProperty.Register(
				"EnableVirtualSpace", 
				typeof(bool), 
				typeof(BindableOptions),
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
		/// The CutCopyWholeLine dependency property.
		/// </summary>
		public static readonly DependencyProperty CutCopyWholeLineProperty =
			DependencyProperty.Register(
				"CutCopyWholeLine", 
				typeof(bool), 
				typeof(BindableOptions), 
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
		/// The AllowScrollBelowDocument dependency property.
		/// </summary>
		public static readonly DependencyProperty AllowScrollBelowDocumentProperty =
			DependencyProperty.Register(
				"AllowScrollBelowDocument", 
				typeof(bool), 
				typeof(BindableOptions), 
				new FrameworkPropertyMetadata(default(bool),
					FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
					CreateCallback<bool>((options, value) => options.AllowScrollBelowDocument = value)));

		private static PropertyChangedCallback CreateCallback<TValue>(Action<TextEditorOptions, TValue> update)
		{
			return (dependencyObject, e) => update(((BindableOptions)dependencyObject).AssociatedObject.Options, (TValue)e.NewValue); 
		}

		void TextEditor_OptionChanged(object sender, PropertyChangedEventArgs e)
		{
			optionsUpdateMap.TryGetValue(e.PropertyName).Apply(update => update(this, AssociatedObject.Options));
		}

		private static readonly IDictionary<string, Action<BindableOptions, TextEditorOptions>> optionsUpdateMap = new Dictionary<string, Action<BindableOptions, TextEditorOptions>>
		{
			{ Reflect.PropertyOf<TextEditorOptions>(p => p.EnableVirtualSpace).Name, (bo, to) => bo.EnableVirtualSpace = to.EnableVirtualSpace },
			{ Reflect.PropertyOf<TextEditorOptions>(p => p.CutCopyWholeLine).Name, (bo, to) => bo.CutCopyWholeLine = to.CutCopyWholeLine },
			{ Reflect.PropertyOf<TextEditorOptions>(p => p.AllowScrollBelowDocument).Name, (bo, to) => bo.AllowScrollBelowDocument = to.AllowScrollBelowDocument }
		};
	}
}