using System.Drawing;
using System.Windows.Forms;

namespace ZeroWin
{
    public sealed partial class ScrollableLabel : ScrollableControl
    {
        public Label scrollLabel = new Label();

        public ScrollableLabel() {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            AutoScroll = true;
            scrollLabel.AutoSize = true;
            Controls.Add(scrollLabel);
            scrollLabel.Font = new Font("Tahoma", 8);
            HScroll = false;
            AutoScrollMargin = new Size(1, 1);
        }
    }
}