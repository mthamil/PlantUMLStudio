using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Manages a TextEditor's bindable selection start and selection length properties.
	/// </summary>
	public class BindableSelectionBehavior : Behavior<TextEditor>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.TextArea.SelectionChanged += TextArea_SelectionChanged;
			AssociatedObject.DataContextChanged += editor_DataContextChanged;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.TextArea.SelectionChanged -= TextArea_SelectionChanged;
			AssociatedObject.DataContextChanged -= editor_DataContextChanged;
		}

		void editor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_dataContextChanged = true;
			_lastStartUpdateFromBinding = false;
			_lastStartUpdateFromControl = false;
			_lastLengthUpdateFromBinding = false;
			_lastLengthUpdateFromControl = false;
		}

		void TextArea_SelectionChanged(object sender, EventArgs e)
		{
			if (!_dataContextChanged)
			{
				if (!_lastStartUpdateFromBinding)
				{
					_lastStartUpdateFromControl = true;
					SelectionStart = AssociatedObject.SelectionStart;
				}
				else
				{
					_lastStartUpdateFromBinding = false;
				}

				if (!_lastLengthUpdateFromBinding)
				{
					_lastLengthUpdateFromControl = true;
					SelectionLength = AssociatedObject.SelectionLength;
				}
				else
				{
					_lastLengthUpdateFromBinding = false;
				}
			}
			else
			{
				_dataContextChanged = false;
			}
		}

		internal void UpdateSelectionStart(int selectionStart)
		{
			// If the change came from the editor itself, don't update.
			if (!_lastStartUpdateFromControl)
			{
				_lastStartUpdateFromBinding = true;
				if (selectionStart > -1)
					AssociatedObject.SelectionStart = selectionStart;
			}
			else
			{
				_lastStartUpdateFromControl = false;
			}
		}

		/// <summary>
		/// Gets or sets the selection start.
		/// </summary>
		public int SelectionStart
		{
			get { return (int)GetValue(SelectionStartProperty); }
			set { SetValue(SelectionStartProperty, value); }
		}

		/// <summary>
		/// The BindableSelectionStart property.
		/// </summary>
		public static readonly DependencyProperty SelectionStartProperty =
			DependencyProperty.Register(
			"SelectionStart",
			typeof(int),
			typeof(BindableSelectionBehavior),
			new UIPropertyMetadata(-1, OnSelectionStartChanged));

		private static void OnSelectionStartChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (BindableSelectionBehavior)dependencyObject;
			behavior.UpdateSelectionStart((int)e.NewValue);
		}

		internal void UpdateSelectionLength(int selectionLength)
		{
			// If the change came from the editor itself, don't update.
			if (!_lastLengthUpdateFromControl)
			{
				_lastLengthUpdateFromBinding = true;
				if (selectionLength > -1)
					AssociatedObject.SelectionLength = selectionLength;
			}
			else
			{
				_lastLengthUpdateFromControl = false;
			}
		}

		/// <summary>
		/// Gets or sets the selection length.
		/// </summary>
		public int SelectionLength
		{
			get { return (int)GetValue(SelectionLengthProperty); }
			set { SetValue(SelectionLengthProperty, value); }
		}

		/// <summary>
		/// The SelectionLength property.
		/// </summary>
		public static readonly DependencyProperty SelectionLengthProperty =
			DependencyProperty.Register(
			"SelectionLength",
			typeof(int),
			typeof(BindableSelectionBehavior),
			new UIPropertyMetadata(-1, OnSelectionLengthChanged));

		private static void OnSelectionLengthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (BindableSelectionBehavior)dependencyObject;
			behavior.UpdateSelectionLength((int)e.NewValue);
		}

		private bool _lastStartUpdateFromControl;
		private bool _lastStartUpdateFromBinding;

		private bool _lastLengthUpdateFromControl;
		private bool _lastLengthUpdateFromBinding;

		private bool _dataContextChanged;
	}
}