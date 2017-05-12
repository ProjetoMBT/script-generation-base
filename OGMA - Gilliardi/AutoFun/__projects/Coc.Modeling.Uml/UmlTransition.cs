using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Coc.Modeling.Uml
{
    public class UmlTransition : UmlAssociation
    {
        public UmlElement Source
        {
            get { return base.End1; }
            set { base.End1 = value; }
        }

        public UmlElement Target
        {
            get { return base.End2; }
            set { base.End2 = value; }
        }

        public override String ToString()
        {
            //return HttpUtility.UrlDecode(this.Source.ToString() + " -> " + this.Target.ToString() + (this.TaggedValues.Count > 0 ? " <Tags:" + TaggedValues.Count + ">" : ""));
            return this.Source.ToString() + " -> " + this.Target.ToString() + (this.TaggedValues.Count > 0 ? " <Tags:" + TaggedValues.Count + ">" : "");
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || ToString() != obj.ToString())
            {
                return false;
            }

            return obj.GetHashCode() == this.GetHashCode();
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Source.Name.GetHashCode()*(-3) + Target.Name.GetHashCode()*7;
        }
    }
}
