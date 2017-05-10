using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.Uml
{
    public abstract class UmlDiagram
    {
        /// <summary>
        /// Stores this diagram name
        /// </summary>
        public String Name { get; set; }

        public String Id { set; get; }

        /// <summary>
        /// Stores uml objects for this model. 
        /// TODO: See UML documentation for validation.
        /// </summary>
        public List<UmlBase> UmlObjects { get; private set; }

        /// <summary>
        /// Stores stereotype set for this model.
        /// TODO: Should be vailable by profile.
        /// </summary>
        public List<String> UmlStereotypes { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UmlDiagram()
        {
            this.UmlObjects = new List<UmlBase>();
            this.UmlStereotypes = new List<String>();
            this.Id = Guid.NewGuid().ToString();
        }

        public UmlElement GetElementById(String id)
        {
            foreach (UmlBase element in UmlObjects.OfType<UmlBase>())
            {
                if (element.Id.Equals(id))
                {
                    return (UmlElement) element;
                }
            }
            return null;
        }

        public UmlModel ParentModel { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
