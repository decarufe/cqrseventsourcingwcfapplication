﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.Backend.Endpoint {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Server.Backend.Endpoint.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Closing {0}....
        /// </summary>
        internal static string Closing_0_ {
            get {
                return ResourceManager.GetString("Closing_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Complete..
        /// </summary>
        internal static string Complete {
            get {
                return ResourceManager.GetString("Complete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CQRS Runtime.
        /// </summary>
        internal static string CQRS_Runtime {
            get {
                return ResourceManager.GetString("CQRS_Runtime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to msmq://localhost/CQRS.ReadModel.
        /// </summary>
        internal static string MsmqEndpoint {
            get {
                return ResourceManager.GetString("MsmqEndpoint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to service bus.
        /// </summary>
        internal static string service_bus {
            get {
                return ResourceManager.GetString("service_bus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Starting {0}....
        /// </summary>
        internal static string Starting_0_ {
            get {
                return ResourceManager.GetString("Starting_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waiting for messages.
        /// </summary>
        internal static string Waiting_for_messages {
            get {
                return ResourceManager.GetString("Waiting_for_messages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WCF Service.
        /// </summary>
        internal static string WCF_Service {
            get {
                return ResourceManager.GetString("WCF_Service", resourceCulture);
            }
        }
    }
}
