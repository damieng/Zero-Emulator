using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class CallStackViewer : Form
    {
        public CallStackViewer() {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }

            //Set up the datagridview for memory
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.RowHeadersVisible = false;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
            dataGridView1.ClearSelection();
            dataGridView1.Rows[0].Selected = true;
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
            dataGridView1.ClearSelection();
            if (dataGridView1.Rows.Count > 0)
                dataGridView1.Rows[0].Selected = true;
        }

        public void RefreshView() {
            dataGridView1.DataSource = null;
            Thread.Sleep(1);
            dataGridView1.Invalidate();
        }
    }
}