using Utilities.Reflection;
using Xunit;

namespace Tests.Unit.Utilities.Reflection
{
	public class ReflectionExtensionsTests
	{
		[Fact]
		public void Test_GetDefaultValue_ReferenceType()
		{
			// Act.
			object defaultValue = typeof(string).GetDefaultValue();

			// Assert.
			Assert.Equal(null, defaultValue);
		}

		[Fact]
		public void Test_GetDefaultValue_Integer()
		{
			// Act.
			object defaultValue = typeof(int).GetDefaultValue();

			// Assert.
			Assert.Equal(0, defaultValue);
		}

		[Fact]
		public void Test_GetDefaultValue_Boolean()
		{
			// Act.
			object defaultValue = typeof(bool).GetDefaultValue();

			// Assert.
			Assert.Equal(false, defaultValue);
		}
	}
}
