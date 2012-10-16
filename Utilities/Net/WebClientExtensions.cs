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