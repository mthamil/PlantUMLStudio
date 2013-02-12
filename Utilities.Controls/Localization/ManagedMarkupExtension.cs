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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using System.Reflection;
using System.Windows;
using System.ComponentModel;

namespace Utilities.Controls.Localization
{
    /// <summary>
    /// Defines a base class for markup extensions which are managed by a central 
    /// <see cref="MarkupExtensionManager"/>. This allows the associated markup targets to be
    /// updated via the manager.
    /// </summary>
    /// <remarks>
    /// The ManagedMarkupExtension holds a weak reference to the target object to allow it to update 
    /// the target.  A weak reference is used to avoid a circular dependency which would prevent the
    /// target being garbage collected.  
    /// </remarks>
    public abstract class ManagedMarkupExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of a markup extension.
        /// </summary>
        public ManagedMarkupExtension(MarkupExtensionManager manager)
        {
            manager.RegisterExtension(this);
        }

        /// <summary>
        /// Returns the value for this instance of the Markup Extension.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>The value of the element</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            RegisterTarget(serviceProvider);

            // When used in a template the _targetProperty may be null - in this case
            // return this.
            if (_targetProperty != null)
            {
                return GetValue();
            }

            return this;
        }

        /// <summary>
        /// Called by <see cref="ProvideValue(IServiceProvider)"/> to register the target and object
        /// using the extension.   
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        protected virtual void RegisterTarget(IServiceProvider serviceProvider)
        {
			var provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
	        object target = provideValueTarget.TargetObject;

            // Check if the target is a SharedDp which indicates the target is a template
            // In this case we don't register the target and ProvideValue returns this
            // allowing the extension to be evaluated for each instance of the template.
            if (target != null && target.GetType().FullName != "System.Windows.SharedDp")
            {
                _targetProperty = provideValueTarget.TargetProperty;
                _targetObjects.Add(new WeakReference(target));
            }
        }

        /// <summary>
        /// Called by <see cref="UpdateTargets"/> to update each target referenced by the extension
        /// </summary>
        /// <param name="target">The target to update</param>
        protected virtual void UpdateTarget(object target)
        {
	        var dependencyProperty = _targetProperty as DependencyProperty;
	        if (dependencyProperty != null)
            {
                var dependencyObject = target as DependencyObject;
                if (dependencyObject != null)
                {
                    dependencyObject.SetValue(dependencyProperty, GetValue());
                }
            }
            else
	        {
		        var property = _targetProperty as PropertyInfo;
		        if (property != null)
		        {
			        property.SetValue(target, GetValue(), null);
		        }
	        }
        }

	    /// <summary>
        /// Update the associated targets
        /// </summary>
        public void UpdateTargets()
        {
            foreach (WeakReference reference in _targetObjects)
            {
                if (reference.IsAlive)
                {
                    UpdateTarget(reference.Target);
                }
            }
        }

        /// <summary>
        /// Checks whether the given object is the target for the extension.
        /// </summary>
        /// <param name="target">The target to check</param>
        /// <returns>True if the object is one of the targets for this extension</returns>
        public bool IsTarget(object target)
        {
	        return _targetObjects.Any(reference => reference.IsAlive && reference.Target == target);
        }

	    /// <summary>
        /// Is an associated target still alive, ie. not garbage collected.
        /// </summary>
        public bool IsTargetAlive
        {
            get 
            {
                // For normal elements the _targetObjects.Count will always be 1
                // for templates the Count may be zero if this method is called
                // in the middle of window elaboration after the template has been
                // instantiated but before the elements that use it have been.  In
                // this case return true so that we don't unhook the extension
                // prematurely.
                if (_targetObjects.Count == 0)
                    return true;
                
                // Otherwise, just check whether the referenced target(s) are alive.
				return _targetObjects.Any(reference => reference.IsAlive);
            } 
        }

        /// <summary>
        /// Returns true if a target attached to this extension is in design mode.
        /// </summary>
        public bool IsInDesignMode
        {
            get
            {
				return _targetObjects
		            .Select(reference => reference.Target)
		            .OfType<DependencyObject>()
		            .Any(element => element != null && DesignerProperties.GetIsInDesignMode(element));
            }
        }

        /// <summary>
        /// Return the target objects the extension is associated with.
        /// </summary>
        /// <remarks>
        /// For normal elements their will be a single target. For templates
        /// their may be zero or more targets
        /// </remarks>
        protected IEnumerable<WeakReference> TargetObjects
        {
            get { return _targetObjects; }
        }

        /// <summary>
        /// Return the Target Property the extension is associated with
        /// </summary>
        /// <remarks>
        /// Can either be a <see cref="DependencyProperty"/> or <see cref="PropertyInfo"/>
        /// </remarks>
        protected object TargetProperty
        {
            get { return _targetProperty; }
        }

        /// <summary>
        /// The type of the Target Property.
        /// </summary>
        protected Type TargetPropertyType
        {
            get
            {
                if (_targetProperty is DependencyProperty)
					return ((DependencyProperty)_targetProperty).PropertyType;
	            if (_targetProperty is PropertyInfo)
		            return ((PropertyInfo)_targetProperty).PropertyType;
	            if (_targetProperty != null)
		            return _targetProperty.GetType();
	            return null;
            }
        }

	    /// <summary>
        /// Returns the value associated with the key from the resource manager.
        /// </summary>
        /// <returns>The value from the resources if possible otherwise the default value</returns>
        protected abstract object GetValue();

		/// <summary>
		/// The target property 
		/// </summary>
		private object _targetProperty;
		
        /// <summary>
        /// List of weak reference to the target DependencyObjects to allow them to 
        /// be garbage collected
        /// </summary>
        private readonly ICollection<WeakReference> _targetObjects = new List<WeakReference>();
    }
}
