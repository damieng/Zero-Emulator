using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace ZeroWin
{
    partial class AboutBox1 : Form
    {
        private readonly Form zwRef;
        private readonly Assembly currentAssembly = Assembly.GetExecutingAssembly();

        public AboutBox1(Form fref) {
            zwRef = fref;
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.SizeInPoints);
            }
        }

        public string AssemblyTitle => GetAttribute<AssemblyTitleAttribute>(a => a.Title) ?? Path.GetFileNameWithoutExtension(currentAssembly.CodeBase);
        public string AssemblyVersion => currentAssembly.GetName().Version.ToString();
        public string AssemblyDescription => GetAttribute<AssemblyDescriptionAttribute>(a => a.Description) ?? "";
        public string AssemblyProduct => GetAttribute<AssemblyProductAttribute>(a => a.Product) ?? "";
        public string AssemblyCopyright => GetAttribute<AssemblyCopyrightAttribute>(a => a.Copyright) ?? "";
        public string AssemblyCompany => GetAttribute<AssemblyCompanyAttribute>(a => a.Company) ?? "";

        private void AboutBox1_Load(object sender, EventArgs e) {
            Location = new Point(zwRef.Location.X + 20, zwRef.Location.Y + 20);
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
        }

        private string GetAttribute<T>(Func<T, String> property) where T : Attribute {
            return property(currentAssembly.GetCustomAttribute<T>());
        }
    }
}