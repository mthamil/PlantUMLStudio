using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable content index property.
	/// </summary>
	public class ContentIndexBehavior : Behavior<TextEditor>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.TextArea.Caret.PositionChanged += caret_PositionChanged;
			AssociatedObject.DataContextChanged += editor_DataContextChanged;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.TextArea.Caret.PositionChanged -= caret_PositionChanged;
			AssociatedObject.DataContextChanged -= editor_DataContextChanged;
		}

		internal void UpdateIndex(int index)
		{
			// If the change came from the editor itself, don't update.
			if (!_lastUpdateFromControl)
			{
				if (0 <= index && index <= AssociatedObject.Text.Length) // TODO: Really fix this.
				{
					AssociatedObject.Select(index, 0);
					//_editor.TextArea.Caret.BringCaretToView();
				}

				if (!_isFirstUpdate)
					_lastUpdateFromBinding = true;
			}
			else
			{
				_lastUpdateFromControl = false;
			}

			_isFirstUpdate = false;
		}

		void caret_PositionChanged(object sender, EventArgs e)
		{
			var caret = sender as Caret;
			if (caret == null)
				return;

			if (!_dataContextChanged)
			{
				if (!_lastUpdateFromBinding)
				{
					_lastUpdateFromControl = true;
					ContentIndex = caret.Offset;
				}
				else
				{
					_lastUpdateFromBinding = false;
				}
			}
			else
			{
			    _dataContextChanged = false;
			}
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_dataContextChanged = true;
			_lastUpdateFromControl = false;
			_lastUpdateFromBinding = false;
		}

		/// <summary>
		/// Gets or sets the content index.
		/// </summary>
		public int ContentIndex
		{
			get { return (int)GetValue(ContentIndexProperty); }
			set { SetValue(ContentIndexProperty, value); }
		}

		/// <summary>
		/// The ContentIndex property.
		/// </summary>
		public static readonly DependencyProperty ContentIndexProperty =
			DependencyProperty.Register(
			"ContentIndex",
			typeof(int),
			typeof(ContentIndexBehavior),
			new UIPropertyMetadata(-1, OnContentIndexChanged));

		private static void OnContentIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (ContentIndexBehavior)dependencyObject;
			behavior.UpdateIndex((int)e.NewValue);
		}

		private bool _isFirstUpdate = true;

		private bool _lastUpdateFromControl;
		private bool _lastUpdateFromBinding;
		private bool _dataContextChanged;
	}
}