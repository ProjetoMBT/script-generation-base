using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.Uml;

namespace Coc.Data.Xmi.Script
{
    public class GroupNode
    {

        private GroupNode father;
        private GroupNode prevSibling;
        private GroupNode nextSibling;
        private string groupName;
        private List<UmlTransition> transitions;
        private List<GroupNode> subGroups;
        
        public GroupNode()
        {
            this.groupName = "";
            this.nextSibling = null;
            this.prevSibling = null;
            this.father = null;
            this.SubGroups = new List<GroupNode>();
            this.transitions = new List<UmlTransition>();
        }
        
        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }

        public GroupNode Father
        {
            get;
            set;
        }

        public GroupNode PrevSibling
        {
            get;
            set;
        }

        public GroupNode NextSibling
        {
            get;
            set;
        }


        public List<UmlTransition> Transitions
        {
            get { return transitions; }
            set { transitions = value; }
        }

        public List<GroupNode> SubGroups
        {
            get { return subGroups; }
            set { subGroups = value; }
        }




    }
}
