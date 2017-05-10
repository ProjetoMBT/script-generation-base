using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Data.Xmi.Script
{
    public class TabHelper
    {

        private int tabCount;
        private string tabText;

        public TabHelper(int tabNum)
        {
            this.tabCount = tabNum;
            this.tabText = "";
        }

        public void incrementTabs(){
            tabCount++;
            updateTabText();
        }

        public void decrementTabs(){
            tabCount--;
            updateTabText();
        }

        private void updateTabText(){
            tabText = new String('\t', tabCount);
        }

        public int TabCount
        {
            get { return tabCount; }
            set { 
                tabCount = value;
                updateTabText();
            }
        }

        public string TabText
        {
            get { return tabText; }
        }

    }
}
