﻿//  PlantUML Editor 2
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
using System.Threading.Tasks;
using PlantUmlEditor.Core.Dependencies.Update;

namespace PlantUmlEditor.Core.Dependencies
{
	/// <summary>
	/// Represents a software component dependency.
	/// </summary>
	public interface IExternalComponent : IComponentUpdateChecker
	{
		/// <summary>
		/// A dependency's name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Asynchronously retrieves a dependency's current version.
		/// </summary>
		/// <returns>A string representing the version</returns>
		Task<string> GetCurrentVersionAsync();
	}
}