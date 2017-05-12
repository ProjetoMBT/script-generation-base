using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.TestSuitStructure
{
    public class Rule
    {
        #region Constructor
        public Rule(string name)
        {
            this.name = name;
        }

        public Rule()
        {

        }
        #endregion

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string leftBoundary;

        public string LeftBoundary
        {
            get { return leftBoundary; }
            set { leftBoundary = value; }
        }

        private string rightBoundary;

        public string RightBoundary
        {
            get { return rightBoundary; }
            set { rightBoundary = value; }
        }

        private string prefix;

        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        private bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private SaveParameter parent;

        public SaveParameter Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private string action;

        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        private string order;

        public string Order
        {
            get { return order; }
            set { this.order = value; }
        }
    }
}
