using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace FeatureModelEditor.ConstraintEditor {
    public class HyperlinkTextBlock : TextBlock {

        #region Constructors
        public HyperlinkTextBlock() : base(){
            this.prepare();
        }

        public HyperlinkTextBlock(string s) : base(){
            this.Text = s;
            this.prepare();
        }
        #endregion

        private void prepare(){
            //hand cursor, to denote click functionality
            this.Cursor = System.Windows.Input.Cursors.Hand; 
            this.Width = 130;
            this.Height = 26;
            //adjust margin and padding properties
            this.Margin = new System.Windows.Thickness(0);
            this.Padding = new System.Windows.Thickness(0);

            //adjust colors for font and background
            this.Background = new SolidColorBrush(Color.FromRgb(0x73, 0xA3, 0xC4));
            this.Foreground = new SolidColorBrush(Colors.White);

            //adds event handlers
            this.MouseEnter += new System.Windows.Input.MouseEventHandler(HyperlinkTextBlock_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(HyperlinkTextBlock_MouseLeave);

            //adjust aligments
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            this.TextAlignment = System.Windows.TextAlignment.Center;
        }

        /// <summary>
        /// Occurs when mouse leaves control.
        /// </summary>
        void HyperlinkTextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            this.Background = new SolidColorBrush(Color.FromRgb(0x73, 0xA3, 0xC4));
            this.Foreground = new SolidColorBrush(Colors.White);
        }

        void HyperlinkTextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            this.Foreground = new SolidColorBrush(Colors.White);
            this.Background = new SolidColorBrush(Color.FromRgb(0x99, 0xAA, 0x77));

        }

    }
}
