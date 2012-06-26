using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
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

			if (!editors.Contains(editor))
			{
				editors.Add(editor);
				documentMap[editor.Document] = editor;
				editor.Document.TextChanged += editor_TextChanged;
			}

			// If the change came from the editor itself, don't update.
			if (!textChangedEditors.Contains(editor))
			{
				contentChangedEditors.Add(editor);
				var newValue = e.NewValue as string;
				editor.Document.Text = newValue;
			}
			else
			{
				textChangedEditors.Remove(editor);
			}
		}

		static void editor_TextChanged(object sender, EventArgs e)
		{
			var document = sender as TextDocument;
			if (document == null)
				return;

			var editor = documentMap[document];
			if (!contentChangedEditors.Contains(editor))
				textChangedEditors.Add(editor);
			else
				contentChangedEditors.Remove(editor);

;			SetContent(editor, document.Text);
		}

		private static readonly ICollection<TextEditor> editors = new HashSet<TextEditor>();

		/// <summary>
		/// Maps documents to their parent editors.
		/// </summary>
		private static readonly IDictionary<TextDocument, TextEditor> documentMap = new Dictionary<TextDocument, TextEditor>();

		/// <summary>
		/// Prevents double updates by tracking whether a change came from the text editor itself.
		/// </summary>
		private static readonly ICollection<TextEditor> textChangedEditors = new HashSet<TextEditor>();

		private static readonly ICollection<TextEditor> contentChangedEditors = new HashSet<TextEditor>();

		#endregion Content

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