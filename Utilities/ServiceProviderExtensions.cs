using System;

namespace Utilities
{
	/// <summary>
	/// Provides extension methods for IServiceProvider.
	/// </summary>
	public static class ServiceProviderExtensions
	{
		/// <summary>
		/// Gets the service of a specified type.
		/// </summary>
		/// <typeparam name="TService">The type of service</typeparam>
		/// <param name="serviceProvider">The service provider</param>
		/// <returns>A service of the specified type</returns>
		public static TService GetService<TService>(this IServiceProvider serviceProvider)
		{
			return (TService)serviceProvider.GetService(typeof(TService));
		}
	}
}