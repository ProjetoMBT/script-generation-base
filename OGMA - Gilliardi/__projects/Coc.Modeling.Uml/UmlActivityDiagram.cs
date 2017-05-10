using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Coc.Modeling.Uml
{
    /// <summary>
    /// Represents an activity diagram.
    /// </summary>
    public class UmlActivityDiagram : UmlDiagram
    {
        /// <summary>
        /// Default constructor. Takes its name as argument.
        /// </summary>
        /// <param name="p">Diagram's name.</param>
        public UmlActivityDiagram(String name)
        {
            this.Name = name;
            this.Lanes = new List<UmlLane>();
        }

        /// <summary>
        /// Stores lanes for this diagram
        /// </summary>
        public List<UmlLane> Lanes { get;set;}

        public override string ToString()
        {
            //return HttpUtility.UrlDecode(this.Name);
            //return this.Name;
            return this.Name + (this.Lanes.Count > 0 ? " <Lanes:" + this.Lanes.Count + ">" : "") + (this.UmlObjects.OfType<UmlElement>().Count() > 0 ?  " <Activities:" + this.UmlObjects.OfType<UmlElement>().Count() + ">" : "");
        }
    }
}
