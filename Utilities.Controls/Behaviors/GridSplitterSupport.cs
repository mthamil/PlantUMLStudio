using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
			if (DesignerProperties.GetIsInDesignMode(dependencyObject))
				return;

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
					if (expander.ExpandDirection == ExpandDirection.Left || expander.ExpandDirection == ExpandDirection.Right)
						behavior = new ColumnGridSplitterSupportBehavior(expander, parentGrid);
					else
						behavior = new RowGridSplitterSupportBehavior(expander, parentGrid);

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

		abstract class GridSplitterSupportBehavior : IDisposable
		{
			protected GridSplitterSupportBehavior(Expander expander, Grid parentGrid)
			{
				_parentGrid = parentGrid;
				_expander = expander;

				_parentGrid.Loaded += parentGrid_Loaded;

				_expander.Initialized += expander_Initialized;
				_expander.Expanded += expander_Expanded;
				_expander.Collapsed += expander_Collapsed;
			}

			protected abstract GridLength DimensionSize { get; set; }

			void expander_Initialized(object sender, EventArgs e)
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
					_dimensionSize = DimensionSize;
					return;
				}

				if (_sizeInitialized)
				{
					if (_sizeManuallyChanged)
						_sizeManuallyChanged = false;
					else
						_newDimensionSize = DimensionSize;
				}
			}

			void expander_Collapsed(object sender, RoutedEventArgs e)
			{
				DimensionSize = GridLength.Auto;

				if (_gridSplitter != null)
					_gridSplitter.IsEnabled = false;

				_sizeManuallyChanged = true;
			}

			void expander_Expanded(object sender, RoutedEventArgs e)
			{
				if (_gridSplitter != null)
					_gridSplitter.IsEnabled = true;

				DimensionSize = _newDimensionSize.HasValue ? _newDimensionSize.Value : _dimensionSize;
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
			private bool _sizeManuallyChanged;

			private GridLength? _newDimensionSize;
			private GridLength _dimensionSize;

			private GridSplitter _gridSplitter;
			private FrameworkElement _element;

			private readonly Grid _parentGrid;
			private readonly Expander _expander;
		}

		class ColumnGridSplitterSupportBehavior : GridSplitterSupportBehavior
		{
			public ColumnGridSplitterSupportBehavior(Expander expander, Grid parentGrid)
				: base(expander, parentGrid) 
			{
				_expanderColumn = parentGrid.ColumnDefinitions[Grid.GetColumn(expander)];
			}

			#region Overrides of GridSplitterSupportBehavior<ColumnDefinition>

			protected override GridLength DimensionSize
			{
				get { return _expanderColumn.Width; }
				set { _expanderColumn.Width = value; }
			}

			#endregion

			private readonly ColumnDefinition _expanderColumn;
		}

		class RowGridSplitterSupportBehavior : GridSplitterSupportBehavior
		{
			public RowGridSplitterSupportBehavior(Expander expander, Grid parentGrid)
				: base(expander, parentGrid)
			{
				_expanderRow = parentGrid.RowDefinitions[Grid.GetRow(expander)];
			}

			#region Overrides of GridSplitterSupportBehavior<ColumnDefinition>

			protected override GridLength DimensionSize
			{
				get { return _expanderRow.Height; }
				set { _expanderRow.Height = value; }
			}

			#endregion

			private readonly RowDefinition _expanderRow;
		}
	}
}