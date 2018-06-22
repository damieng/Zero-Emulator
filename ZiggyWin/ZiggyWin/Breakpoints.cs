using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Speccy;

namespace ZeroWin
{
    public partial class Breakpoints : Form
    {
        private readonly Monitor monitor;

        public Breakpoints(Monitor _monitor) {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }

            monitor = _monitor;
            dataGridView2.ColumnHeadersBorderStyle = Monitor.ProperColumnHeadersBorderStyle;

            //Define Header Style
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = DefaultBackColor,
                Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.WindowText,
                SelectionBackColor = DefaultBackColor,
                SelectionForeColor = SystemColors.WindowText,
                WrapMode = DataGridViewTriState.False
            };
            dataGridView2.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;

            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = SystemColors.ControlLightLight,
                Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.WindowText,
                SelectionBackColor = SystemColors.Highlight,
                SelectionForeColor = SystemColors.HighlightText
            };

            dataGridView2.DefaultCellStyle = dataGridViewCellStyle3;

            //Set up the datagridview for breakpoints
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewTextBoxColumn dgrid2ColCondition = new DataGridViewTextBoxColumn
            {
                HeaderText = "Condition",
                Name = "Condition",
                Width = 141,
                DataPropertyName = "Condition"
            };
            dataGridView2.Columns.Add(dgrid2ColCondition);

            DataGridViewTextBoxColumn dgrid2ColAddress = new DataGridViewTextBoxColumn
            {
                HeaderText = "Address",
                Name = "Address",
                Width = 141,
                DataPropertyName = "AddressAsString"
            };
            dataGridView2.Columns.Add(dgrid2ColAddress);

            DataGridViewTextBoxColumn dgrid3ColData = new DataGridViewTextBoxColumn
            {
                HeaderText = "Value",
                Name = "Data",
                Width = 141,
                DataPropertyName = "DataAsString"
            };
            dataGridView2.Columns.Add(dgrid3ColData);

            dataGridView2.DataSource = monitor.breakPointConditions;

            //Setup the listbox for valid breakpoint registers
            foreach (SPECCY_EVENT speccyEvent in Utilities.EnumToList<SPECCY_EVENT>())
                comboBox2.Items.Add(Utilities.GetStringFromEnum(speccyEvent));

            comboBox2.SelectedIndex = 0;
            comboBox2_SelectedIndexChanged(this, null); //sanity check for case ULA port breakpoints are selected
        }

        public void RefreshView(bool isHexView)
        {
            dataGridView2.Columns[1].DefaultCellStyle.Format = isHexView ? "x2" : "";
        }

        private void clearSelectedButton_Click(object sender, EventArgs e) {
            DataGridViewSelectedRowCollection rowCollection = dataGridView2.SelectedRows;
            if (rowCollection.Count < 1)
                return;

            foreach (DataGridViewRow row in rowCollection) {
                //convert dashes (-) to -1 (int) where required,
                //else convert the actual value to int.
                int _addr = -1;
                int _val = -1;
                if ((String)row.Cells[1].Value != "-")
                    _addr = Convert.ToInt32(row.Cells[1].Value);

                if ((String)row.Cells[2].Value != "-")
                    _val = Convert.ToInt32(row.Cells[2].Value);

                SPECCY_EVENT speccyEvent = Utilities.GetEnumFromString((string)row.Cells[0].Value, SPECCY_EVENT.OPCODE_PC);
                KeyValuePair<SPECCY_EVENT, Monitor.BreakPointCondition> kv = new KeyValuePair<SPECCY_EVENT, Monitor.BreakPointCondition>(speccyEvent, new Monitor.BreakPointCondition(speccyEvent, _addr, _val));
                monitor.RemoveBreakpoint(kv);
            }
        }

        private void clearAllButton_Click(object sender, EventArgs e) {
            monitor.RemoveAllBreakpoints();
        }

        private void addOtherBreakpointButton_Click(object sender, EventArgs e) {
            if ((comboBox2.SelectedIndex < 0) || (comboBox2.SelectedIndex < 14 && maskedTextBox2.Text.Length < 1))
                return;

            int addr = -1;
            int val;

            SPECCY_EVENT speccyEvent = Utilities.GetEnumFromString(comboBox2.Text, SPECCY_EVENT.OPCODE_PC);

            if (comboBox2.SelectedIndex < 14) {
                addr = Utilities.ConvertToInt(maskedTextBox2.Text);

                if (addr > 65535) {
                    MessageBox.Show("The address is not within 0 to 65535!", "Invalid input", MessageBoxButtons.OK);
                    return;
                }
            }
            else if (speccyEvent == SPECCY_EVENT.ULA_WRITE || speccyEvent == SPECCY_EVENT.ULA_READ)
                addr = 254; //0xfe

            if (maskedTextBox3.Text.Length > 0) {
                val = Utilities.ConvertToInt(maskedTextBox3.Text);

                if (val > 255) {
                    MessageBox.Show("The value is not within 0 to 255!", "Invalid input", MessageBoxButtons.OK);
                    return;
                }
            }
            else
                val = -1;

            string _str = comboBox2.SelectedItem.ToString();// +"@" + addr.ToString();
            SPECCY_EVENT speccEventFromString = Utilities.GetEnumFromString(_str, SPECCY_EVENT.OPCODE_PC);

            KeyValuePair<SPECCY_EVENT, Monitor.BreakPointCondition> kv = new KeyValuePair<SPECCY_EVENT, Monitor.BreakPointCondition>(speccEventFromString, new Monitor.BreakPointCondition(speccEventFromString, addr, val));

            monitor.AddBreakpoint(kv);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            maskedTextBox3.Text = "";
            SPECCY_EVENT speccyEvent = Utilities.GetEnumFromString(comboBox2.Text, SPECCY_EVENT.OPCODE_PC);
            if (speccyEvent == SPECCY_EVENT.ULA_WRITE || speccyEvent == SPECCY_EVENT.ULA_READ) {
                maskedTextBox2.Text = "$fe";
                maskedTextBox2.ReadOnly = true;
                maskedTextBox3.ReadOnly = false;
            }
            else if (speccyEvent == SPECCY_EVENT.INTERRUPT || speccyEvent == SPECCY_EVENT.RE_INTERRUPT) {
                maskedTextBox2.Text = "";

                maskedTextBox3.ReadOnly = true;
                maskedTextBox2.ReadOnly = true;
            }
            else {
                maskedTextBox3.ReadOnly = false;
                maskedTextBox2.ReadOnly = false;
            }
        }
    }
}