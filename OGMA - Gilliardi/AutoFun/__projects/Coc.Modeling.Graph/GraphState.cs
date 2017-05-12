using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Modeling.Graph
{
    public class GraphState
    {
        public Node Node
        {
            get;
            set;
        }
        public List<Node> Nodes
        {
            get;
            set;
        }

        public List<Node> Finals
        {
            get;
            set;
        }

        #region Constructors
        public GraphState()
        {
            this.Nodes = new List<Node>();
            this.Finals = new List<Node>();
            
        }
  
        #endregion

        #region Methods

        public void addNode(Node n)
        {
            this.Nodes.Add(n);
        }

        public List<Node> getNodes()
        {
            return this.Nodes;
        }
      
        public override string ToString()
        {
            return write(this, "");
        }

        private string write(GraphState s, string p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(p + s.Node.Name);
            
            return sb.ToString();
        }
        #endregion
    }
}
