using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Modeling.Graph
{
    /// <summary>
    /// Represents a node.
    /// </summary>
    [Serializable]
    public class Node
    {

        #region Constructors
        /// <summary>
        /// Parameterless constructor. Used by XmlSerializer.
        /// </summary>
        public Node()
        {
            this.mark = false;
            this.Name = "nameless_node";
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Node(String name)
        {
            this.mark = false;
            this.Name = name;
        }
        /// <summary>
        /// Full constructor.
        /// </summary>
        public Node(String name, bool mark)
        {
            this.mark = mark;
            this.Name = name;
        }

        public Node(String name, String id)
        {
            this.Name = name;
            this.Id = id;
        }
        #endregion

        /// <summary>
        /// Identifies the node on a graph.
        /// </summary>
        [XmlAttribute()]
        public String Id
        {
            get;
            set;
        }

        /// <summary>
        /// Name property.
        /// </summary>
        [XmlAttribute()]
        public String Name
        {
            get;
            set;
        }

        public bool mark;

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            //if (((Node)obj).Id.Equals(this.Id))
            //{
            //    return true;
            //}

            if (((Node)obj).Name.Equals(this.Name))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public bool isChecked()
        {
            return mark;
        }

        public void check()
        {
            mark = true;
        }

        public void uncheck()
        {
            mark = false;
        }

        #endregion
    }
}
