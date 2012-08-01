using System;
using System.Net;

namespace Utilities.Net
{
	/// <summary>
	/// Provides extensions for the WebClient class.
	/// </summary>
	public static class WebClientExtensions
	{
		/// <summary>
		/// Provides access to asynchronous WebClient operations.
		/// </summary>
		/// <param name="webClient">The WebClient being extended</param>
		/// <returns>A web client that can be used for asynchronous operations</returns>
		public static IAsyncWebClient Async(this WebClient webClient)
		{
			return AsyncWebClientFactory(webClient);
		}

		internal static Func<WebClient, IAsyncWebClient> AsyncWebClientFactory = wc => new AsyncWebClient(wc);
	}
}