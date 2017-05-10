using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.FiniteStateMachine
{
    public class StateNode
    {
        private State state;
        private String[] preamble;
        private List<StateNode> children;

        public State State { get { return state; } }
        public String[] Preamble { get { return preamble; } }
        public List<StateNode> Children { get { return children; } }

        private StateNode()
        {
            this.state = null;
            this.preamble = null;
            this.children = null;
        }

        public StateNode(State aState, String[] aPreamble)
            : this()
        {
            this.state = aState;
            this.preamble = aPreamble;
            this.children = new List<StateNode>();
        }
        public StateNode(State aState, String[] aPreamble, List<StateNode> theChildren)
            : this(aState, aPreamble)
        {
            this.AddChildren(theChildren);
        }

        public StateNode getState(State s)
        {
            StateNode x = null;
            if (state.Name.Equals(s.Name))
            {
                return this;
            }
            else if (children.Count > 0)
            {
                foreach (StateNode sn in children)
                {
                    x = sn.getState(s);
                    if (x != null)
                    {
                        return x;
                    }
                }
            }
            return null;
        }

        public void AddChildren(List<StateNode> newChildren)
        {
            this.children.AddRange(newChildren);
        }

        public override string ToString()
        {
            return write(this, "");
        }

        private string write(StateNode s, string p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(p + s.state.Name + " {" + arrayToString(s.preamble) + "}");
            foreach (StateNode c in s.children)
            {
                sb.Append(Environment.NewLine + write(c, p + "\t"));
            }
            return sb.ToString();
        }

        private string arrayToString(string[] p)
        {
            string ret = "";
            if (p.Length > 0)
                ret = p[0];
            for (int i = 1; i < p.Length; i++)
                ret += "," + p[i];
            return ret;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (!(obj is StateNode))
                return false;

            StateNode o = (StateNode)obj;
            if (!o.State.Equals(this.State))
                return false;
            if (o.Preamble.Length != this.Preamble.Length)
                return false;
            for (int i = 0; i < this.preamble.Length; i++)
                if (this.preamble[i] != o.preamble[i])
                    return false;
            /*
            if (o.children.Count != this.children.Count)
                return false;
            foreach (StateNode s in this.children)
                if (!o.children.Contains(s))
                    return false;
            //*/
            return true;
        }
    }
}
