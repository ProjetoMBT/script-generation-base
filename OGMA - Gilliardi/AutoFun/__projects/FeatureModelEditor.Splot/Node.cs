using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureModelEditor.Splot {
    public class Node {
        public NodeType Type { get; set; }
        public String Name{ get; set; }
        public List<Node> Children {get;set;}
        public Int32 Depth { get; set; }

        public Node(){
            this.Children = new List<Node>();
        }

        public Node(String name){
            this.Children = new List<Node>();
            this.Name = name;
        }

        public Node(String name, NodeType type){
            this.Children = new List<Node>();
            this.Name = name;
            this.Type = type;
        }
    }

    public enum NodeType {
        Mandatory = 1,
        Optional = 2,
        Or = 3,
        Xor = 4,
        Root = 5
    }
}
