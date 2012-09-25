using System.Windows;
using System.Windows.Controls;

namespace Utilities.Controls.Behaviors.GridSplitterExpanderSupport
{
	class ColumnExpanderSize : ExpanderSize
	{
		public ColumnExpanderSize(Expander expander, Grid parentGrid)
		{
			_expanderColumn = parentGrid.ColumnDefinitions[Grid.GetColumn(expander)];
		}

		#region Overrides of ExpanderSize

		public override GridLength DimensionSize
		{
			get { return _expanderColumn.Width; }
			set { _expanderColumn.Width = value; }
		}

		#endregion

		private readonly ColumnDefinition _expanderColumn;
	}
}