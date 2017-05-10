using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Modeling.Graph
{
    /// <summary>
    /// Represents a Edge into a Graph.
    /// </summary>
    [Serializable]
    public class Edge : IComparable
    {
        /// <summary>
        /// From where the Edge comes.
        /// </summary>
        [XmlElement()]
        public Node NodeA
        {
            get;
            set;
        }

        /// <summary>
        /// To where the Edge goes.
        /// </summary>
        [XmlElement()]
        public Node NodeB
        {
            get;
            set;
        }

        public Dictionary<String, String> Values;
        private bool mark;
        
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// 
        public Edge()
        {

        }
        public Edge(Node NodeA, Node NodeB, Dictionary<String, String> someValues)
        {
            this.NodeA = NodeA;
            this.NodeB = NodeB;
            this.Values = new Dictionary<string, string>();
            foreach (KeyValuePair<String, String> value in someValues)
            {
                Values.Add(value.Key, value.Value);
            }
        }

        public Edge(Node nodeA, Node nodeB, bool mark)
        {
            this.NodeA = NodeA;
            this.NodeB = NodeB;
            this.mark = mark;
            this.Values = new Dictionary<string, string>();
        }

        public Edge(Node nodeA, Node nodeB, bool mark, Dictionary<String, String> someValues)
            : this(nodeA, nodeB, mark)
        {
            foreach (KeyValuePair<String, String> value in someValues)
            {
                Values.Add(value.Key, value.Value);
            }
        }

        /// <summary>
        /// Parameterless constructor. Used by serializer.
        /// </summary>  
        #endregion

        /// <summary>
        /// CompareTo implementation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Edge e;
            try
            {
                e = (Edge)obj;
            }
            catch (Exception ee)
            {
                Console.WriteLine("Unable to compare edge. Given object isnt a graph. " + ee.Source);
                return -1;
            }

            int nodes = (this.NodeA.Equals(e.NodeA) &&
                            this.NodeB.Equals(e.NodeB)
                            ? 0 : 1);

            return nodes;
        }

        public override string ToString()
        {
            return NodeA.ToString() + " -> " + NodeB.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Edge otr = (Edge)obj;
            if (!otr.NodeA.Equals(this.NodeA))
                return false;
            if (!otr.NodeB.Equals(this.NodeB))
                return false;
            //compare values

            return true;
        }

        public override int GetHashCode()
        {
            return NodeA.GetHashCode() ^ NodeB.GetHashCode() ^ Values.GetHashCode();
        }

        public bool isChecked()
        {
            return mark;
        }

        public void Check()
        {
            mark = true;
        }

        public void UnCheck()
        {
            mark = false;
        }

        public void SetTaggedValue(String tag, String value)
        {
            this.Values[tag.TrimEnd().TrimStart()] = value;
        }

        public String GetTaggedValue(String tag)
        {
            tag = tag.ToUpper();
            if (this.Values.Keys.Contains(tag))
            {
                return this.Values[tag];
            }
            return null;
        }
    }
}
