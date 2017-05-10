using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;

namespace FeatureModelEditor {
    /// <summary>
    /// Represents an extension inside aplication.
    /// </summary>
    public class Module {
        
        /// <summary>
        /// Extension name. Useful for exception messages.
        /// </summary>
        [XmlAttribute()]
        public string Name {get;set;}
        
        /// <summary>
        /// Represents the type name (without the assembly name).
        /// </summary>
        [XmlAttribute()]
        public string Type {get;set;}
        
        /// <summary>
        /// Assembly where the type is inside. (just the file name, not the full name).
        /// </summary>
        [XmlAttribute()]
        public string Assembly {get;set;}

        /// <summary>
        /// Interface where the extension will be "plugged".
        /// </summary>
        [XmlAttribute()]
        public string Interface {get;set;}
    }
}
