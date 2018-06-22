using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class Trainer_Wizard : Form
    {
        public Form1 ziggyWin;
        private readonly TrainerValueInput inputDialog = new TrainerValueInput();

        public class Pokes
        {
            public byte bank;
            public int address;
            public int newVal;
            public int oldVal;
        }

        public class Trainer
        {
            public string name;
            public List<Pokes> pokeList = new List<Pokes>();
        }

        private readonly List<Trainer> TrainerList = new List<Trainer>();

        public void LoadTrainer(string filename) {
            pokesListBox.Items.Clear();
            TrainerList.Clear();
            using (FileStream fs = new FileStream(filename, FileMode.Open)) {
                StreamReader sr = new StreamReader(fs);
                string line;
                char[] delimiters = { '\r', '\n', ' ' };
                do {
                    line = sr.ReadLine();

                    if (line != null && line[0] == 'N') {
                        Trainer trainer = new Trainer { name = line.Substring(1, line.Length - 1) };
                        string[] fields = null;

                        do {
                            line = sr.ReadLine();
                            if (line != null)
                            {
                                fields = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                                Pokes poke = new Pokes
                                {
                                    bank = Convert.ToByte(fields[1]),
                                    address = Convert.ToInt32(fields[2]),
                                    newVal = Convert.ToInt32(fields[3]),
                                    oldVal = Convert.ToInt32(fields[4])
                                };
                                trainer.pokeList.Add(poke);
                            }
                        } while (fields != null && fields[0] != "Z");

                        pokesListBox.Items.Add(trainer.name);
                        TrainerList.Add(trainer);
                    }
                } while (line != null && line[0] != 'Y');
            }
        }

        private void ApplyTrainers() {
            for (int f = 0; f < TrainerList.Count; f++) {
                Trainer trainer = TrainerList[f];
                bool applyPokes = (pokesListBox.GetItemCheckState(f) == CheckState.Checked);

                for (int g = 0; g < trainer.pokeList.Count; g++) {
                    Pokes p = trainer.pokeList[g];

                    if (applyPokes && (p.newVal > 255)) {
                        inputDialog.Title = trainer.name;
                        inputDialog.ShowDialog();
                        p.newVal = inputDialog.PokeValue;
                    }

                    if (p.bank == 8) //48k
                    {
                        //Remove poke only if old value is a non-zero value
                        if (!applyPokes && (p.oldVal == 0))
                            continue;

                        ziggyWin.zx.PokeByteNoContend(p.address, (applyPokes ? p.newVal : p.oldVal));
                    }
                    else {
                        //Remove poke only if old value is a non-zero value
                        if (!applyPokes && (p.oldVal == 0))
                            continue;

                        ziggyWin.zx.PokeByteNoContend(p.address, (applyPokes ? p.newVal : p.oldVal));
                    }
                }
            }
        }

        public Trainer_Wizard(Form1 zw) {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
            ziggyWin = zw;
        }

        private void button1_Click(object sender, EventArgs e) {
            ApplyTrainers();
            Hide();
            MessageBox.Show("Selected pokes are now active.", "Pokes applied", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void button3_Click(object sender, EventArgs e) {
            openFileDialog1.Title = "Choose a .POK file";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = ".POK files|*.POK";

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                label3.Text = openFileDialog1.SafeFileName;
                LoadTrainer(openFileDialog1.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            Hide();
        }

        private void pokesListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (e.NewValue != CheckState.Checked) {
                CheckedListBox.CheckedIndexCollection selectedItems = pokesListBox.CheckedIndices;
                if (selectedItems.Count == 1) {
                    button1.Enabled = false;
                    return;
                }
            }
            button1.Enabled = true;
        }
    }
}