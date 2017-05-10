using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace FeatureModelEditor.ConstraintEditor {
    class ConstraintGrid : Grid{

        private StackPanel innerGrid = new StackPanel(){
            Background = new SolidColorBrush(Colors.Transparent),
            VerticalAlignment = System.Windows.VerticalAlignment.Top,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            Orientation = Orientation.Horizontal
        };

        public ConstraintGrid() : base() {
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.Height = 30;
            this.Children.Add(innerGrid);
        }

        public StackPanel GetInnerContainer(){
            return this.Children.OfType<StackPanel>().FirstOrDefault();
        }

        internal string GetContraintAsString() {
            String s = "";

            foreach(TextBlock block in this.GetInnerContainer().Children.OfType<TextBlock>())
                s += " " + block.Text;

            return s.Trim();
        }
    }
}
