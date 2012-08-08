using System;
using System.Globalization;
using System.Windows.Data;
using Moq;
using Utilities.Controls.Converters;
using Xunit;

namespace Unit.Tests.Utilities.Controls.Converters
{
	public class ComposingConverterTests
	{
		[Fact]
		public void Test_Convert()
		{
			// Arrange.
			var converter1 = new Mock<IValueConverter>();
			converter1.Setup(c => c.Convert(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<CultureInfo>()))
				.Returns("first");

			var converter2 = new Mock<IValueConverter>();
			converter2.Setup(c => c.Convert(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<CultureInfo>()))
				.Returns("second");

			converter.Converters.Add(converter1.Object);
			converter.Converters.Add(converter2.Object);

			// Act.
			var converted = converter.Convert("initial", typeof(int), "param", CultureInfo.InvariantCulture);

			// Assert.
			converter1.Verify(c => c.Convert("initial", typeof(int), "param", It.IsAny<CultureInfo>()));
			converter2.Verify(c => c.Convert("first", typeof(int), "param", It.IsAny<CultureInfo>()));
			Assert.Equal("second", converted);
		}

		[Fact]
		public void Test_ConvertBack()
		{
			// Arrange.
			var converter1 = new Mock<IValueConverter>();
			converter1.Setup(c => c.ConvertBack(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<CultureInfo>()))
				.Returns("first");

			var converter2 = new Mock<IValueConverter>();
			converter2.Setup(c => c.ConvertBack(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<CultureInfo>()))
				.Returns("second");

			converter.Converters.Add(converter1.Object);
			converter.Converters.Add(converter2.Object);

			// Act.
			var converted = converter.ConvertBack("initial", typeof(int), "param", CultureInfo.InvariantCulture);

			// Assert.
			converter1.Verify(c => c.ConvertBack("second", typeof(int), "param", It.IsAny<CultureInfo>()));
			converter2.Verify(c => c.ConvertBack("initial", typeof(int), "param", It.IsAny<CultureInfo>()));
			Assert.Equal("first", converted);
		}

		private readonly ComposingConverter converter = new ComposingConverter();
	}
}