//  PlantUML Studio
//  Copyright 2015 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - omaralzabir@gmail.com (original author)
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
using SharpEssentials;

namespace PlantUmlStudio.Core.Utilities
{
    /// <summary>
    /// Provides extension methods for objects that implement <see cref="ISupportInitialize"/>.
    /// </summary>
    public static class InitializationSupportExtensions
    {
        /// <summary>
        /// Begins a component initialization scope.
        /// </summary>
        /// <param name="component">The object to initialize.</param>
        public static IDisposable BeginInitialize(this ISupportInitialize component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            return new InitializationToken(component);
        }

        class InitializationToken : DisposableBase
        {
            private readonly ISupportInitialize _component;

            public InitializationToken(ISupportInitialize component)
            {
                _component = component;
                _component.BeginInit();
            }

            protected override void OnDisposing()
            {
                _component.EndInit();
            }
        }
    }
}