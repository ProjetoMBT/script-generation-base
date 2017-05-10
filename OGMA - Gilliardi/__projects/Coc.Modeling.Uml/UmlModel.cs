using System;
using System.Collections.Generic;
using System.Linq;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Modeling.Uml
{
    /*
    /// <summary>
    /// <img src="images/UML.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    /// <summary>
    /// Represents a full UmlModel. A model contains a set of 
    /// diagrams and stereotypes.
    /// </summary>
    public class UmlModel : GeneralUseStructure
    {
        public String Id { set; get; }


        /// <summary>
        /// Stores model name (optional).
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Stores stereotype for this model.
        /// </summary>
        private List<String> stereotypes;

        /// <summary>
        /// Default constructor. Takes its name as parameter.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        public UmlModel(String modelName)
        {
            this.Name = modelName;
            this.stereotypes = new List<String>();
            this.diagrams = new Dictionary<String, UmlDiagram>();
            this.Id = Guid.NewGuid().ToString();
        }

        public UmlModel()
        {
            this.Name = "";
            this.Id = Guid.NewGuid().ToString();
            this.stereotypes = new List<String>();
            this.diagrams = new Dictionary<String, UmlDiagram>();
        }

        /// <summary>
        /// Stores diagrams. A model is made of diagrams and stereotypes.
        /// TODO: Review Uml documentation.
        /// </summary>
        private Dictionary<String, UmlDiagram> diagrams;

        /// <summary>
        /// Adds a diagram to this model.
        /// </summary>
        /// <param name="diagram">Diagram to be added.</param>
        public void AddDiagram(UmlDiagram diagram)
        {
            this.diagrams[diagram.Id] = diagram;
        }

        /// <summary>
        /// Returns a set containing diagram models.
        /// </summary>
        public List<UmlDiagram> Diagrams { get { return this.diagrams.Values.ToList(); } }

        public void Clear()
        {
            this.stereotypes.Clear();
            this.diagrams.Clear();
        }
        public Object clone()
        {
            return base.MemberwiseClone();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}