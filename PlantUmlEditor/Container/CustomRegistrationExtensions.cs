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
			return registration.WithProperty(Reflect.PropertyOf(property).Name, propertyValue);
		}
	}
}