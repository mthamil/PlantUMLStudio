using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Allows behavior that supports the use of a GridSplitter in conjunction
	/// with an Expander without required the use of GridLength.Auto.
	/// </summary>
	public static class GridSplitterSupport
	{
		/// <summary>
		/// Gets whether GridSplitter support is enabled.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(Expander))]
		public static bool GetIsEnabled(Expander element)
		{
			return (bool)element.GetValue(IsEnabledProperty);
		}

		/// <summary>
		/// Sets whether GridSplitter support is enabled.
		/// </summary>
		public static void SetIsEnabled(Expander element, bool value)
		{
			element.SetValue(IsEnabledProperty, value);
		}

		/// <summary>
		/// The IsGridSplitterSupportEnabled property.
		/// </summary>
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached(
			"IsEnabled",
			typeof(bool),
			typeof(GridSplitterSupport),
			new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

		private static void OnIsEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var expander = dependencyObject as Expander;
			if (expander == null)
				return;

			var parentGrid = expander.Parent as Grid;
			if (parentGrid == null)
				return;

			bool newValue = (bool)e.NewValue;

			GridSplitterSupportBehavior behavior;
			if (!enabledBehaviors.TryGetValue(expander, out behavior))
			{
				if (newValue)
				{
					behavior = new GridSplitterSupportBehavior(expander, parentGrid);
					enabledBehaviors[expander] = behavior;
				}
			}
			else
			{
				if (!newValue)
				{
					enabledBehaviors.Remove(expander);
					behavior.Dispose();
				}
			}
		}

		private static readonly IDictionary<Expander, GridSplitterSupportBehavior> enabledBehaviors = new Dictionary<Expander, GridSplitterSupportBehavior>();

		class GridSplitterSupportBehavior : IDisposable
		{
			public GridSplitterSupportBehavior(Expander expander, Grid parentGrid)
			{
				_parentGrid = parentGrid;
				_expander = expander;
				_expanderColumn = _parentGrid.ColumnDefinitions[Grid.GetColumn(_expander)];

				_parentGrid.Loaded += parentGrid_Loaded;

				_expander.Initialized += expander_Initialized;
				_expander.Expanded += expander_Expanded;
				_expander.Collapsed += expander_Collapsed;
			}

			void expander_Initialized(object sender, System.EventArgs e)
			{
				_element = (FrameworkElement)_expander.Content;
				_element.SizeChanged += element_SizeChanged;
			}

			void parentGrid_Loaded(object sender, RoutedEventArgs e)
			{
				_gridSplitter = _parentGrid.Children
					.Cast<UIElement>()
					.OfType<GridSplitter>()
					.FirstOrDefault();
			}

			void element_SizeChanged(object sender, SizeChangedEventArgs e)
			{
				if (!_sizeInitialized)
				{
					_sizeInitialized = true;
					_width = _expanderColumn.Width;
					return;
				}

				if (_sizeInitialized)
				{
					if (_sizeManuallyChanged)
						_sizeManuallyChanged = false;
					else
						_newWidth = _expanderColumn.Width;
				}
			}

			void expander_Collapsed(object sender, RoutedEventArgs e)
			{
				_expanderColumn.Width = GridLength.Auto;

				if (_gridSplitter != null)
					_gridSplitter.IsEnabled = false;

				_sizeManuallyChanged = true;
			}

			void expander_Expanded(object sender, RoutedEventArgs e)
			{
				if (_gridSplitter != null)
					_gridSplitter.IsEnabled = true;

				_expanderColumn.Width = _newWidth.HasValue ? _newWidth.Value : _width;
			}

			#region Implementation of IDisposable

			public void Dispose()
			{
				_parentGrid.Loaded -= parentGrid_Loaded;

				_expander.Initialized -= expander_Initialized;
				_expander.Expanded -= expander_Expanded;
				_expander.Collapsed -= expander_Collapsed;

				_element.SizeChanged -= element_SizeChanged;
			}

			#endregion

			private bool _sizeInitialized;
			private GridLength? _newWidth;
			private bool _sizeManuallyChanged;
			
			private GridSplitter _gridSplitter;
			private FrameworkElement _element;

			private GridLength _width;

			private readonly Grid _parentGrid;
			private readonly Expander _expander;
			private readonly ColumnDefinition _expanderColumn;
		}
	}
}