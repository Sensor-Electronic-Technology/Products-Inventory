﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventory.Common.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SETI\\elmendorfal")]
        public string LDAP_UserName {
            get {
                return ((string)(this["LDAP_UserName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("hQeY3bO3C2cC/DHXF3vmv4KwPZJpIJeDCHGqMDK+J6U=")]
        public string LDAP_EncryptedPassword {
            get {
                return ((string)(this["LDAP_EncryptedPassword"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("lNmrUIFj/zLnOUO3FrmbGKdQ0nY8eN9TpNrk/zPytAE=")]
        public string LDAP_Key {
            get {
                return ((string)(this["LDAP_Key"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3lryijwucB/DQAwPStmUbetOTAfw37cl6Y2DHH7hN6E=")]
        public string LDAP_IV {
            get {
                return ((string)(this["LDAP_IV"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("59Ho/m6wetIUX2zd8ph+8NvI1/PH9OhTt4m6f4S21+I=")]
        public string LDAP_EncryptedAddress {
            get {
                return ((string)(this["LDAP_EncryptedAddress"]));
            }
        }
    }
}
