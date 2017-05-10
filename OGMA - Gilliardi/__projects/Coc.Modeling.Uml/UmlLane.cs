using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.Uml
{
    public class UmlLane : UmlBase, IComparable
    {

        private List<UmlElement> elements;

        //trocar (UmlDimension)
        public List<UmlLane> ListLane { get; set; }

        [Obsolete("Use it as read only.")]
        public IEnumerable<UmlElement> Elements
        {
            get
            {
                return this.elements;
            }
        }

        /// <summary>
        /// Adds an element to list.
        /// </summary>
        /// <param name="element">Element to be added.</param>
        public void AddElement(UmlElement element)
        {
            this.elements.Add(element);
            ((UmlActionState)element).ParentLane = this;
        }

        //TODO
        public void RemoveElement(UmlElement element)
        {

        }

        public void ClearElements()
        {
            this.elements.Clear();
        }

        public List<UmlElement> GetElements()
        {
            return elements;
        }

        public UmlLane()
        {
            this.elements = new List<UmlElement>();
            ListLane = new List<UmlLane>();
        }

        public int Index { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is UmlLane)
            {
                UmlLane obj1 = (UmlLane)obj;
                return this.Index - obj1.Index;
            }
            else
            {
                return int.MinValue;
            }
        }

        public override string ToString()
        {
            return this.Name + (this.elements.Count > 0 ? " <Elements:" + this.elements.Count + ">" : "");
        }
    }
}
