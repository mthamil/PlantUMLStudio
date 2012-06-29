using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit;
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

			TextEditorContentProperty contentProperty;
			if (!contentBehaviors.TryGetValue(editor, out contentProperty))
			{
				contentProperty = new TextEditorContentProperty(editor);
				contentBehaviors[editor] = contentProperty;
			}

			contentProperty.UpdateContent(e.NewValue as string);
		}

		private static readonly IDictionary<TextEditor, TextEditorContentProperty> contentBehaviors = new Dictionary<TextEditor, TextEditorContentProperty>();

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

			TextEditorContentIndexProperty contentIndexProperty;
			if (!contentIndexBehaviors.TryGetValue(editor, out contentIndexProperty))
			{
				contentIndexProperty = new TextEditorContentIndexProperty(editor);
				contentIndexBehaviors[editor] = contentIndexProperty;
			}

			contentIndexProperty.UpdateIndex((int)e.NewValue);
		}

		private static readonly IDictionary<TextEditor, TextEditorContentIndexProperty> contentIndexBehaviors = new Dictionary<TextEditor, TextEditorContentIndexProperty>();

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
		public static void SetHighlightingDefinition(TextEditor textEditor, string value)
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
	}
}