//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
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