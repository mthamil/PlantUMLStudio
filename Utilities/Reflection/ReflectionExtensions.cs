using System;

namespace Utilities.Reflection
{
	/// <summary>
	/// Contains extension methods for reflection types.
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// If a type is a primitive such as int, returns its default, otherwise
		/// null is returned.
		/// </summary>
		/// <param name="type">The type to create a value for</param>
		/// <returns>A default value for the type</returns>
		public static object GetDefaultValue(this Type type)
		{
			if (type.IsValueType && type != voidType)	// can't create an instance of Void
				return Activator.CreateInstance(type);

			return null;
		}
		private static readonly Type voidType = typeof(void);
	}
}