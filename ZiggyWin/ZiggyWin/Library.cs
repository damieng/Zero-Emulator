using System;
using System.IO;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class ZLibrary : Form
    {
        public ZLibrary() {
            InitializeComponent();
        }

        private void scanButton_Click(object sender, EventArgs e) {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                ScanFolder(folderBrowserDialog1.SelectedPath);
            }
        }

        private void ScanFolder(string folder) {
            try {
                string[] filenames = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                char[] delimiters = { '(', ')', '[', ']' };
                foreach (string s in filenames) {
                    string[] filename = Path.GetFileName(s).Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    string name = filename[0];
                    string year = "";
                    string pub = "";
                    int offset = 0;
                    if (filename.Length > 1) {
                        if (filename[1].Contains("demo".ToLower())) {
                            name += filename[1];
                            offset++;
                        }
                    }
                    if (filename.Length > 2)
                        year = filename[1 + offset];
                    if (filename.Length > 3)
                        pub = filename[2 + offset];
                    Console.WriteLine(name + " Year: " + year + " publisher: " + pub);
                }
            }
            catch (UnauthorizedAccessException UAEx) {
                MessageBox.Show(UAEx.Message, "Error", MessageBoxButtons.OK);
            }
            catch (PathTooLongException PathEx) {
                MessageBox.Show(PathEx.Message, "Error", MessageBoxButtons.OK);
            }
        }
    }
}