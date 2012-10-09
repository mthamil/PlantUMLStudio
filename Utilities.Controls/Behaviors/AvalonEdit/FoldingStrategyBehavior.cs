using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Utilities.Controls.Behaviors.AvalonEdit
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
			AssociatedObject.DataContextChanged += editor_DataContextChanged;
			AssociatedObject.DocumentChanged += editor_DocumentChanged;

			EditorDocumentChanged();
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.DataContextChanged -= editor_DataContextChanged;
			AssociatedObject.DocumentChanged -= editor_DocumentChanged;
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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
					WeakEventManager<TextDocument, DocumentChangeEventArgs>.RemoveHandler(document, "Changed", document_Changed);
				}
				FoldingManager.Uninstall(_currentFoldingManager);
			}
		}

		void editor_DocumentChanged(object sender, EventArgs e)
		{
			EditorDocumentChanged();
		}

		private void EditorDocumentChanged()
		{
			if (FoldingStrategy == null)
				return;

			_currentDocument = new WeakReference<TextDocument>(AssociatedObject.Document);

			TextDocument document;
			if (_currentDocument.TryGetTarget(out document))
			{
				_currentFoldingManager = FoldingManager.Install(AssociatedObject.TextArea);
				FoldingStrategy.UpdateFoldings(_currentFoldingManager, document);

				IEnumerable<NewFolding> foldings;
				if (_documents.TryGetValue(document, out foldings))
					_currentFoldingManager.UpdateFoldings(foldings, -1);
				else
					_documents.Add(document, Enumerable.Empty<NewFolding>());

				WeakEventManager<TextDocument, DocumentChangeEventArgs>.AddHandler(document, "Changed", document_Changed);
			}
		}

		void document_Changed(object sender, DocumentChangeEventArgs e)
		{
			var document = sender as TextDocument;
			if (document == null)
				return;

			if (document != AssociatedObject.Document)
				return;

			FoldingStrategy.UpdateFoldings(_currentFoldingManager, document);
		}

		/// <summary>
		/// Gets or sets an editor folding strategy.
		/// </summary>
		public AbstractFoldingStrategy FoldingStrategy
		{
			get { return (AbstractFoldingStrategy)GetValue(FoldingStrategyProperty); }
			set { SetValue(FoldingStrategyProperty, value); }
		}

		/// <summary>
		/// The FoldingStrategy property.
		/// </summary>
		public static readonly DependencyProperty FoldingStrategyProperty =
			DependencyProperty.Register(
			"FoldingStrategy",
			typeof(AbstractFoldingStrategy),
			typeof(FoldingStrategyBehavior),
			new UIPropertyMetadata(null, OnFoldingStrategyChanged));

		private static void OnFoldingStrategyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (FoldingStrategyBehavior)dependencyObject;
			behavior.EditorDocumentChanged();
		}

		private FoldingManager _currentFoldingManager;
		private WeakReference<TextDocument> _currentDocument;

		private readonly ConditionalWeakTable<TextDocument, IEnumerable<NewFolding>> _documents = new ConditionalWeakTable<TextDocument, IEnumerable<NewFolding>>();
	}
}