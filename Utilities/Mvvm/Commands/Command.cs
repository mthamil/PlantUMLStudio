//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.ComponentModel;
using Utilities.Mvvm.Commands.Builder;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// Contains factory methods for commands.
	/// </summary>
	public static class Command
	{
		/// <summary>
		/// Begins creating a command for a given object.
		/// </summary>
		/// <typeparam name="TSource">The type of object that owns the command</typeparam>
		/// <param name="source">The object that owns the command</param>
		/// <returns>A new command builder</returns>
		public static ICommandBuilder<TSource> For<TSource>(TSource source) where TSource : INotifyPropertyChanged
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return new CommandBuilder<TSource>(source);
		}
	}
}