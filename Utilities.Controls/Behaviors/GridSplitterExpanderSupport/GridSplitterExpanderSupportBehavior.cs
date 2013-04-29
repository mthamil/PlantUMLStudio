//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Utilities.Controls.Behaviors.GridSplitterExpanderSupport
{
	/// <summary>
	/// Behavior that supports the use of a GridSplitter in conjunction
	/// with an Expander without the limitation of having to use GridLength.Auto.
	/// </summary>
	public class GridSplitterExpanderSupportBehavior : Behavior<Expander>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			var parentGrid = AssociatedObject.Parent as Grid;
			if (parentGrid == null)
				return;

			if (AssociatedObject.ExpandDirection == ExpandDirection.Left || AssociatedObject.ExpandDirection == ExpandDirection.Right)
				_expanderSize = new ColumnExpanderSize(AssociatedObject, parentGrid);
			else
				_expanderSize = new RowExpanderSize(AssociatedObject, parentGrid);

			_parentGrid = parentGrid;

			_parentGrid.Loaded += parentGrid_Loaded;

			AssociatedObject.Initialized += expander_Initialized;
			AssociatedObject.Expanded += expander_Expanded;
			AssociatedObject.Collapsed += expander_Collapsed;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			_parentGrid.Loaded -= parentGrid_Loaded;

			AssociatedObject.Initialized -= expander_Initialized;
			AssociatedObject.Expanded -= expander_Expanded;
			AssociatedObject.Collapsed -= expander_Collapsed;

			_expanderContent.SizeChanged -= expanderContent_SizeChanged;
		}

		void expander_Initialized(object sender, EventArgs e)
		{
			_expanderContent = (FrameworkElement)AssociatedObject.Content;
			_expanderContent.SizeChanged += expanderContent_SizeChanged;
		}

		void parentGrid_Loaded(object sender, RoutedEventArgs e)
		{
			_gridSplitter = _parentGrid.Children
				.Cast<UIElement>()
				.OfType<GridSplitter>()
				.FirstOrDefault();
		}

		void expanderContent_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!_sizeInitialized)
			{
				_sizeInitialized = true;
				_dimensionSize = _expanderSize.DimensionSize;
				return;
			}

			if (_sizeInitialized)
			{
				if (_sizeManuallyChanged)
					_sizeManuallyChanged = false;
				else
					_newDimensionSize = _expanderSize.DimensionSize;
			}
		}

		void expander_Collapsed(object sender, RoutedEventArgs e)
		{
			_expanderSize.DimensionSize = GridLength.Auto;

			if (_gridSplitter != null)
				_gridSplitter.IsEnabled = false;

			_sizeManuallyChanged = true;
		}

		void expander_Expanded(object sender, RoutedEventArgs e)
		{
			if (_gridSplitter != null)
				_gridSplitter.IsEnabled = true;

			_expanderSize.DimensionSize = _newDimensionSize.HasValue ? _newDimensionSize.Value : _dimensionSize;
		}

		private bool _sizeInitialized;
		private bool _sizeManuallyChanged;

		private GridLength? _newDimensionSize;
		private GridLength _dimensionSize;

		private GridSplitter _gridSplitter;
		private FrameworkElement _expanderContent;

		private ExpanderSize _expanderSize;
		private Grid _parentGrid;
	}
}