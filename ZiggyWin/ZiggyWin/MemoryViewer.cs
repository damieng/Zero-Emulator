using System.Drawing;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class MemoryViewer : Form
    {
        private readonly Monitor monitor;

        public MemoryViewer(Monitor _monitor) {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
            monitor = _monitor;

            //Set up the datagridview for memory
            dataGridView1.AutoGenerateColumns = false;

            DataGridViewTextBoxColumn dgridColAddress2 = new DataGridViewTextBoxColumn
            {
                HeaderText = "Address",
                Name = "Address",
                Width = 60,
                DataPropertyName = "Address"
            };
            dataGridView1.Columns.Add(dgridColAddress2);

            DataGridViewTextBoxColumn dgridColGetBytes = new DataGridViewTextBoxColumn
            {
                HeaderText = "Bytes",
                Name = "Bytes",
                Width = 280,
                DataPropertyName = "GetBytes"
            };
            dataGridView1.Columns.Add(dgridColGetBytes);

            DataGridViewTextBoxColumn dgridColGetChars = new DataGridViewTextBoxColumn
            {
                HeaderText = "Characters",
                Name = "Characters",
                Width = 115,
                DataPropertyName = "GetCharacters"
            };
            dataGridView1.Columns.Add(dgridColGetChars);

            dataGridView1.DataSource = monitor.memoryViewList;
        }

        public void RefreshData(bool isHex) {
            dataGridView1.DataSource = null;
            System.Threading.Thread.Sleep(1);
            dataGridView1.DataSource = monitor.memoryViewList;
            dataGridView1.Columns[0].DefaultCellStyle.Format = isHex ? "x2" : "";
        }

        private void MemoryViewer_FormClosing(object sender, FormClosingEventArgs e) {
            dataGridView1.DataSource = null;
        }
    }
}