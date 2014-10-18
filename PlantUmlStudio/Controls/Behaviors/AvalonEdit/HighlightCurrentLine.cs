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

using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace PlantUmlStudio.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Behavior that adds highlighting for the line an editor's cursor is on.
	/// </summary>
	public class HighlightCurrentLine : Behavior<TextEditor>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			if (IsEnabled)
				Enable();
		}
		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			Disable();
		}

		private void Enable()
		{
			currentLineHighlighter = new CurrentLineHighlighter(AssociatedObject.TextArea);
			AssociatedObject.TextArea.TextView.BackgroundRenderers.Add(currentLineHighlighter);
			AssociatedObject.TextArea.Caret.PositionChanged += Caret_PositionChanged;
		}

		private void Disable()
		{
			AssociatedObject.TextArea.TextView.BackgroundRenderers.Remove(currentLineHighlighter);
			currentLineHighlighter = null;
			AssociatedObject.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
		}

		void Caret_PositionChanged(object sender, System.EventArgs e)
		{
			AssociatedObject.TextArea.TextView.InvalidateLayer(KnownLayer.Background, DispatcherPriority.Render);
		}

		/// <summary>
		/// Whether current line highlighting is enabled.
		/// </summary>
		public bool IsEnabled
		{
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		/// <summary>
		/// The <see cref="IsEnabled"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.Register(
				"IsEnabled", 
				typeof(bool), 
				typeof(HighlightCurrentLine), 
				new PropertyMetadata(default(bool), OnIsEnabledChanged));


		private static void OnIsEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (HighlightCurrentLine)dependencyObject;
			bool isEnabled = (bool)e.NewValue;

			if (isEnabled)
				behavior.Enable();
			else
				behavior.Disable();
		}

		private IBackgroundRenderer currentLineHighlighter;
	}
}