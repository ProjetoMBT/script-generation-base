using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeatureModelEditor;
using PlugSpl.Atlas;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace FeatureModelEditor.Splot {
    public class SplotFileFormat : IFileFormat {

        public void SaveTo(AtlasFeatureModel atlas, string filename) {
            MessageBox.Show("Save function for SPLOT format was not implemented.");
        }

        public string GetFilter() {
            return "S.P.L.O.T. Feature Model (*.xml)|*.xml";
        }
        
        /// <summary>
        /// Parses an Atlas from an existing SPLOT xml format.
        /// </summary>
        /// <returns></returns>
        public AtlasFeatureModel LoadFrom(string filename) {
            
            //loads XML document.
            XmlDocument document = new XmlDocument();
            document.Load(filename);

            //select tree node.
            XmlNodeList foundNodes = document.SelectNodes("//feature_tree");
            
            if(foundNodes.Count != 1)
                throw new Exception("Unable to parse document.");

            //tree content
            String treeInfo = foundNodes[0].InnerText; 
            String[] rawNodes = treeInfo.Split('\n');

            //remove dummy windows line-break characters
            if(rawNodes[0].StartsWith("\r")) 
                for(int i = 0; i < rawNodes[i].Count(); i++)
                    rawNodes[i] = rawNodes[i].Replace("\r","");

            //convert strings to nodes (tree)
            Stack<Node> nodeStack = new Stack<Node>();
            
            
            foreach(String stringNode in rawNodes){

                if(stringNode.Trim() == string.Empty)
                    continue;

                Node n = stringNode.ToNode();

                //root node
                if(nodeStack.Count == 0){
                    nodeStack.Push(n);
                    continue;
                }
                
                if(nodeStack.Peek().Depth < n.Depth){ //is subnode
                    nodeStack.Peek().Children.Add(n);
                    nodeStack.Push(n);
                }else if(nodeStack.Peek().Depth == n.Depth){ // same-level node
                    nodeStack.Pop();
                    nodeStack.Peek().Children.Add(n);
                    nodeStack.Push(n);
                }else{ //uncle-node -> pops it
                    while(nodeStack.Peek().Depth > n.Depth)
                        nodeStack.Pop();
                    nodeStack.Peek().Children.Add(n);
                    nodeStack.Push(n);
                }
            }

            
            AtlasFeatureModel atlas = new AtlasFeatureModel();
            AtlasFeature root = this.buildFeature(nodeStack.LastOrDefault(), null, ref atlas);
            return atlas;


        }

        private AtlasFeature buildFeature(Node node, AtlasFeature parent, ref AtlasFeatureModel atlas) {
            AtlasFeature f = new AtlasFeature(node.Name);
            atlas.AddFeature(f);
            
            //children
            foreach(Node n in node.Children){

                AtlasFeature ff = buildFeature(n, f, ref atlas);
                AtlasConnection con = null; 

                switch (n.Type){
                    case NodeType.Mandatory:
                    case NodeType.Root:
                        con = new AtlasConnection(f, ff, AtlasConnectionType.Mandatory);
                        break;
                    case NodeType.Optional:
                        con = new AtlasConnection(f, ff, AtlasConnectionType.Optional);
                        break;
                    case NodeType.Or:
                        con = new AtlasConnection(f, ff, AtlasConnectionType.OrRelation);
                        break;
                    case NodeType.Xor:
                        con = new AtlasConnection(f, ff, AtlasConnectionType.Alternative);
                        break;
                }
                atlas.AddConnection(con);
            }

            return f;
        }

    }

    public static class StringExtensions
    {
        public static Node ToNode(this String str)
        {
            Node n = new Node();
            
            n.Depth = str.Split('\t').Count() - 1;
            n.Name = str.Split(' ')[1].Split('[')[0].Split('(')[0].Replace("\t","");

            str = str.Replace(":","");
            str = str.Trim();            

            switch(str.Split(' ')[0]){
                case "o": 
                    n.Type = NodeType.Optional;
                    break;
                case "m":
                    n.Type = NodeType.Mandatory;
                    break;
                case "g":
                    if(str.Split('[')[1].Contains('*'))
                        n.Type = NodeType.Or;
                    else
                        n.Type = NodeType.Or;
                    break;
                default:
                    n.Type = NodeType.Root;
                    break;
            }

            return n;
        }
    }   
}
