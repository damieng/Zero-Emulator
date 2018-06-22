using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class TrainerValueInput : Form
    {
        public int PokeValue {
            get {
                if (maskedTextBox1.Text == "")
                    maskedTextBox1.Text = "0000";
                return Convert.ToInt32(maskedTextBox1.Text);
            }
        }

        public string Title {
            set => Text = value;
        }

        public TrainerValueInput() {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
        }
    }
}