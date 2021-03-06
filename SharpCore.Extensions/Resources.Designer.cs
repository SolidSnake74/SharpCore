﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.0
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SharpCore.Extensions {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SharpCore.Extensions.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
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
        ///   Busca una cadena traducida similar a ^[a-zA-Z]+$.
        /// </summary>
        internal static string Alpha {
            get {
                return ResourceManager.GetString("Alpha", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^[a-zA-Z0-9]+$.
        /// </summary>
        internal static string AlphaNumeric {
            get {
                return ResourceManager.GetString("AlphaNumeric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^\d{1,2}[\///]\d{1,2}[\\\/]\d{4}$.
        /// </summary>
        internal static string Date {
            get {
                return ResourceManager.GetString("Date", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$.
        /// </summary>
        internal static string EmailAddress {
            get {
                return ResourceManager.GetString("EmailAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$.
        /// </summary>
        internal static string Guid {
            get {
                return ResourceManager.GetString("Guid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^-?\d+$.
        /// </summary>
        internal static string Integer {
            get {
                return ResourceManager.GetString("Integer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^[1-9]+$.
        /// </summary>
        internal static string NonZeroInteger {
            get {
                return ResourceManager.GetString("NonZeroInteger", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$.
        /// </summary>
        internal static string PhoneNumber {
            get {
                return ResourceManager.GetString("PhoneNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^-?\d+(\.\d+)?$.
        /// </summary>
        internal static string Real {
            get {
                return ResourceManager.GetString("Real", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^\d{3}-\d{2}-\d{4}$.
        /// </summary>
        internal static string SocialSecurityNumber {
            get {
                return ResourceManager.GetString("SocialSecurityNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^\d{1,2}:\d{1,2}:\d{4}$.
        /// </summary>
        internal static string Time {
            get {
                return ResourceManager.GetString("Time", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^http:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&amp;=]*)?$.
        /// </summary>
        internal static string Url {
            get {
                return ResourceManager.GetString("Url", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a ^\d{5}(-\d{4})?$.
        /// </summary>
        internal static string Zipcode {
            get {
                return ResourceManager.GetString("Zipcode", resourceCulture);
            }
        }
    }
}
