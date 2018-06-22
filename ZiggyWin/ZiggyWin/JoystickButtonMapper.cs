using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class JoystickButtonMapper : Form
    {
        public JoystickButtonMapper() {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
        }
    }
}