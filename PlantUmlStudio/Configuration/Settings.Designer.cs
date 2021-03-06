﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlantUmlStudio.Configuration {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<MyDocuments>\\PlantUmlStudio\\samples")]
        public string LastPath {
            get {
                return ((string)(this["LastPath"]));
            }
            set {
                this["LastPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("dot.exe")]
        public string GraphVizLocation {
            get {
                return ((string)(this["GraphVizLocation"]));
            }
            set {
                this["GraphVizLocation"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://downloads.sourceforge.net/sourceforge/plantuml/plantuml.jar")]
        public global::System.Uri PlantUmlDownloadLocation {
            get {
                return ((global::System.Uri)(this["PlantUmlDownloadLocation"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\PlantUML\\plantuml.jar")]
        public string PlantUmlLocation {
            get {
                return ((string)(this["PlantUmlLocation"]));
            }
            set {
                this["PlantUmlLocation"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".puml")]
        public string PlantUmlFileExtension {
            get {
                return ((string)(this["PlantUmlFileExtension"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://plantuml.com/download.html")]
        public global::System.Uri PlantUmlVersionSource {
            get {
                return ((global::System.Uri)(this["PlantUmlVersionSource"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".*\\(Version (?<version>\\d{4,5})\\).*")]
        public string PlantUmlRemoteVersionPattern {
            get {
                return ((string)(this["PlantUmlRemoteVersionPattern"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\PlantUML.xshd")]
        public string PlantUmlHighlightingDefinition {
            get {
                return ((string)(this["PlantUmlHighlightingDefinition"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".*version (?<version>\\d{4,5}(beta\\d+)?)[\\s].*")]
        public string PlantUmlLocalVersionPattern {
            get {
                return ((string)(this["PlantUmlLocalVersionPattern"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".*version[\\s]+(?<version>[\\.\\d]+)[\\s]+.*")]
        public string GraphVizLocalVersionPattern {
            get {
                return ((string)(this["GraphVizLocalVersionPattern"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RememberOpenFiles {
            get {
                return ((bool)(this["RememberOpenFiles"]));
            }
            set {
                this["RememberOpenFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection OpenFiles {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["OpenFiles"]));
            }
            set {
                this["OpenFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoSaveEnabled {
            get {
                return ((bool)(this["AutoSaveEnabled"]));
            }
            set {
                this["AutoSaveEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:30")]
        public global::System.TimeSpan AutoSaveInterval {
            get {
                return ((global::System.TimeSpan)(this["AutoSaveInterval"]));
            }
            set {
                this["AutoSaveInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection RecentFiles {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["RecentFiles"]));
            }
            set {
                this["RecentFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int MaximumRecentFiles {
            get {
                return ((int)(this["MaximumRecentFiles"]));
            }
            set {
                this["MaximumRecentFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool HighlightCurrentLine {
            get {
                return ((bool)(this["HighlightCurrentLine"]));
            }
            set {
                this["HighlightCurrentLine"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowLineNumbers {
            get {
                return ((bool)(this["ShowLineNumbers"]));
            }
            set {
                this["ShowLineNumbers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool EnableVirtualSpace {
            get {
                return ((bool)(this["EnableVirtualSpace"]));
            }
            set {
                this["EnableVirtualSpace"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool EnableWordWrap {
            get {
                return ((bool)(this["EnableWordWrap"]));
            }
            set {
                this["EnableWordWrap"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool EmptySelectionCopiesEntireLine {
            get {
                return ((bool)(this["EmptySelectionCopiesEntireLine"]));
            }
            set {
                this["EmptySelectionCopiesEntireLine"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AllowScrollingBelowContent {
            get {
                return ((bool)(this["AllowScrollingBelowContent"]));
            }
            set {
                this["AllowScrollingBelowContent"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".*<h4>New Release[\\s]+(?<version>[0-9]+\\.[0-9]+)[\\s]+.*")]
        public string GraphVizRemoteVersionPattern {
            get {
                return ((string)(this["GraphVizRemoteVersionPattern"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.graphviz.org/News.php")]
        public global::System.Uri GraphVizVersionSource {
            get {
                return ((global::System.Uri)(this["GraphVizVersionSource"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.graphviz.org/Download.php")]
        public global::System.Uri GraphVizDownloadLocation {
            get {
                return ((global::System.Uri)(this["GraphVizDownloadLocation"]));
            }
        }
    }
}
