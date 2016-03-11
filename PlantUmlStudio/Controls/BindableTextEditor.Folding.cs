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

            FoldingManager.Uninstall(_currentFoldingManager);
            _currentFoldingManager = null;
        }

        protected override void OnDocumentChanged(EventArgs e)
        {
            base.OnDocumentChanged(e);
            OnDocumentChangedFolding();
        }

        private void OnDocumentChangedFolding()
        {
            if (FoldingStrategy == null || Document == null)
                return;

            _currentDocument?.TryGetTarget()
                             .Apply(document =>
                                    document.RemoveWeakHandler<TextDocument, DocumentChangeEventArgs>(nameof(TextDocument.Changed), Document_Changed));

            _currentFoldingManager = FoldingManager.Install(TextArea);

            // Update foldings.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CurrentFoldings == null)
                {
                    _currentFoldingManager.GenerateFoldings(Document, FoldingStrategy);
                }
                else
                {
                    _currentFoldingManager.UpdateFoldings(CurrentFoldings);
                }
                UpdateCurrentFoldings();
            }));
                        

            _currentDocument = new WeakReference<TextDocument>(Document);
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
            UpdateCurrentFoldings();
        }

        private void TextView_VisualLinesChanged(object sender, EventArgs e)
        {
            if (DataContext == null || Document == null || _currentFoldingManager == null)
                return;

            if (!_currentFoldingManager.AllFoldings.Any())
                return;

            if (!_currentFoldingManager.AllFoldings.Select(f => f.IsFolded).SequenceEqual(
                 CurrentFoldings?.Select(f => f.DefaultClosed) ?? Enumerable.Empty<bool>()))
            {
                UpdateCurrentFoldings();
            }
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
            editor.OnDocumentChangedFolding();
        }

        private void UpdateCurrentFoldings()
        {
            CurrentFoldings =
                _currentFoldingManager
                    .AllFoldings
                    .Select(f => new NewFolding(f.StartOffset, f.EndOffset)
                    {
                        DefaultClosed = f.IsFolded,
                        Name = f.Title
                    })
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
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        private FoldingManager _currentFoldingManager;
        private static WeakReference<TextDocument> _currentDocument;
    }
}