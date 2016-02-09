using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Xunit;
using PlantUmlStudio.Controls.Behaviors.AvalonEdit.Folding;

namespace Tests.Unit.PlantUmlStudio.Controls.Behaviors.AvalonEdit
{
    public class FoldingManagerExtensionsTests
    {
        [Fact]
        public void Test_UpdateFoldings_Preserves_Foldings()
        {
            // Arrange.
            var doc = new TextDocument(@"
                This is some test text.
                It needs to span multiple
                lines in order to demonstrate 
                folding.
            ");

            var manager = new FoldingManager(doc);

            // Act.
            manager.UpdateFoldings(new[]
            {
                new NewFolding(17, 57) { DefaultClosed = true },
                new NewFolding(101, 148) { DefaultClosed = true}
            });

            // Assert.
            Assert.True(manager.AllFoldings.All(f => f.IsFolded));
        }
    }
}