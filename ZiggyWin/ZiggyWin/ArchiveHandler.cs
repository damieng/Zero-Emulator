using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class ArchiveHandler : Form
    {
        public ZipArchiveEntry FileToOpen { get; set; }

        public ArchiveHandler(IEnumerable<ZipArchiveEntry> entries) {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new System.Drawing.Font(System.Drawing.SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
            listView1.Columns.Add("File");
            listView1.Columns.Add("Size (bytes)").TextAlign = HorizontalAlignment.Right;

            foreach (ZipArchiveEntry entry in entries) {
                ListViewItem listItem = new ListViewItem { Text = entry.FullName, Tag = entry };
                listItem.SubItems.Add(new ListViewItem.ListViewSubItem(listItem, entry.Length.ToString()));
                listView1.Items.Add(listItem);
            }
            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void button1_Click(object sender, EventArgs e) {
            FileToOpen = (ZipArchiveEntry)listView1.SelectedItems[0].Tag;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}