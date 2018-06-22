using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ZeroWin.Properties;

namespace ZeroWin
{
    public partial class Profiler : Form
    {
        private readonly Monitor monitor;

        public Profiler(Monitor _monitor) {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
            monitor = _monitor;

            dataGridView3.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn dgridColLogAddress = new DataGridViewTextBoxColumn
            {
                HeaderText = "Address",
                Name = "Address",
                DataPropertyName = "Address"
            };
            dataGridView3.Columns.Add(dgridColLogAddress);

            DataGridViewTextBoxColumn dgridColLogTstates = new DataGridViewTextBoxColumn
            {
                HeaderText = "T-State",
                Name = "Tstates",
                DataPropertyName = "Tstates"
            };
            dataGridView3.Columns.Add(dgridColLogTstates);

            DataGridViewTextBoxColumn dgridColLogInstructions = new DataGridViewTextBoxColumn
            {
                HeaderText = "Instruction",
                Name = "Opcodes",
                DataPropertyName = "Opcodes"
            };
            dataGridView3.Columns.Add(dgridColLogInstructions);
            dataGridView3.DataSource = monitor.logList;

            if (monitor.logList.Count == 0) {
                clearButton.Enabled = false;
                saveButton.Enabled = false;
            } else {
                clearButton.Enabled = true;
                saveButton.Enabled = true;
                if (monitor.isTraceOn) {
                    traceButton.Image = Resources.logStop;
                    traceButton.Text = "Stop";
                }
            }
        }

        public void RefreshData() {
            dataGridView3.DataSource = null;
            Thread.Sleep(1);
            dataGridView3.DataSource = monitor.logList;
        }

        private void saveButton_Click(object sender, EventArgs e) {
            try {
                saveFileDialog1.Title = "Save Log";
                saveFileDialog1.FileName = "trace.log";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    if (monitor.useHexNumbers) {
                        sw.WriteLine("All numbers in hex.");
                        sw.WriteLine("-------------------");
                    } else {
                        sw.WriteLine("All numbers in decimal.");
                        sw.WriteLine("-----------------------");
                    }
                    foreach (Monitor.LogMessage log in monitor.logList) {
                        sw.WriteLine("{0,-5}   {1,-5}   {2,-20}", log.Address, log.Tstates, log.Opcodes);
                    }
                    sw.Close();
                }
            } catch {
                MessageBox.Show("Zero was unable to create a file! Either the disk is full, or there is a problem with access rights to the folder or something else entirely!",
                        "File Write Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void traceButton_Click(object sender, EventArgs e) {
            if (monitor.isTraceOn) {
                monitor.isTraceOn = false;
                traceButton.Image = Resources.logStart;
                traceButton.Text = "Start";
                if (monitor.logList.Count > 0) {
                    saveButton.Enabled = true;
                    clearButton.Enabled = true;
                }
            } else {
                monitor.logList.Clear();
                monitor.isTraceOn = true;
                traceButton.Image = Resources.logStop;
                traceButton.Text = "Stop";
                MessageBox.Show("Logging of calls will start once you press Play in the debugger window.", "Ready to trace", MessageBoxButtons.OK);
            }
        }

        private void clearButton_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you wish to clear the execution log?", "Clear Log", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                monitor.logList.Clear();
                dataGridView3.Refresh();
                clearButton.Enabled = false;
                saveButton.Enabled = false;
            }
        }
    }
}