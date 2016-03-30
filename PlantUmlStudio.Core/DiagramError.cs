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

using System;
using SharpEssentials;

namespace PlantUmlStudio.Core
{
    /// <summary>
    /// Contains information about a diagram error.
    /// </summary>
    public class DiagramError
    {
        /// <summary>
        /// Initializes a new <see cref="DiagramError"/>.
        /// </summary>
        public DiagramError(int lineNumber, string message)
        {
            LineNumber = lineNumber;
            Message = message;
        }

        /// <summary>
        /// The line number the error is on.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Attempts to create a <see cref="DiagramError"/> from a string.
        /// </summary>
        public static Option<DiagramError> TryParse(string error)
        {
            if (error.StartsWith("ERROR"))
            {
                var errorParts = error.Split(new[] { Environment.NewLine },
                                             StringSplitOptions.RemoveEmptyEntries);

                if (errorParts.Length >= 3)
                    return new DiagramError(Int32.Parse(errorParts[1]), errorParts[2]);
            }

            return Option.None<DiagramError>();
        } 
    }
}