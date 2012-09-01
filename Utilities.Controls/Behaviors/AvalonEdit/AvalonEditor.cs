using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;

namespace Utilities.Controls.Behaviors.AvalonEdit
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

			bool firstUpdate = false;
			TextEditorContentIndexBehavior contentIndexBehavior;
			if (!contentIndexBehaviors.TryGetValue(editor, out contentIndexBehavior))
			{
				contentIndexBehavior = new TextEditorContentIndexBehavior(editor);
				contentIndexBehaviors[editor] = contentIndexBehavior;
				firstUpdate = true;
			}

			contentIndexBehavior.UpdateIndex((int)e.NewValue, firstUpdate);
		}

		private static readonly IDictionary<TextEditor, TextEditorContentIndexBehavior> contentIndexBehaviors = new Dictionary<TextEditor, TextEditorContentIndexBehavior>();

		#endregion ContentIndex

		#region Scrolloffset

		/// <summary>
		/// Gets the scroll offset.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextEditor))]
		public static Vector GetScrollOffset(TextEditor textEditor)
		{
			return (Vector)textEditor.GetValue(ScrollOffsetProperty);
		}

		/// <summary>
		/// Sets the scroll offset.
		/// </summary>
		public static void SetScrollOffset(TextEditor textEditor, Vector value)
		{
			textEditor.SetValue(ScrollOffsetProperty, value);
		}

		/// <summary>
		/// The ScrollOffset property.
		/// </summary>
		public static readonly DependencyProperty ScrollOffsetProperty =
			DependencyProperty.RegisterAttached(
			"ScrollOffset",
			typeof(Vector),
			typeof(AvalonEditor),
			new UIPropertyMetadata(new Vector(-1, -1), OnScrollOffsetChanged));

		private static void OnScrollOffsetChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var editor = dependencyObject as TextEditor;
			if (editor == null)
				return;

			BindableScrollOffsetBehavior behavior;
			if (!scrollOffsetBehaviors.TryGetValue(editor, out behavior))
			{
				behavior = new BindableScrollOffsetBehavior(editor);
				scrollOffsetBehaviors[editor] = behavior;
			}

			behavior.UpdateOffset((Vector)e.NewValue);
		}

		private static readonly IDictionary<TextEditor, BindableScrollOffsetBehavior> scrollOffsetBehaviors = new Dictionary<TextEditor, BindableScrollOffsetBehavior>();

		#endregion ScrollOffset

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

			FoldingStrategyBehavior foldingStrategyBehavior;
			if (!foldingBehaviors.TryGetValue(editor, out foldingStrategyBehavior))
			{
				var foldingBehavior = new FoldingStrategyBehavior(editor, foldingStrategy);
				foldingBehaviors[editor] = foldingBehavior;
			}
			else
			{
				if (foldingStrategy == null)
				{
					foldingBehaviors.Remove(editor);
				}
			}

		}

		private static readonly IDictionary<TextEditor, FoldingStrategyBehavior> foldingBehaviors = new Dictionary<TextEditor, FoldingStrategyBehavior>();

		#endregion FoldingStrategy

		#region BindableSelectionStart

		/// <summary>
		/// Gets the selection start.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextEditor))]
		public static int GetBindableSelectionStart(TextEditor textEditor)
		{
			return (int)textEditor.GetValue(BindableSelectionStartProperty);
		}

		/// <summary>
		/// Sets the selection start.
		/// </summary>
		public static void SetBindableSelectionStart(TextEditor textEditor, int value)
		{
			textEditor.SetValue(BindableSelectionStartProperty, value);
		}

		/// <summary>
		/// The BindableSelectionStart property.
		/// </summary>
		public static readonly DependencyProperty BindableSelectionStartProperty =
			DependencyProperty.RegisterAttached(
			"BindableSelectionStart",
			typeof(int),
			typeof(AvalonEditor),
			new UIPropertyMetadata(-1, OnBindableSelectionStartChanged));

		private static void OnBindableSelectionStartChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var editor = dependencyObject as TextEditor;
			if (editor == null)
				return;

			BindableSelectionStartBehavior behavior;
			if (!selectionStartBehaviors.TryGetValue(editor, out behavior))
			{
				behavior = new BindableSelectionStartBehavior(editor);
				selectionStartBehaviors[editor] = behavior;
			}

			behavior.UpdateSelectionStart((int)e.NewValue);
		}

		private static readonly IDictionary<TextEditor, BindableSelectionStartBehavior> selectionStartBehaviors = new Dictionary<TextEditor, BindableSelectionStartBehavior>();

		#endregion BindableSelectionStart

		#region BindableSelectionLength

		/// <summary>
		/// Sets the selection length.
		/// </summary>
		public static void SetBindableSelectionLength(TextEditor textEditor, int value)
		{
			textEditor.SetValue(BindableSelectionLengthProperty, value);
		}

		/// <summary>
		/// Gets the selection length.
		/// </summary>
		public static int GetBindableSelectionLength(TextEditor textEditor)
		{
			return (int)textEditor.GetValue(BindableSelectionLengthProperty);
		}

		public static readonly DependencyProperty BindableSelectionLengthProperty =
			DependencyProperty.RegisterAttached(
			"BindableSelectionLength", 
			typeof(int), 
			typeof(AvalonEditor),
			new UIPropertyMetadata(-1, OnBindableSelectionLengthChanged));

		private static void OnBindableSelectionLengthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var editor = dependencyObject as TextEditor;
			if (editor == null)
				return;

			BindableSelectionLengthBehavior behavior;
			if (!selectionLengthBehaviors.TryGetValue(editor, out behavior))
			{
				behavior = new BindableSelectionLengthBehavior(editor);
				selectionLengthBehaviors[editor] = behavior;
			}

			behavior.UpdateSelectionLength((int)e.NewValue);
		}

		private static readonly IDictionary<TextEditor, BindableSelectionLengthBehavior> selectionLengthBehaviors = new Dictionary<TextEditor, BindableSelectionLengthBehavior>();

		#endregion BindableSelectionLength
	}
}