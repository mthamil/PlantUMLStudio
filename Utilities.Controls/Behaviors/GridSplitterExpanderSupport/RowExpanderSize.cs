using System.Windows;
using System.Windows.Controls;

namespace Utilities.Controls.Behaviors.GridSplitterExpanderSupport
{
	class RowExpanderSize : ExpanderSize
	{
		public RowExpanderSize(Expander expander, Grid parentGrid)
		{
			_expanderRow = parentGrid.RowDefinitions[Grid.GetRow(expander)];
		}

		#region Overrides of ExpanderSize

		public override GridLength DimensionSize
		{
			get { return _expanderRow.Height; }
			set { _expanderRow.Height = value; }
		}

		#endregion

		private readonly RowDefinition _expanderRow;
	}
}