using System;
using System.Windows.Controls;
using Coc.Data.LoadRunner.SequenceModel;

namespace Coc.Data.LoadRunner.ParameterEditor
{
    public class CustomCheckbox : CheckBox
    {
        private ImageTreeViewItemCor parentWindow;

        public ImageTreeViewItemCor ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }

        private String type;

        public String Type
        {
            get { return type; }
            set { this.type = value; }
        }

        private SaveParameter relatedSaveParam;

        public SaveParameter RelatedSaveParam
        {
            get { return relatedSaveParam; }
            set { relatedSaveParam = value; }
        }

        private Rule relatedRule;

        public Rule RelatedRule
        {
            get { return relatedRule; }
            set { relatedRule = value; }
        }

        public CustomCheckbox(ImageTreeViewItemCor parent)
        {
            this.parentWindow = parent;
        }
    }
}