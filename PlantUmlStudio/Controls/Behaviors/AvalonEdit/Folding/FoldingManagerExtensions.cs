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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace PlantUmlStudio.Controls.Behaviors.AvalonEdit.Folding
{
    static class FoldingManagerExtensions
    {
        /// <summary>
        /// Creates new foldings and updates a <see cref="FoldingManager"/> with them.
        /// </summary>
        public static IEnumerable<NewFolding> GenerateFoldings(this FoldingManager manager, TextDocument document, IFoldingStrategy strategy)
        {
            int firstErrorOffset;
            var newFoldings = strategy.CreateNewFoldings(document, out firstErrorOffset).ToList();
            manager.UpdateFoldings(newFoldings);
            return newFoldings;
        }

        /// <summary>
        /// Updates the foldings in this <see cref="FoldingManager"/> using the given new foldings.
        /// This method will try to correlate new foldings with existing foldings and will attempt
        /// to keep the state (<see cref="FoldingSection.IsFolded"/>) of existing foldings.
        /// </summary>
        /// <remarks>
        /// Based on original code from: https://github.com/icsharpcode/AvalonEdit/blob/master/ICSharpCode.AvalonEdit/Folding/FoldingManager.cs
        /// This custom version was created in order to fix an issue where only the first collapsed folding was preserved.
        /// See: https://github.com/icsharpcode/AvalonEdit/issues/86
        /// </remarks>
        public static void UpdateFoldings(this FoldingManager manager, IEnumerable<NewFolding> newFoldings)
        {
            if (newFoldings == null)
                throw new ArgumentNullException(nameof(newFoldings));

            var oldFoldings = manager.AllFoldings.ToArray();
            int oldFoldingIndex = 0;
            int previousStartOffset = 0;

            // Merge new foldings into old foldings so that sections keep being collapsed.
            // Both oldFoldings and newFoldings are sorted by start offset.
            foreach (var newFolding in newFoldings)
            {
                // ensure newFoldings are sorted correctly
                if (newFolding.StartOffset < previousStartOffset)
                    throw new ArgumentException(nameof(newFoldings) + " must be sorted by start offset");

                previousStartOffset = newFolding.StartOffset;

                if (newFolding.StartOffset == newFolding.EndOffset)
                    continue; // ignore zero-length foldings

                // Remove old foldings that were skipped.
                while (oldFoldingIndex < oldFoldings.Length && newFolding.StartOffset > oldFoldings[oldFoldingIndex].StartOffset)
                    manager.RemoveFolding(oldFoldings[oldFoldingIndex++]);

                // Reuse current folding if its matching.
                FoldingSection section;
                if (oldFoldingIndex < oldFoldings.Length && newFolding.StartOffset == oldFoldings[oldFoldingIndex].StartOffset)
                {
                    section = oldFoldings[oldFoldingIndex++];
                    section.Length = newFolding.EndOffset - newFolding.StartOffset;
                }
                else
                {
                    // No matching current folding; create a new one.
                    section = manager.CreateFolding(newFolding.StartOffset, newFolding.EndOffset);
                    section.IsFolded = newFolding.DefaultClosed;
                    section.Tag = newFolding;
                }
                section.Title = newFolding.Name;
            }

            // Remove all outstanding old foldings.
            while (oldFoldingIndex < oldFoldings.Length)
            {
                var oldSection = oldFoldings[oldFoldingIndex++];
                manager.RemoveFolding(oldSection);
            }
        }
    }
}