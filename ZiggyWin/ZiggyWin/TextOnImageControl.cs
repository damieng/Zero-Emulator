using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class TextOnImageControl : UserControl
    {
        public String header = null;
        public String text;

        public Point textAnchor = new Point(5, 35);

        public TextOnImageControl() {
            InitializeComponent();
        }

        public void SetText(String _text, Point anchor) {
            text = _text;
            textAnchor = new Point(anchor.X, anchor.Y);
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (BackgroundImage != null && text != null) {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                StringFormat strFormat = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near
                };

                e.Graphics.DrawString(header, new Font("Comic Sans MS", 14, FontStyle.Bold), Brushes.RosyBrown, new Point(5, 5));
                e.Graphics.DrawString(text, new Font("Comic Sans MS", 9), Brushes.DarkBlue, new RectangleF(textAnchor.X, textAnchor.Y, Width - textAnchor.X, Height - textAnchor.Y), strFormat);
            }
        }
    }
}