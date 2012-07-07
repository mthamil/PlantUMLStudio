using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Moq;
using Utilities.Controls.MultiKey;
using Xunit;
using Xunit.Extensions;

namespace Unit.Tests.Utilities.Controls.MultiKey
{
	public class MultiKeyGestureConverterTests
	{
		[Theory]
		[InlineData("CTRL+A", new [] { Key.A }, new [] { ModifierKeys.Control })]
		[InlineData("CTRL+A,K", new[] { Key.A, Key.K }, new[] { ModifierKeys.Control, ModifierKeys.None })]
		[InlineData("CTRL+A,SHIFT+K", new[] { Key.A, Key.K }, new[] { ModifierKeys.Control, ModifierKeys.Shift })]
		public void Test_ConvertFrom(string input, Key[] expectedKeys, ModifierKeys[] expectedModifiers)
		{
			// Arrange.
			var converter = new MultiKeyGestureConverter();

			// Act.
			var gesture = (MultiKeyGesture)converter.ConvertFrom(new Mock<ITypeDescriptorContext>().Object, CultureInfo.InvariantCulture, input);

			// Assert.
			Assert.NotNull(gesture);
			Assert.Equal(expectedKeys, gesture.Keys.ToArray());
			Assert.Equal(expectedModifiers, gesture.Modifiers.ToArray());
		}
	}
}