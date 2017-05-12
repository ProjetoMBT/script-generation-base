using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace UmlComponentDiagramEditor
{
    public interface SelectObject
    {
        void SetPosition(Point p);
        Point GetPosition();
        void Select();
        void Unselect();
        string getName();
        void setName(string name);
        ConnectionPoint getSelectedPoint();
        void setSelectedPoint(ConnectionPoint newValue);
        ComponentDiagram getParentDiagram();
        void setRelatedComponent(SelectObject so);
    }
}
