using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class PokeMemory : Form
    {
        private readonly Monitor monitorRef;

        public PokeMemory(Monitor m) {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
            monitorRef = m;
        }

        private void button1_Click(object sender, EventArgs e) {
            int addr = Utilities.ConvertToInt(textBox1.Text);
            int val = Utilities.ConvertToInt(textBox2.Text);
            
            if (addr > -1 && val > -1) {
                monitorRef.PokeByte(addr, val & 0xff);
                Close();
            }
        }
    }
}