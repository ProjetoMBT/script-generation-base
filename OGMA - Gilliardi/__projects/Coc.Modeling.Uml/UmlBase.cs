using System;
using System.Collections.Generic;
using System.Linq;
using Coc.Data.ControlAndConversionStructures;
using System.Web;

namespace Coc.Modeling.Uml
{
    /// <summary>
    /// Represents a base object for each Uml Object. Every Uml
    /// Object must derive from UmlBase. Properties that are
    /// commom to all Uml Objects must be changed in its base.
    /// </summary>
    public abstract class UmlBase : GeneralUseStructure
    {
        /// <summary>
        /// Stores current tagged values.
        /// </summary>
        public Dictionary<String, String> TaggedValues {get; private set;}

        /// <summary>
        /// Default construtor. Initializes internal structures.
        /// </summary>
        public UmlBase()
        {
            this.TaggedValues = new Dictionary<String, String>();
            this.Id = Guid.NewGuid().ToString();
            this.Stereotypes = new List<string>();
        }
        
        /// <summary>
        /// Stores comments for this object. Objects can be shared between 
        /// uml objects.
        /// </summary>
        public UmlComments Comments {get; set;}

        /// <summary>
        /// Set given tag value to given string.
        /// </summary>
        /// <param name="tag">Tag to be changed.</param>
        /// <param name="value">Value to be set.</param>
        public void SetTaggedValue(String tag, String value)
        {
            this.TaggedValues[tag.TrimEnd().TrimStart()] = value;
        }

        /// <summary>
        /// Return the value associated with given tag.
        /// </summary>
        /// <param name="tag">Tag to search for.</param>
        /// <returns>Value associated with given tag.</returns>
        public String GetTaggedValue(String tag)
        {
            if(this.TaggedValues.Keys.Contains(tag))
            {
                return this.TaggedValues[tag];
            }
            return null;
        }

        /// <summary>
        /// Returns a identifier for this object. Identifier may 
        /// prevent item duplication inside a list.
        /// </summary>
        public string Id { get;  set; }

        /// <summary>
        /// Stores uml base name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Stores object's stereotypes.
        /// </summary>
        public List<String> Stereotypes { get; set; }

        public override string ToString()
        {
            return Name;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            UmlBase otr = (UmlBase)obj;
            return this.Id.Equals(otr.Id);
        }
    }
}
