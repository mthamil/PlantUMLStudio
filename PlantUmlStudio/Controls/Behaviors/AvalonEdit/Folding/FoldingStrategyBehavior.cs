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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using SharpEssentials.Weak;

namespace PlantUmlStudio.Controls.Behaviors.AvalonEdit.Folding
{
	/// <summary>
	/// Behavior for managing a text editor's folding strategy.
	/// </summary>
	/// <remarks>This is a mess!</remarks>
	public class FoldingStrategyBehavior : Behavior<TextEditor>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.DataContextChanged += Editor_DataContextChanged;
			AssociatedObject.DocumentChanged += Editor_DocumentChanged;

			OnEditorDocumentChanged();
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.DataContextChanged -= Editor_DataContextChanged;
			AssociatedObject.DocumentChanged -= Editor_DocumentChanged;
		}

		void Editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_currentFoldingManager != null)
			{
				// Currently the DefaultClosed property does not seem to be respected when foldings are restored.
				var foldings = _currentFoldingManager
					.AllFoldings
					.Select(f => new NewFolding(f.StartOffset, f.EndOffset) { DefaultClosed = f.IsFolded, Name = f.Title });

				TextDocument document;
				if (_currentDocument.TryGetTarget(out document))
				{
					_documents.Remove(document);
					_documents.Add(document, foldings.ToList());
                    document.RemoveWeakHandler<TextDocument, DocumentChangeEventArgs>(nameof(TextDocument.Changed), Document_Changed);
				}
				FoldingManager.Uninstall(_currentFoldingManager);
			}
		}

		void Editor_DocumentChanged(object sender, EventArgs e)
		{
			OnEditorDocumentChanged();
		}

		private void OnEditorDocumentChanged()
		{
			if (FoldingStrategy == null || AssociatedObject.Document == null)
				return;

			_currentDocument = new WeakReference<TextDocument>(AssociatedObject.Document);
			_currentFoldingManager = FoldingManager.Install(AssociatedObject.TextArea);

			IEnumerable<NewFolding> foldings;
		    if (_documents.TryGetValue(AssociatedObject.Document, out foldings))
		    {
		        _currentFoldingManager.UpdateFoldings(foldings);
		    }
		    else
		    {
		        _documents.Add(AssociatedObject.Document, Enumerable.Empty<NewFolding>());
		        _currentFoldingManager.GenerateFoldings(AssociatedObject.Document, FoldingStrategy);
		    }

		    AssociatedObject.Document.AddWeakHandler<TextDocument, DocumentChangeEventArgs>(nameof(TextDocument.Changed), Document_Changed);
		}

		void Document_Changed(object sender, DocumentChangeEventArgs e)
		{
			var document = sender as TextDocument;
			if (document == null)
				return;

			if (document != AssociatedObject.Document)
				return;

            _currentFoldingManager.GenerateFoldings(document, FoldingStrategy);
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
		/// The FoldingStrategy property.
		/// </summary>
		public static readonly DependencyProperty FoldingStrategyProperty =
			DependencyProperty.Register(nameof(FoldingStrategy),
			    typeof(IFoldingStrategy),
			    typeof(FoldingStrategyBehavior),
			    new UIPropertyMetadata(null, OnFoldingStrategyChanged));

		private static void OnFoldingStrategyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (FoldingStrategyBehavior)dependencyObject;
			behavior.OnEditorDocumentChanged();
		}

		private FoldingManager _currentFoldingManager;
		private WeakReference<TextDocument> _currentDocument;

		private readonly ConditionalWeakTable<TextDocument, IEnumerable<NewFolding>> _documents = new ConditionalWeakTable<TextDocument, IEnumerable<NewFolding>>();
	}
}