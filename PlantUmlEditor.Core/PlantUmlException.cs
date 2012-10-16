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
using System.Runtime.Serialization;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Represents PlantUML errors.
	/// </summary>
	[Serializable]
	public class PlantUmlException : Exception
	{
		/// <see cref="Exception()"/>
		public PlantUmlException() { }

		/// <see cref="Exception(string)"/>
		public PlantUmlException(string message)
			: base(message) { }

		/// <see cref="Exception(string, Exception)"/>
		public PlantUmlException(string message, Exception inner) 
			: base(message, inner) { }

		/// <see cref="Exception(SerializationInfo, StreamingContext)"/>
		protected PlantUmlException(SerializationInfo info, StreamingContext context) 
			: base(info, context) { }
	}
}