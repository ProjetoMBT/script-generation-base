using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Modeling.Graph
{
    /// <summary>
    /// Represents a Node model.
    /// </summary>
    [Serializable]
    public class DirectedGraph : GeneralUseStructure
    {
        /// <summary>
        /// EPSILON constant. Denotes empty sets.
        /// </summary>
        public static string EPSILON = new String('\u0190', 1);

        /// <summary>
        /// Finals Nodes.
        /// </summary>
        [XmlElement()]
        public List<Node> Finals;

        /// <summary>
        /// Input property.
        /// </summary>
        [XmlElement()]
        public List<string> Input
        {
            get;
            set;
        }
        /// <summary>
        /// Output property.
        /// </summary>
        [XmlElement()]
        public List<string> Output
        {
            get;
            set;
        }
        /// <summary>
        /// Node property.
        /// </summary>
        [XmlElement()]
        public List<Node> Nodes
        {
            get;
            set;
        }

        /// <summary>
        /// Defines a name to the instance.
        /// </summary>
        [XmlAttribute()]
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Node.
        /// </summary>
        [XmlElement()]
        public Node RootNode
        {
            get;
            set;
        }

        /// <summary>
        /// Edges.
        /// </summary>
        [XmlElement()]
        public List<Edge> Edges
        {
            get;
            set;
        }
     
        public Stack<String> Pilha
        {
            get;
            set;
        }

        public Dictionary<String, String> Values;

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public DirectedGraph(String name)
            :this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Parameterless constructor. Used by serializer.
        /// </summary>
        public DirectedGraph()
        {
            this.Nodes = new List<Node>();
            this.Edges = new List<Edge>();
            this.Finals = new List<Node>();
            this.Input = new List<string>();
            this.Output = new List<string>();
            this.Values = new Dictionary<String, String>();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Adds a new Edge to Graph Edge data.
        /// </summary>
        /// <param name="e"> Edge.</param>
        public void addEdge(Edge e)
        {
            if (!Nodes.Contains(e.NodeA))
            {
                addNode(e.NodeA);
            }
            if (!Nodes.Contains(e.NodeB))
            {
                addNode(e.NodeB);
            }
            Edges.Add(e);
        }

        public void addEdge(Node nA, Node nB)
        {
            Edge e = new Edge(nA, nB,false);
            addEdge(e);
        }

        /// <summary>
        /// Add a input data to input.
        /// </summary>
        /// <param name="inputData"></param>
        public Boolean AddInput(string inputData)
        {
            if (!Input.Contains(inputData))
            {
                Input.Add(inputData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a output to output data.
        /// </summary>
        /// <param name="outputData"></param>
        public Boolean AddOutput(string outputData)
        {
            if (!Output.Contains(outputData))
            {
                Output.Add(outputData);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Adds a node to the Graph.
        /// </summary>
        /// <param name="node">A node reference.</param>
        /// <returns>added to the node in list.</returns>
        public void addNode(Node node)
        {
            this.Nodes.Add(node);
        }

        /// <summary>
        /// checkFinal.
        /// </summary>
        /// <param name="n">A node reference.</param>
        /// <returns>added to the node in  Finals list.</returns>
        public void checkFinal(Node n)
        {
            if (!Nodes.Contains(n))
            {
                addNode(n);
            }
            this.Finals.Add(n);
        }

        public void WipeOutNode(Node s)
        {
            Edges.RemoveAll(x => x.NodeA.Equals(s) || x.NodeB.Equals(s));
            Nodes.RemoveAll(x => x.Equals(s));
        }

        public String printStack(Stack<String> pilha) 
        {
            String ret = "";
            foreach (String  s in pilha)
            {
                ret += " " + s + " ";
            }
            return ret;
        }

        public void SetTaggedValue(String tag, String value)
        {
            this.Values[tag.TrimEnd().TrimStart()] = value;
        }

        public String GetTaggedValue(String tag)
        {
            if (this.Values.Keys.Contains(tag))
            {
                return this.Values[tag];
            }
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
