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
using System.Runtime.CompilerServices;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace PlantUmlStudio.Controls
{
    public partial class BindableTextEditor : TextEditor
    {
        private void OnDataContextChanged_Scroll(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            object scrollOffset;
            if (ScrollOffsets.TryGetValue(e.NewValue, out scrollOffset))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ScrollToHorizontalOffset(((Vector)scrollOffset).X);
                    ScrollToVerticalOffset(((Vector)scrollOffset).Y);
                }));
            }
        }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            var textView = (TextView)sender;
            if (DataContext == null)
                return;

            if (textView.DataContext != DataContext)
                return;

            ScrollOffsets.Remove(DataContext);
            ScrollOffsets.Add(DataContext, textView.ScrollOffset);
        }

        private static readonly ConditionalWeakTable<object, object> ScrollOffsets = new ConditionalWeakTable<object, object>();
    }
}