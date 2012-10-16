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
using System.Linq.Expressions;
using Autofac;
using Autofac.Builder;
using Utilities.Reflection;

namespace PlantUmlEditor.Container
{
	/// <summary>
	/// Provides additional registration helpers.
	/// </summary>
	public static class CustomRegistrationExtensions
	{
		/// <summary>
		/// Configures an explicit value for a property.
		/// 
		/// </summary>
		/// <typeparam name="TLimit">Registration limit type.</typeparam>
		/// <typeparam name="TStyle">Registration style.</typeparam>
		/// <typeparam name="TReflectionActivatorData">Activator data type.</typeparam>
		/// <typeparam name="TValue">The type of the property being set</typeparam>
		/// <param name="registration">Registration to set property on.</param>
		/// <param name="property">An expression referencing a property on the target type.</param>
		/// <param name="propertyValue">Value to supply to the property.</param>
		/// <returns>
		/// A registration builder allowing further configuration of the component.
		/// </returns>
		public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithProperty<TLimit, TReflectionActivatorData, TStyle, TValue>(
			this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration, Expression<Func<TLimit, TValue>> property, TValue propertyValue)
			where TReflectionActivatorData : ReflectionActivatorData
		{
			return registration.WithProperty(Reflect.PropertyOf(typeof(TLimit), property).Name, propertyValue);
		}
	}
}