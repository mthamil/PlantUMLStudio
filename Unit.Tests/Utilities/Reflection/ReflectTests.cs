using System;
using System.Reflection;
using Utilities.Reflection;
using Xunit;

namespace Unit.Tests.Utilities.Reflection
{
	public class ReflectTests
	{
		[Fact]
		public void Test_PropertyOf_ValueType()
		{
			// Act.
			PropertyInfo property = Reflect.PropertyOf<TestType, int>(t => t.IntProperty);

			// Assert.
			Assert.Equal("IntProperty", property.Name);
			Assert.Equal(typeof(Int32), property.PropertyType);
			Assert.Equal(typeof(TestType), property.DeclaringType);
		}

		[Fact]
		public void Test_PropertyOf_ReferenceType()
		{
			// Act.
			PropertyInfo property = Reflect.PropertyOf<TestType, string>(t => t.StringProperty);

			// Assert.
			Assert.Equal("StringProperty", property.Name);
			Assert.Equal(typeof(String), property.PropertyType);
			Assert.Equal(typeof(TestType), property.DeclaringType);
		}

		[Fact]
		public void Test_PropertyOf_Method()
		{
			Assert.Throws<ArgumentException>(() => Reflect.PropertyOf<TestType, string>(t => t.ToString()));
		}

		[Fact]
		public void Test_PropertyOf_ConstantValue()
		{
			Assert.Throws<ArgumentException>(() => Reflect.PropertyOf<TestType, int>(t => 1));
		}

		[Fact]
		public void Test_MethodOf_HasReturnValue()
		{
			// Act.
			MethodInfo method = Reflect.MethodOf<TestType, string>(t => t.ToString());

			// Assert.
			Assert.Equal("ToString", method.Name);
			Assert.Equal(typeof(string), method.ReturnType);
			Assert.Equal(typeof(object), method.DeclaringType);
		}

		[Fact]
		public void Test_MethodOf_VoidReturn()
		{
			// Act.
			MethodInfo method = Reflect.MethodOf<TestType>(t => t.VoidReturning());

			// Assert.
			Assert.Equal("VoidReturning", method.Name);
			Assert.Equal(typeof(void), method.ReturnType);
			Assert.Equal(typeof(TestType), method.DeclaringType);
			Assert.Equal(0, method.GetParameters().Length);
		}

		[Fact]
		public void Test_MethodOf_Overload()
		{
			// Act.
			MethodInfo method = Reflect.MethodOf<TestType>(t => t.VoidReturning(0));

			// Assert.
			Assert.Equal("VoidReturning", method.Name);
			Assert.Equal(typeof(void), method.ReturnType);
			Assert.Equal(typeof(TestType), method.DeclaringType);
			Assert.Equal(1, method.GetParameters().Length);
		}

		[Fact]
		public void Test_MethodOf_Property()
		{
			Assert.Throws<ArgumentException>(() => Reflect.MethodOf<TestType, string>(t => t.StringProperty));
		}

		[Fact]
		public void Test_MethodOf_ConstantValue()
		{
			Assert.Throws<ArgumentException>(() => Reflect.MethodOf<TestType, int>(t => 1));
		}

		private class TestType
		{
			public int IntProperty { get; set; }
			public string StringProperty { get; set; }
			public void VoidReturning() {}
			public void VoidReturning(int value) { }
		}
	}
}