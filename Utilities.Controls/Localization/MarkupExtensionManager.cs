// PlantUML Editor
// Copyright 2013 Matthew Hamilton - matthamilton@live.com
// Copyright 2008 Grant Frisken, Infralution (original author)
// Originally licensed under the CodeProject Open License.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;

namespace Utilities.Controls.Localization
{
    /// <summary>
    /// Defines a class for managing <see cref="ManagedMarkupExtension"/> objects
    /// </summary>
    /// <remarks>
    /// This class provides a single point for updating all markup targets that use the given Markup 
    /// Extension managed by this class.   
    /// </remarks>
    public class MarkupExtensionManager 
    {
        /// <summary>
        /// Initializes a new instance of a manager.
        /// </summary>
        /// <param name="cleanupInterval">
        /// The interval at which to cleanup and remove extensions associated with garbage
        /// collected targets.  This specifies the number of new Markup Extensions that are
        /// created before a cleanup is triggered
        /// </param>
        public MarkupExtensionManager(int cleanupInterval)
        {
            _cleanupInterval = cleanupInterval;
        }

        /// <summary>
        /// Forces the update of all active targets that use the markup extension.
        /// </summary>
        public virtual void UpdateAllTargets()
        {
            foreach (ManagedMarkupExtension extension in _extensions)
            {
                extension.UpdateTargets();
            }
        }

        /// <summary>
        /// All currently active extensions.
        /// </summary>
        public IEnumerable<ManagedMarkupExtension> ActiveExtensions
        {
            get { return _extensions; }
        }

        /// <summary>
        /// Cleans up references to extensions for targets which have been garbage collected.
        /// </summary>
        /// <remarks>
        /// This method is called periodically as new <see cref="ManagedMarkupExtension"/> objects 
        /// are registered to release <see cref="ManagedMarkupExtension"/> objects which are no longer 
        /// required (because their target has been garbage collected).  This method does
        /// not need to be called externally, however it can be useful to call it prior to calling
        /// GC.Collect to verify that objects are being garbage collected correctly.
        /// </remarks>
        public void CleanupInactiveExtensions()
        {
            var newExtensions = new List<ManagedMarkupExtension>(_extensions.Count);
			newExtensions.AddRange(_extensions.Where(extension => extension.IsTargetAlive));
            _extensions = newExtensions;
        }

        /// <summary>
        /// Registers a new extension and remove extensions which reference target objects
        /// that have been garbage collected.
        /// </summary>
        /// <param name="extension">The extension to register</param>
        internal void RegisterExtension(ManagedMarkupExtension extension)
        {
            // Cleanup extensions for target objects which have been garbage collected
            // for performance only do this periodically.
            if (_cleanupCount > _cleanupInterval)
            {
                CleanupInactiveExtensions();
                _cleanupCount = 0;
            }
            _extensions.Add(extension);
            _cleanupCount++;
        }

		/// <summary>
		/// List of active extensions
		/// </summary>
		private List<ManagedMarkupExtension> _extensions = new List<ManagedMarkupExtension>();

		/// <summary>
		/// The number of extensions added since the last cleanup
		/// </summary>
		private int _cleanupCount;

		/// <summary>
		/// The interval at which to cleanup and remove extensions
		/// </summary>
		private readonly int _cleanupInterval = 40;
    }
}
