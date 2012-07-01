using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Attached property that provides data binding on a text editor's Text.
	/// </summary>
	public static class AvalonEditor
	{
		#region Content

		/// <summary>
		/// Gets the content.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextEditor))]
		public static string GetContent(TextEditor textEditor)
		{
			return (string)textEditor.GetValue(ContentProperty);
		}

		/// <summary>
		/// Sets the content.
		/// </summary>
		public static void SetContent(TextEditor textEditor, string value)
		{
			textEditor.SetValue(ContentProperty, value);
		}

		/// <summary>
		/// The Content property.
		/// </summary>
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.RegisterAttached(
			"Content",
			typeof(string),
			typeof(AvalonEditor),
			new UIPropertyMetadata(string.Empty, OnContentChanged));

		private static void OnContentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var editor = dependencyObject as TextEditor;
			if (editor == null)
				return;

			TextEditorContentBehavior contentBehavior;
			if (!contentBehaviors.TryGetValue(editor, out contentBehavior))
			{
				contentBehavior = new TextEditorContentBehavior(editor);
				contentBehaviors[editor] = contentBehavior;
			}

			contentBehavior.UpdateContent(e.NewValue as string);
		}

		private static readonly IDictionary<TextEditor, TextEditorContentBehavior> contentBehaviors = new Dictionary<TextEditor, TextEditorContentBehavior>();

		#endregion Content

		#region ContentIndex

		/// <summary>
		/// Gets the content index.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextEditor))]
		public static int GetContentIndex(TextEditor textEditor)
		{
			return (int)textEditor.GetValue(ContentIndexProperty);
		}

		/// <summary>
		/// Sets the content index.
		/// </summary>
		public static void SetContentIndex(TextEditor textEditor, int value)
		{
			textEditor.SetValue(ContentIndexProperty, value);
		}

		/// <summary>
		/// The ContentIndex property.
		/// </summary>
		public static readonly DependencyProperty ContentIndexProperty =
			DependencyProperty.RegisterAttached(
			"ContentIndex",
			typeof(int),
			typeof(AvalonEditor),
			new UIPropertyMetadata(-1, OnContentIndexChanged));

		private static void OnContentIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var editor = dependencyObject as TextEditor;
			if (editor == null)
				return;

			TextEditorContentIndexBehavior contentIndexBehavior;
			if (!contentIndexBehaviors.TryGetValue(editor, out contentIndexBehavior))
			{
				contentIndexBehavior = new TextEditorContentIndexBehavior(editor);
				contentIndexBehaviors[editor] = contentIndexBehavior;
			}

			contentIndexBehavior.UpdateIndex((int)e.NewValue);
		}

		private static readonly IDictionary<TextEditor, TextEditorContentIndexBehavior> contentIndexBehaviors = new Dictionary<TextEditor, TextEditorContentIndexBehavior>();

		#endregion ContentIndex

		#region HighlightingDefinition

		/// <summary>
		/// Gets the syntax highlighting definition.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextEditor))]
		public static Uri GetHighlightingDefinition(TextEditor textEditor)
		{
			return (Uri)textEditor.GetValue(ContentProperty);
		}

		/// <summary>
		/// Sets the syntax highlighting definition.
		/// </summary>
		public static void SetHighlightingDefinition(TextEditor textEditor, Uri value)
		{
			textEditor.SetValue(ContentProperty, value);
		}

		/// <summary>
		/// The HighlightingDefinition property.
		/// </summary>
		public static readonly DependencyProperty HighlightingDefinitionProperty =
			DependencyProperty.RegisterAttached(
			"HighlightingDefinition",
			typeof(Uri),
			typeof(AvalonEditor),
			new UIPropertyMetadata(null, OnHighlightingDefinitionChanged));

		private static void OnHighlightingDefinitionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(dependencyObject))
				return;

			var editor = dependencyObject as TextEditor;
			if (editor == null)
				return;

			var definitionUri = e.NewValue as Uri;
			if (definitionUri == null)
				return;

			var definitionPath = definitionUri.IsAbsoluteUri ? 
				definitionUri.AbsolutePath : 
				Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + definitionUri);
			using (var stream = new StreamReader(definitionPath))
			{
				using (var reader = new XmlTextReader(stream))
				{
					editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
				}
			}
		}

		#endregion SyntaxHighlightingDefinition

		#region FoldingStrategy

		/// <summary>
		/// Gets the code folding strategy.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextEditor))]
		public static AbstractFoldingStrategy GetFoldingStrategy(TextEditor textEditor)
		{
			return (AbstractFoldingStrategy)textEditor.GetValue(ContentProperty);
		}

		/// <summary>
		/// Sets the code folding strategy.
		/// </summary>
		public static void SetFoldingStrategy(TextEditor textEditor, AbstractFoldingStrategy value)
		{
			textEditor.SetValue(ContentProperty, value);
		}

		/// <summary>
		/// The FoldingStrategy property.
		/// </summary>
		public static readonly DependencyProperty FoldingStrategyProperty =
			DependencyProperty.RegisterAttached(
			"FoldingStrategy",
			typeof(AbstractFoldingStrategy),
			typeof(AvalonEditor),
			new UIPropertyMetadata(null, OnFoldingStrategyChanged));

		private static void OnFoldingStrategyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(dependencyObject))
				return;

			var editor = dependencyObject as TextEditor;
			if (editor == null)
				return;

			var foldingStrategy = e.NewValue as AbstractFoldingStrategy;
			if (foldingStrategy == null)
				return;

			FoldingStrategyBehavior foldingStrategyBehavior;
			if (!foldingBehaviors.TryGetValue(editor, out foldingStrategyBehavior))
			{
				var foldingBehavior = new FoldingStrategyBehavior(editor.Document, editor.TextArea, foldingStrategy);
				foldingBehaviors[editor] = foldingBehavior;
			}
		}

		private static readonly IDictionary<TextEditor, FoldingStrategyBehavior> foldingBehaviors = new Dictionary<TextEditor, FoldingStrategyBehavior>();

		#endregion FoldingStrategy
	}
}