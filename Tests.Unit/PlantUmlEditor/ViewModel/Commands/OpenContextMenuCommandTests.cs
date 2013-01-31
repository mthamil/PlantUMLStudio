using System.Windows;
using System.Windows.Controls;
using Utilities.Controls.Commands;
using Xunit;

namespace Tests.Unit.PlantUmlEditor.ViewModel.Commands
{
	public class OpenContextMenuCommandTests
	{
		[Fact]
		public void Test_Execute()
		{
			// Act.
			_command.Execute(_element);

			// Assert.
			Assert.Equal(_element, _element.ContextMenu.PlacementTarget);
			Assert.True(_element.ContextMenu.IsOpen);
		}

		private readonly OpenContextMenuCommand _command = new OpenContextMenuCommand();

		private readonly FrameworkElement _element = new FrameworkElement
		{
			ContextMenu = new ContextMenu()
		};
	}
}