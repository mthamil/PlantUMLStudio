//  PlantUML Editor
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2008 Grant Frisken, Infralution (original author)
//  Originally licensed under the CodeProject Open License.
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
using System.Globalization;
using System.Threading;

namespace Utilities.Controls.Localization
{
	/// <summary>
    /// Provides the ability to change the UICulture for WPF Windows and controls
    /// dynamically.  
    /// </summary>
    /// <remarks>
    /// XAML elements that use the <see cref="LocalizeExtension"/> are automatically
    /// updated when the <see cref="CultureManager.UICulture"/> property is changed.
    /// </remarks>
    public class CultureManager : ICultureManager
	{
		/// <summary>
		/// The default culture manager instance.
		/// </summary>
		public static ICultureManager Default
		{
			get { return _defaultInstance; }
		}

        /// <summary>
        /// Sets the UICulture for a <see cref="CultureManager"/> and raises the <see cref="UICultureChanged"/>
        /// event causing any XAML elements using the <see cref="LocalizeExtension"/> to automatically
        /// update.
        /// </summary>
        public CultureInfo UICulture
        {
            get
            {
                if (_uiCulture == null)
                {
                    _uiCulture = Thread.CurrentThread.CurrentUICulture;
                }
                return _uiCulture;
            }
            set
            {
                if (value != UICulture)
                {
                    _uiCulture = value;
                    Thread.CurrentThread.CurrentUICulture = value;
                    if (SynchronizeThreadCulture)
                    {
                        UpdateThreadCulture(value);
                    }
                    UICultureExtension.UpdateAllTargets();
                    LocalizeExtension.UpdateAllTargets();
                    OnUICultureChanged();
                }
            }
        }

		/// <summary>
		/// Raised when the <see cref="UICulture"/> is changed.
		/// </summary>
		/// <remarks>
		/// Since this event is static if the client object does not detach from the event a reference
		/// will be maintained to the client object preventing it from being garbage collected - thus
		/// causing a potential memory leak. 
		/// </remarks>
		public event EventHandler UICultureChanged;

		private void OnUICultureChanged()
		{
			var localEvent = UICultureChanged;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

        /// <summary>
        /// If set to true then the <see cref="Thread.CurrentCulture"/> property is changed
        /// to match the current <see cref="UICulture"/>.
        /// </summary>
        public bool SynchronizeThreadCulture
        {
            get { return _synchronizeThreadCulture; }
            set
            {
                _synchronizeThreadCulture = value;
                if (value)
                {
                    UpdateThreadCulture(UICulture);
                }
            }
        }

        /// <summary>
        /// Sets the thread culture to the given culture.
        /// </summary>
        /// <param name="value">The culture to set</param>
        /// <remarks>If the culture is neutral then a specific culture is created.</remarks>
        private void UpdateThreadCulture(CultureInfo value)
        {
	        Thread.CurrentThread.CurrentCulture = value.IsNeutralCulture
				? CultureInfo.CreateSpecificCulture(value.Name) 
				: value;
        }

	    /// <summary>
        /// Current UICulture of the manager.
        /// </summary>
        private CultureInfo _uiCulture;

        /// <summary>
        /// Should the <see cref="Thread.CurrentCulture"/> be changed when the
        /// <see cref="UICulture"/> changes.
        /// </summary>
        private bool _synchronizeThreadCulture = true;

		private static readonly CultureManager _defaultInstance = new CultureManager();
    }
}
