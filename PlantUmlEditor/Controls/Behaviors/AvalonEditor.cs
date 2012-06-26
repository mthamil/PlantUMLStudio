using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace PlantUmlEditor.Controls.Behaviors
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

			if (!contentEditors.Contains(editor))
			{
				contentEditors.Add(editor);
				contentDocumentMap[editor.Document] = editor;
				editor.Document.TextChanged += contentEditor_TextChanged;
			}

			// If the change came from the editor itself, don't update.
			if (!contentTextChangedEditors.Contains(editor))
			{
				contentChangedEditors.Add(editor);
				var newValue = e.NewValue as string;
				editor.Document.Text = newValue;
			}
			else
			{
				contentTextChangedEditors.Remove(editor);
			}
		}

		static void contentEditor_TextChanged(object sender, EventArgs e)
		{
			var document = sender as TextDocument;
			if (document == null)
				return;

			var editor = contentDocumentMap[document];
			if (!contentChangedEditors.Contains(editor))
				contentTextChangedEditors.Add(editor);
			else
				contentChangedEditors.Remove(editor);

;			SetContent(editor, document.Text);
		}

		private static readonly ICollection<TextEditor> contentEditors = new HashSet<TextEditor>();

		/// <summary>
		/// Maps documents to their parent editors.
		/// </summary>
		private static readonly IDictionary<TextDocument, TextEditor> contentDocumentMap = new Dictionary<TextDocument, TextEditor>();

		/// <summary>
		/// Prevents double updates by tracking whether a change came from the text editor itself.
		/// </summary>
		private static readonly ICollection<TextEditor> contentTextChangedEditors = new HashSet<TextEditor>();

		private static readonly ICollection<TextEditor> contentChangedEditors = new HashSet<TextEditor>();

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

			if (!contentIndexEditors.Contains(editor))
			{
				contentIndexEditors.Add(editor);
				contentIndexCaretMap[editor.TextArea.Caret] = editor;
				editor.TextArea.Caret.PositionChanged += caret_PositionChanged;
			}

			// If the change came from the editor itself, don't update.
			if (!contentIndexPositionChangedEditors.Contains(editor))
			{
				contentIndexChangedEditors.Add(editor);
				var newValue = (int)e.NewValue;
				editor.TextArea.Caret.Offset = newValue;
			}
			else
			{
				contentIndexPositionChangedEditors.Remove(editor);
			}
		}

		static void caret_PositionChanged(object sender, EventArgs e)
		{
			var caret = sender as Caret;
			if (caret == null)
				return;

			var editor = contentIndexCaretMap[caret];
			if (!contentIndexChangedEditors.Contains(editor))
				contentIndexPositionChangedEditors.Add(editor);
			else
				contentIndexChangedEditors.Remove(editor);

			SetContentIndex(editor, caret.Offset);
		}

		private static readonly ICollection<TextEditor> contentIndexEditors = new HashSet<TextEditor>();

		/// <summary>
		/// Maps carets to their parent editors.
		/// </summary>
		private static readonly IDictionary<Caret, TextEditor> contentIndexCaretMap = new Dictionary<Caret, TextEditor>();

		/// <summary>
		/// Prevents double updates by tracking whether a change came from the text editor itself.
		/// </summary>
		private static readonly ICollection<TextEditor> contentIndexPositionChangedEditors = new HashSet<TextEditor>();

		private static readonly ICollection<TextEditor> contentIndexChangedEditors = new HashSet<TextEditor>();

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