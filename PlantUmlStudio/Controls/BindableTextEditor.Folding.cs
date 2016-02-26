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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using PlantUmlStudio.Controls.Behaviors.AvalonEdit.Folding;
using SharpEssentials.Weak;

namespace PlantUmlStudio.Controls
{
    public partial class BindableTextEditor : TextEditor
    {
        private void OnDataContextChanged_Folding(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_currentFoldingManager == null)
                return;

            _currentDocument.TryGetTarget()
                            .Apply(document =>
                            {
                                var foldings = _currentFoldingManager
                                    .AllFoldings
                                    .Select(f => new NewFolding(f.StartOffset, f.EndOffset) { DefaultClosed = f.IsFolded, Name = f.Title })
                                    .ToList();

                                Foldings.Remove(document);
                                Foldings.Add(document, foldings);
                                document.RemoveWeakHandler<TextDocument, DocumentChangeEventArgs>(nameof(TextDocument.Changed), Document_Changed);
                            });


            FoldingManager.Uninstall(_currentFoldingManager);
        }

        protected override void OnDocumentChanged(EventArgs e)
        {
            base.OnDocumentChanged(e);
            OnEditorDocumentChanged();
        }

        private void OnEditorDocumentChanged()
        {
            if (FoldingStrategy == null || Document == null)
                return;

            _currentDocument = new WeakReference<TextDocument>(Document);
            _currentFoldingManager = FoldingManager.Install(TextArea);

            IEnumerable<NewFolding> foldings;
            if (Foldings.TryGetValue(Document, out foldings))
            {
                _currentFoldingManager.UpdateFoldings(foldings);
            }
            else
            {
                Foldings.Add(Document, Enumerable.Empty<NewFolding>());
                _currentFoldingManager.GenerateFoldings(Document, FoldingStrategy);
            }
            //UpdateCurrentFoldings();

            Document.AddWeakHandler<TextDocument, DocumentChangeEventArgs>(nameof(TextDocument.Changed), Document_Changed);
        }

        private void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            var document = sender as TextDocument;
            if (document == null)
                return;

            if (document != Document)
                return;

            _currentFoldingManager.GenerateFoldings(document, FoldingStrategy);
            //UpdateCurrentFoldings();
        }

        /// <summary>
        /// Gets or sets an editor folding strategy.
        /// </summary>
        public IFoldingStrategy FoldingStrategy
        {
            get { return (IFoldingStrategy)GetValue(FoldingStrategyProperty); }
            set { SetValue(FoldingStrategyProperty, value); }
        }

        /// <summary>
        /// The <see cref="FoldingStrategy"/> property.
        /// </summary>
        public static readonly DependencyProperty FoldingStrategyProperty =
            DependencyProperty.Register(nameof(FoldingStrategy),
                typeof(IFoldingStrategy),
                typeof(BindableTextEditor),
                new PropertyMetadata(null, OnFoldingStrategyChanged));

        private static void OnFoldingStrategyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var editor = (BindableTextEditor)dependencyObject;
            editor.OnEditorDocumentChanged();
        }

        private void UpdateCurrentFoldings()
        {
            CurrentFoldings = _currentFoldingManager
                                    .AllFoldings
                                    .Select(f => new NewFolding(f.StartOffset, f.EndOffset) { DefaultClosed = f.IsFolded, Name = f.Title })
                                    .ToList();
        }

        public IEnumerable<NewFolding> CurrentFoldings
        {
            get { return (IEnumerable<NewFolding>)GetValue(CurrentFoldingsProperty); }
            set { SetValue(CurrentFoldingsProperty, value); }
        }

        public static readonly DependencyProperty CurrentFoldingsProperty =
            DependencyProperty.Register(nameof(CurrentFoldings),
                typeof(IEnumerable<NewFolding>),
                typeof(BindableTextEditor),
                new FrameworkPropertyMetadata(
                    Enumerable.Empty<NewFolding>(),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) => OnCurrentFoldingsChanged((BindableTextEditor)d,
                                                       (IEnumerable<NewFolding>)e.NewValue,
                                                       (IEnumerable<NewFolding>)e.OldValue)));

        private static void OnCurrentFoldingsChanged(BindableTextEditor editor, IEnumerable<NewFolding> newValue, IEnumerable<NewFolding> oldValue)
        {
            //if (editor.Document == null)
            //    return;

            //if (behavior.AssociatedObject.TextArea.TextView.DataContext != behavior.AssociatedObject.DataContext)
            //    return;

            //if (newValue == null)
            //{
            //    editor._currentFoldingManager.GenerateFoldings(editor.Document, editor.FoldingStrategy);
            //    editor.UpdateCurrentFoldings();
            //}
            //else
            //{
            //    editor._currentFoldingManager.UpdateFoldings(newValue);
            //}
        }

        private FoldingManager _currentFoldingManager;

        private static WeakReference<TextDocument> _currentDocument;
        private static readonly ConditionalWeakTable<TextDocument, IEnumerable<NewFolding>> Foldings = new ConditionalWeakTable<TextDocument, IEnumerable<NewFolding>>();
    }
}