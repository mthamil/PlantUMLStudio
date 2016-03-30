//  PlantUML Studio
//  Copyright 2016 Matthew Hamilton - matthamilton@live.com
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using SharpEssentials;

namespace PlantUmlStudio.Core
{
    /// <summary>
    /// Represents the result of compiling a diagram.
    /// </summary>
    public sealed class DiagramResult
    {
        /// <summary>
        /// Initializes a successful result.
        /// </summary>
        public DiagramResult(ImageSource image)
        {
            Image = Option.Some(image);
            Errors = Enumerable.Empty<DiagramError>();
        }

        /// <summary>
        /// Initializes a failed result.
        /// </summary>
        /// <param name="errors"></param>
        public DiagramResult(IEnumerable<DiagramError> errors)
        {
            Image = Option.None<ImageSource>();
            Errors = errors;
        }

        /// <summary>
        /// The rendered diagram image if successful.
        /// </summary>
        public Option<ImageSource> Image { get; }

        /// <summary>
        /// Any errors resulting from diagram compilation.
        /// </summary>
        public IEnumerable<DiagramError> Errors { get; } 
    }
}