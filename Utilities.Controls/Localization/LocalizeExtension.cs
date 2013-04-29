//  PlantUML Studio
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Resources;
using System.Reflection;
using System.Windows;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Collections.ObjectModel;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Utilities.Controls.Localization")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2007/xaml/presentation", "Utilities.Controls.Localization")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2008/xaml/presentation", "Utilities.Controls.Localization")]

namespace Utilities.Controls.Localization
{
    /// <summary>
    /// Defines the handling method for the <see cref="LocalizeExtension.GetResource"/> event
    /// </summary>
    /// <param name="resourceFileName">The name of the resource file</param>
    /// <param name="key">The resource key within the file</param>
    /// <param name="culture">The culture to get the resource for</param>
    /// <returns>The resource</returns>
    public delegate object GetResourceHandler(string resourceFileName, string key, CultureInfo culture);

    /// <summary>
    /// A markup extension to allow resources for WPF Windows and controls to be retrieved
    /// from an embedded resource (resx) file associated with the window or control.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    [ContentProperty("Children")]
    public class LocalizeExtension : ManagedMarkupExtension
    {
        /// <summary>
        /// This global event allows a designer or preview application (such as Globalizer.NET) to
        /// intercept calls to get resources and provide the values instead dynamically.
        /// </summary>
        public static event GetResourceHandler GetResource;

	    /// <summary>
        /// Initializes a new instance of the markup extension.
        /// </summary>
        public LocalizeExtension()
			: this(null) { }

        /// <summary>
		/// Initializes a new instance of the markup extension.
        /// </summary>
        /// <param name="key">The key used to get the value from the resources</param>
        public LocalizeExtension(string key)
			: this(MarkupExtensionManager.For<LocalizeExtension>(CleanupInterval), CultureManager.Default, key) { }

		/// <summary>
		/// Initializes a new instance of the markup extension.
		/// </summary>
	    internal LocalizeExtension(MarkupExtensionManager markupExtensionManager, ICultureManager cultureManager, string key)
			: base(markupExtensionManager)
		{
			_cultureManager = cultureManager;
			_key = key;

			WeakEventManager<ICultureManager, EventArgs>.AddHandler(_cultureManager, "UICultureChanged", cultureManager_UICultureChanged);
		}

	    private void cultureManager_UICultureChanged(object sender, EventArgs eventArgs)
	    {
		    UpdateTargets();
	    }

	    /// <summary>
        /// The fully qualified name of the embedded resx (without .resources) to get
        /// the resource from.
        /// </summary>
        public string ResxName
        {
            get 
            {
                // If the ResxName property is not set explicitly then check the attached property.
                string result = _resxName;
                if (string.IsNullOrEmpty(result))
                {
                    if (_defaultResxName == null)
                    {
                        WeakReference targetRef = TargetObjects.FirstOrDefault(target => target.IsAlive);
                        if (targetRef != null)
                        {
	                        var targetDependencyObject = targetRef.Target as DependencyObject;
	                        if (targetDependencyObject != null)
                            {
                               _defaultResxName = targetDependencyObject.GetValue(DefaultResxNameProperty) as string;
                            }
                        }
                    }
                    result = _defaultResxName;
                }
                return result; 
            }
            set 
            { 
                _resxName = value; 
            }
        }

        /// <summary>
        /// The name of the resource key.
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// The default value to use if a resource can't be found.
        /// </summary>
        /// <remarks>
        /// This is particularly useful for properties which require non-null
        /// values because it allows a page to be displayed even if
        /// the resource can't be loaded.
        /// </remarks>
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        /// <summary>
        /// The child Resx elements (if any).
        /// </summary>
        /// <remarks>
        /// You can nest Resx elements in this case the parent Resx element
        /// value is used as a format string to format the values from child Resx
        /// elements similar to a <see cref="MultiBinding"/>.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<LocalizeExtension> Children
        {
            get { return _children; }
        }

        #region Delegated Binding properties

        /// <summary>
        /// The associated binding for the extension.
        /// </summary>
        public Binding Binding
        {
            get { return _binding.Value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.ElementName"/>.
        /// </summary>
        [DefaultValue(null)]
        public string BindingElementName
        {
            get { return Binding.ElementName; }
            set { Binding.ElementName = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.Path"/>.
        /// </summary>
        [DefaultValue(null)]
        public PropertyPath BindingPath
        {
            get { return Binding.Path; }
            set { Binding.Path = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.RelativeSource"/>.
        /// </summary>
        [DefaultValue(null)]
        public RelativeSource BindingRelativeSource
        {
            get { return Binding.RelativeSource; }
            set { Binding.RelativeSource = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.Source"/>.
        /// </summary>
        [DefaultValue(null)]
        public object BindingSource
        {
            get { return Binding.Source; }
            set { Binding.Source = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.XPath"/>.
        /// </summary>
        [DefaultValue(null)]
        public string BindingXPath
        {
            get { return Binding.XPath; }
            set { Binding.XPath = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.Converter"/>.
        /// </summary>
        [DefaultValue(null)]
        public IValueConverter BindingConverter
        {
            get { return Binding.Converter; }
            set { Binding.Converter = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.BindingBase.BindingGroupName"/>.
        /// </summary>
        [DefaultValue(null)]
        public string BindingGroupName
        {
            get { return Binding.BindingGroupName; }
            set { Binding.BindingGroupName = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.ConverterCulture"/>.
        /// </summary>
        [DefaultValue(null)]
        public CultureInfo BindingConverterCulture
        {
            get { return Binding.ConverterCulture; }
            set { Binding.ConverterCulture = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.ConverterParameter"/>.
        /// </summary>
        [DefaultValue(null)]
        public object BindingConverterParameter
        {
            get { return Binding.ConverterParameter; }
            set { Binding.ConverterParameter = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.BindsDirectlyToSource"/>.
        /// </summary>
        [DefaultValue(false)]
        public bool BindsDirectlyToSource
        {
            get { return Binding.BindsDirectlyToSource; }
            set { Binding.BindsDirectlyToSource = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.Mode"/>.
        /// </summary>
        [DefaultValue(BindingMode.Default)]
        public BindingMode BindingMode
        {
            get { return Binding.Mode; }
            set { Binding.Mode = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.NotifyOnSourceUpdated"/>.
        /// </summary>
        [DefaultValue(false)]
        public bool BindingNotifyOnSourceUpdated
        {
            get { return Binding.NotifyOnSourceUpdated; }
            set { Binding.NotifyOnSourceUpdated = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.NotifyOnTargetUpdated"/>.
        /// </summary>
        [DefaultValue(false)]
        public bool BindingNotifyOnTargetUpdated
        {
            get { return Binding.NotifyOnTargetUpdated; }
            set { Binding.NotifyOnTargetUpdated = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.NotifyOnValidationError"/>.
        /// </summary>
        [DefaultValue(false)]
        public bool BindingNotifyOnValidationError
        {
            get { return Binding.NotifyOnValidationError; }
            set { Binding.NotifyOnValidationError = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.AsyncState"/>.
        /// </summary>
        [DefaultValue(null)]
        public object BindingAsyncState
        {
            get { return Binding.AsyncState; }
            set { Binding.AsyncState = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.IsAsync"/>.
        /// </summary>
        [DefaultValue(false)]
        public bool BindingIsAsync
        {
            get { return Binding.IsAsync; }
            set { Binding.IsAsync = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.BindingBase.FallbackValue"/>.
        /// </summary>
        [DefaultValue(null)]
        public object BindingFallbackValue
        {
            get { return Binding.FallbackValue; }
            set { Binding.FallbackValue = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.BindingBase.TargetNullValue"/>.
        /// </summary>
        [DefaultValue(null)]
        public object BindingTargetNullValue
        {
            get { return Binding.TargetNullValue; }
            set { Binding.TargetNullValue = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.ValidatesOnDataErrors"/>.
        /// </summary>
        [DefaultValue(false)]
        public bool BindingValidatesOnDataErrors
        {
            get { return Binding.ValidatesOnDataErrors; }
            set { Binding.ValidatesOnDataErrors = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.ValidatesOnExceptions"/>.
        /// </summary>
        [DefaultValue(false)]
        public bool BindingValidatesOnExceptions
        {
            get { return Binding.ValidatesOnExceptions; }
            set { Binding.ValidatesOnExceptions = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.UpdateSourceTrigger"/>.
        /// </summary>
        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger BindingUpdateSourceTrigger
        {
            get { return Binding.UpdateSourceTrigger; }
            set { Binding.UpdateSourceTrigger = value; }
        }

        /// <summary>
        /// Use the Resx value to format bound data.  See <see cref="System.Windows.Data.Binding.ValidationRules"/>.
        /// </summary>
        [DefaultValue(false)]
        public Collection<ValidationRule> BindingValidationRules
        {
            get { return Binding.ValidationRules; }
        }

        #endregion

        /// <summary>
        /// Returns the value for this instance of the Markup Extension.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>The value of the element</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Register the target and property so we can update them.
            RegisterTarget(serviceProvider);

            if (String.IsNullOrEmpty(Key) && !IsBindingExpression)
                throw new ArgumentException("You must set the resource Key or Binding properties");

            // If the extension is used in a template or as a child of another
            // extension (for multi-binding) then return this.
            if (TargetProperty == null || IsMultiBindingChild)
            {
                return this;
            }

	        // If this extension has child Resx elements then invoke AFTER this method has returned
	        // to setup the MultiBinding on the target element.
	        if (IsMultiBindingParent)
	        {
		        MultiBinding binding = CreateMultiBinding();
		        return binding.ProvideValue(serviceProvider);
	        }

	        if (IsBindingExpression)
	        {
		        // If this is a simple binding then return the binding.
		        Binding binding = CreateBinding();
		        return binding.ProvideValue(serviceProvider);
	        }

	        // Otherwise, return the value from the resources.
	        return GetValue();
        }

		/// <summary>
		/// Gets the DefaultResxName attached property for the given target.
		/// </summary>
		/// <param name="target">The Target object</param>
		/// <returns>The name of the Resx</returns>
		[AttachedPropertyBrowsableForChildren(IncludeDescendants = true)]
		public static string GetDefaultResxName(DependencyObject target)
		{
			return (string)target.GetValue(DefaultResxNameProperty);
		}

		/// <summary>
		/// Sets the DefaultResxName attached property for the given target.
		/// </summary>
		/// <param name="target">The Target object</param>
		/// <param name="value">The name of the Resx</param>
		public static void SetDefaultResxName(DependencyObject target, string value)
		{
			target.SetValue(DefaultResxNameProperty, value);
		}

        /// <summary>
        /// The ResxName attached property.
        /// </summary>
        public static readonly DependencyProperty DefaultResxNameProperty =
            DependencyProperty.RegisterAttached(
            "DefaultResxName",
            typeof(string),
            typeof(LocalizeExtension),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.Inherits,
                OnDefaultResxNamePropertyChanged));

		/// <summary>
		/// Handles a change to the attached DefaultResxName property.
		/// </summary>
		/// <param name="element">The dependency object (a WPF element)</param>
		/// <param name="args">The dependency property changed event arguments</param>
		/// <remarks>In design mode, updates the extension with the correct ResxName</remarks>
		private static void OnDefaultResxNamePropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{
			if (DesignerProperties.GetIsInDesignMode(element))
			{
				var applicableExtensions = MarkupExtensionManager.For<LocalizeExtension>(CleanupInterval).ActiveExtensions
																 .OfType<LocalizeExtension>()
															     .Where(extension => extension.IsTarget(element));
				foreach (var extension in applicableExtensions)
				{
					// Force the resource manager to be reloaded when the attached resx name changes.
					extension._resourceManager = null;
					extension._defaultResxName = args.NewValue as string;
					extension.UpdateTarget(element);
				}
			}
		}

        #region Local Methods

        /// <summary>
		/// Creates a binding for this <see cref="LocalizeExtension"/>.
        /// </summary>
		/// <returns>A binding for this <see cref="LocalizeExtension"/></returns>
        private Binding CreateBinding()
        {
	        if (!IsBindingExpression)
	        {
		        return new Binding
		        {
			        Source = GetValue()
		        };
	        }

	        // Copy all the properties of the binding to a new binding.
	        var binding = new Binding
	        {
		        AsyncState = _binding.Value.AsyncState,
		        BindingGroupName = _binding.Value.BindingGroupName,
		        BindsDirectlyToSource = _binding.Value.BindsDirectlyToSource,
		        Converter = _binding.Value.Converter,
		        ConverterCulture = _binding.Value.ConverterCulture,
		        ConverterParameter = _binding.Value.ConverterParameter,
		        FallbackValue = _binding.Value.FallbackValue,
		        IsAsync = _binding.Value.IsAsync,
		        Mode = _binding.Value.Mode,
		        NotifyOnSourceUpdated = _binding.Value.NotifyOnSourceUpdated,
		        NotifyOnTargetUpdated = _binding.Value.NotifyOnTargetUpdated,
		        NotifyOnValidationError = _binding.Value.NotifyOnValidationError,
		        Path = _binding.Value.Path,
		        TargetNullValue = _binding.Value.TargetNullValue,
		        XPath = _binding.Value.XPath,
		        StringFormat = GetValue() as string,
		        UpdateSourceTrigger = _binding.Value.UpdateSourceTrigger,
		        ValidatesOnDataErrors = _binding.Value.ValidatesOnDataErrors,
		        ValidatesOnExceptions = _binding.Value.ValidatesOnExceptions
	        };

	        foreach (var rule in _binding.Value.ValidationRules)
		        binding.ValidationRules.Add(rule);
				
	        if (_binding.Value.ElementName != null)
		        binding.ElementName = _binding.Value.ElementName;

	        if (_binding.Value.RelativeSource != null)
		        binding.RelativeSource = _binding.Value.RelativeSource;

	        if (_binding.Value.Source != null)
		        binding.Source = _binding.Value.Source;

	        return binding;
        }

	    /// <summary>
		/// Creates a new MultiBinding that binds to the child <see cref="LocalizeExtension"/>s.
        /// </summary>
        private MultiBinding CreateMultiBinding()
        {
            var result = new MultiBinding();
            foreach (var child in _children)
            {
                // Ensure the child has a resx name.
                if (child.ResxName == null)
                {
                    child.ResxName = ResxName;
                }
                result.Bindings.Add(child.CreateBinding());
            }
            result.StringFormat = GetValue() as string;
            return result;
        }

        /// <summary>
        /// Whether any binding properties have been set.
        /// </summary>
        private bool IsBindingExpression
        {
            get 
            {
				return (_binding.Value.Source != null || _binding.Value.RelativeSource != null ||
					    _binding.Value.ElementName != null || _binding.Value.XPath != null ||
					    _binding.Value.Path != null); 
            }
        }

        /// <summary>
		/// Whether this <see cref="LocalizeExtension"/> is being used as a multi-binding parent.
        /// </summary>
        private bool IsMultiBindingParent
        {
            get { return _children.Count > 0; }
        }

        /// <summary>
		/// Whether this <see cref="LocalizeExtension"/> is being used inside another <see cref="LocalizeExtension"/> for multi-binding.
        /// </summary>
        private bool IsMultiBindingChild
        {
            get 
            { 
                return (TargetPropertyType == typeof(Collection<LocalizeExtension>)); 
            }
        }

        /// <summary>
        /// Produces the value for the markup extension.
        /// </summary>
        /// <returns>The value from the resources if possible otherwise the default value</returns>
        protected override object GetValue()
        {
            if (string.IsNullOrEmpty(Key)) 
				return null;

            object result = null;
            if (!String.IsNullOrEmpty(ResxName))
            {
                try
                {
	                var localGetResource = GetResource;
					if (localGetResource != null)
                    {
						result = localGetResource(ResxName, Key, _cultureManager.UICulture);
                    }

                    if (result == null)
                    {
                        if (_resourceManager == null)
                        {
                            _resourceManager = GetResourceManager(ResxName);
                        }
                        if (_resourceManager != null)
                        {
                            result = _resourceManager.GetObject(Key, _cultureManager.UICulture);
                        }
                    }

                    if (!IsMultiBindingChild)
                    {
                        result = ConvertValue(result);
                    }
                }
                catch (Exception e)
                {
					Debug.Write(e);
                }
            }

            if (result == null)
            {
                result = GetDefaultValue(Key);
            }
            return result;
        }

        /// <summary>
        /// Updates the given target when the culture changes.
        /// </summary>
        /// <param name="target">The target to update</param>
        protected override void UpdateTarget(object target)
        {
            // Binding of child extensions is done by the parent.
            if (IsMultiBindingChild) 
				return;

            if (IsMultiBindingParent)
            {
                var element = target as FrameworkElement;
                if (element != null)
                {
                    MultiBinding multiBinding = CreateMultiBinding();
                    element.SetBinding(TargetProperty as DependencyProperty, multiBinding);
                }
            }
            else if (IsBindingExpression)
            {
                var element = target as FrameworkElement;
                if (element != null)
                {
                    Binding binding = CreateBinding();
                    element.SetBinding(TargetProperty as DependencyProperty, binding);
                }
            }
            else
            {
                base.UpdateTarget(target);
            }
        }

        /// <summary>
        /// Checks if an assembly contains an embedded resx of the given name.
        /// </summary>
        /// <param name="assembly">The assembly to check</param>
        /// <param name="resxName">The name of the resource we are looking for</param>
        /// <returns>True if the assembly contains the resource</returns>
        private bool HasEmbeddedResx(Assembly assembly, string resxName)
        {
            if (assembly.IsDynamic) 
				return false;

            try
            {
                string[] resources = assembly.GetManifestResourceNames();
                string searchName = resxName.ToLower() + ".resources";

                if (resources.Any(resource => resource.ToLower() == searchName))
                {
	                return true;
                }
            }
            catch (Exception e)
            {
                // GetManifestResourceNames may throw an exception
                // for some assemblies - just ignore these assemblies,
				// but log them in debug mode.
				Debug.Write(e);
            }
            return false;
        }

		/// <summary>
		/// Checks whether an assembly is a system assembly.
		/// </summary>
	    private bool IsSystemAssembly(Assembly assembly)
	    {
			string name = assembly.FullName;
			return (name.StartsWith("Microsoft.") ||
			        name.StartsWith("System.") ||
			        name.StartsWith("System,") ||
			        name.StartsWith("mscorlib,") ||
			        name.StartsWith("PresentationFramework,") ||
			        name.StartsWith("WindowsBase,"));
	    }

	    /// <summary>
        /// Finds the assembly that contains the type.
        /// </summary>
        /// <returns>The assembly if loaded (otherwise null)</returns>
        private Assembly FindResourceAssembly()
        {
            var assembly = Assembly.GetEntryAssembly();

            // Check the entry assembly first - this will short circuit a lot of searching.
            if (assembly != null && HasEmbeddedResx(assembly, ResxName)) 
				return assembly;

			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(searchAssembly => !IsSystemAssembly(searchAssembly))	// Don't check system provided assemblies.
				.FirstOrDefault(searchAssembly => HasEmbeddedResx(searchAssembly, ResxName));
        }

        /// <summary>
        /// Gets the resource manager for this type.
        /// </summary>
        /// <param name="resxName">The name of the embedded resx</param>
        /// <returns>The resource manager</returns>
        /// <remarks>Caches resource managers to improve performance</remarks>
        private ResourceManager GetResourceManager(string resxName)
        {
            if (resxName == null) 
				return null;

			WeakReference<ResourceManager> reference;
			ResourceManager result = null;
            if (_resourceManagers.TryGetValue(resxName, out reference))
            {
	            if (!reference.TryGetTarget(out result))
	            {
					// If the resource manager has been garbage collected then remove the cache
					// entry (it will be readded).
					_resourceManagers.Remove(resxName);
	            }
            }

            if (result == null)
            {
                var assembly = FindResourceAssembly();
                if (assembly != null)
                {
                    result = new ResourceManager(resxName, assembly);
					_resourceManagers.Add(resxName, new WeakReference<ResourceManager>(result));
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a resource object to the type required by the WPF element.
        /// </summary>
        /// <param name="value">The resource value to convert</param>
        /// <returns>The WPF element value</returns>
        private object ConvertValue(object value)
        {
            var bitmapSource = TryCreateBitmapSource(value);
	        if (bitmapSource != null)
            {
	            // If the target property is expecting the Icon to be content then we
                // create an ImageControl and set its Source property to image.
                if (TargetPropertyType == typeof(object))
                {
                    return new System.Windows.Controls.Image
                    {
	                    Source = bitmapSource,
	                    Width = bitmapSource.Width,
	                    Height = bitmapSource.Height
                    };
                }

	            return bitmapSource;
            }
            
	        // Allow for resources to either contain simple strings or typed data.
	        Type targetType = TargetPropertyType;
	        if (targetType != null)
	        {
		        if (value is String && targetType != typeof(String) && targetType != typeof(object))
		        {
			        TypeConverter converter = TypeDescriptor.GetConverter(targetType);
			        return converter.ConvertFromInvariantString(value as string);
		        }
	        }

			return value;
        }

		/// <summary>
		/// Attempts to create a <see cref="BitmapSource"/> from an icon or bitmap so
		/// that it is usable by WPF.
		/// </summary>
	    private BitmapSource TryCreateBitmapSource(object value)
		{
			if (value is Icon)
			{
				// For icons we must create a new BitmapFrame from the icon data stream
				// The approach we use for bitmaps (below) doesn't work when setting the
				// Icon property of a window (although it will work for other Icons).
				using (var iconStream = new MemoryStream())
				{
					var icon = (Icon)value;
					icon.Save(iconStream);
					iconStream.Seek(0, SeekOrigin.Begin);
					return BitmapFrame.Create(iconStream);
				}
			}

			if (value is Bitmap)
			{
				var bitmap = (Bitmap)value;
				IntPtr bitmapHandle = bitmap.GetHbitmap();
				var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
					bitmapHandle, IntPtr.Zero, Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
				bitmapSource.Freeze();
				DeleteObject(bitmapHandle);
				return bitmapSource;
			}

			return null;
		}

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

	    /// <summary>
        /// Returns the default value for a property.
        /// </summary>
        private object GetDefaultValue(string key)
        {
            Type targetType = TargetPropertyType;
            if (_defaultValue == null)
            {
                if (targetType == typeof(String) || targetType == typeof(object) || IsMultiBindingChild)
                {
                    return "#" + key;
                }
            }
            else if (targetType != null)
            {
				// Convert the default value to the required type if necessary.
                if (targetType != typeof(String) && targetType != typeof(object))
                {
                    try
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                        return converter.ConvertFromInvariantString(_defaultValue);
                    }
                    catch (Exception e)
                    {
						Debug.Write(e);
                    }
                }
            }

			return _defaultValue;
        }

        #endregion

		/// <summary>
		/// The explicitly set embedded Resx Name (if any).
		/// </summary>
		private string _resxName;

		/// <summary>
		/// The default resx name (based on the attached property).
		/// </summary>
		private string _defaultResxName;

	    /// <summary>
		/// The key used to retrieve the resource.
		/// </summary>
		private string _key;

		/// <summary>
		/// The default value for the property.
		/// </summary>
		private string _defaultValue;

		/// <summary>
		/// The resource manager to use for this extension.  Holding a strong reference to the
		/// Resource Manager keeps it in the cache while ever there are LocalizeExtensions that
		/// are using it.
		/// </summary>
		private ResourceManager _resourceManager;

		private readonly ICultureManager _cultureManager;

		/// <summary>
		/// The binding (if any) used to store the binding properties for the extension  .
		/// </summary>
		private readonly Lazy<Binding> _binding = new Lazy<Binding>(() => new Binding());

		/// <summary>
		/// The child <see cref="LocalizeExtension"/>s (if any) when using MultiBinding expressions.
		/// </summary>
		private readonly Collection<LocalizeExtension> _children = new Collection<LocalizeExtension>();

	    private const int CleanupInterval = 40;

		/// <summary>
		/// Cached resource managers.
		/// </summary>
		private static readonly IDictionary<string, WeakReference<ResourceManager>> _resourceManagers = new Dictionary<string, WeakReference<ResourceManager>>();
    }
}
