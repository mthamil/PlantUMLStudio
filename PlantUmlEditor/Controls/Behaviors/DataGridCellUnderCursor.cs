using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PlantUmlEditor.Controls.Behaviors
{
	/// <summary>
	/// 
	/// </summary>
	public static class DataGridCellUnderCursor
	{
		/// <summary>
		/// Gets the property value.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(DataGrid))]
		public static object GetCellUnderCursor(DataGrid dataGrid)
		{
			return dataGrid.GetValue(CellUnderCursorProperty);
		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		public static void SetCellUnderCursor(DataGrid dataGrid, object value)
		{
			dataGrid.SetValue(CellUnderCursorProperty, value);
		}

		/// <summary>
		/// The CellUnderCursor dependency property.
		/// </summary>
		public static readonly DependencyProperty CellUnderCursorProperty =
			DependencyProperty.RegisterAttached(
			"CellUnderCursor",
			typeof(object),
			typeof(DataGridCellUnderCursor),
			new UIPropertyMetadata(null, OnCellUnderCursorChanged));

		private static void OnCellUnderCursorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			DataGrid dataGrid = dependencyObject as DataGrid;
			if (dataGrid == null)
				return;

			if (!Equals(e.NewValue, e.OldValue))
			{
				var newValue = e.NewValue;

				// Remove any previous filter.
				CellUnderCursorBehavior oldBehavior;
				if (behaviors.TryGetValue(dataGrid, out oldBehavior))
				{
					oldBehavior.Unregister();
					behaviors.Remove(dataGrid);
				}

				if (newValue != null)
					behaviors.Add(dataGrid, new CellUnderCursorBehavior(dataGrid));
			}
		}

		private class CellUnderCursorBehavior
		{
			public CellUnderCursorBehavior(DataGrid dataGrid)
			{
				dataGrid.InitializingNewItem += new InitializingNewItemEventHandler(dataGrid_InitializingNewItem);
				dataGrid.IsMouseDirectlyOverChanged += new DependencyPropertyChangedEventHandler(dataGrid_IsMouseDirectlyOverChanged);
			}

			void dataGrid_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
			{
			}

			void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
			{
				
			}


			public void Unregister()
			{
					
			}
		}

		private static readonly IDictionary<DataGrid, CellUnderCursorBehavior> behaviors = new ConcurrentDictionary<DataGrid, CellUnderCursorBehavior>();
	}
}