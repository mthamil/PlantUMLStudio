using System;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	internal class BindableScrollOffsetBehavior
	{
		public BindableScrollOffsetBehavior(TextEditor textEditor)
		{
			_textEditor = textEditor;

			_textEditor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
			_textEditor.DataContextChanged += textEditor_DataContextChanged;
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
					if (textView != _textEditor.TextArea.TextView)
						return;

					AvalonEditor.SetScrollOffset(_textEditor, textView.ScrollOffset);
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

		public void UpdateOffset(Vector newValue)
		{
			if (!_lastUpdatedFromControl)
			{
				_textEditor.ScrollToHorizontalOffset(newValue.X);
				_textEditor.ScrollToVerticalOffset(newValue.Y);

				_lastUpdatedFromBinding = true;
			}
			else
			{
				_lastUpdatedFromControl = false;
			}
		}

		private bool _lastUpdatedFromControl;
		private bool _lastUpdatedFromBinding;
		private bool _dataContextChanged;

		private readonly TextEditor _textEditor;
	}
}