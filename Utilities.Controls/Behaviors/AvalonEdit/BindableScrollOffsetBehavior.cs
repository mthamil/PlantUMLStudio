using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// A behavior that allows binding of scroll offset information.
	/// </summary>
	public class BindableScrollOffsetBehavior : Behavior<TextEditor>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
			AssociatedObject.DataContextChanged += textEditor_DataContextChanged;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.TextArea.TextView.ScrollOffsetChanged -= TextView_ScrollOffsetChanged;
			AssociatedObject.DataContextChanged -= textEditor_DataContextChanged;
		}

		void textEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_dataContextChanged = true;
			_lastUpdatedFromControl = false;
			_lastUpdatedFromBinding = false;
		}

		void TextView_ScrollOffsetChanged(object sender, EventArgs e)
		{
			if (!_dataContextChanged)
			{
				if (!_lastUpdatedFromBinding)
				{
					var textView = (TextView)sender;
					if (textView != AssociatedObject.TextArea.TextView)
						return;

					ScrollOffset = textView.ScrollOffset;
					_lastUpdatedFromControl = true;
				}
				else
				{
					_lastUpdatedFromBinding = false;
				}
			}
			else
			{
				_dataContextChanged = false;
			}
		}

		private void UpdateOffset(Vector newValue)
		{
			if (!_lastUpdatedFromControl)
			{
				AssociatedObject.ScrollToHorizontalOffset(newValue.X);
				AssociatedObject.ScrollToVerticalOffset(newValue.Y);

				_lastUpdatedFromBinding = true;
			}
			else
			{
				_lastUpdatedFromControl = false;
			}
		}

		/// <summary>
		/// Gets or sets the scroll offset.
		/// </summary>
		public Vector ScrollOffset
		{
			get { return (Vector)GetValue(ScrollOffsetProperty); }
			set { SetValue(ScrollOffsetProperty, value); }
		}

		/// <summary>
		/// The ScrollOffset property.
		/// </summary>
		public static readonly DependencyProperty ScrollOffsetProperty =
			DependencyProperty.Register(
			"ScrollOffset",
			typeof(Vector),
			typeof(BindableScrollOffsetBehavior),
			new UIPropertyMetadata(new Vector(-1, -1), OnScrollOffsetChanged));

		private static void OnScrollOffsetChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (BindableScrollOffsetBehavior)dependencyObject;
			behavior.UpdateOffset((Vector)e.NewValue);
		}

		private bool _lastUpdatedFromControl;
		private bool _lastUpdatedFromBinding;
		private bool _dataContextChanged;
	}
}