﻿using System.Drawing;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class Machine_State : Form
    {
        public Machine_State() {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
        }

        public void RefreshView(Form1 ziggyWin) {
            Text = ziggyWin.config.CurrentSpectrumModel;
            tStateLabel.Text = ziggyWin.zx.totalTStates.ToString();
            frameLengthLabel.Text = ziggyWin.zx.FrameLength.ToString();
            pagingCheckBox.Checked = !ziggyWin.zx.pagingDisabled;
            shadowScreenCheckBox.Checked = ziggyWin.zx.showShadowScreen;
            page1Label.Text = ziggyWin.zx.BankInPage0;
            page2Label.Text = ziggyWin.zx.BankInPage1;
            page3Label.Text = ziggyWin.zx.BankInPage2;
            page4Label.Text = ziggyWin.zx.BankInPage3;
            contendedLabel.Text = ziggyWin.zx.contendedBankPagedIn ? "contended" : "";

            switch (ziggyWin.config.CurrentSpectrumModel) {
                case "ZX Spectrum 48k":
                    romNameLabel.Text = ziggyWin.config.Current48kROM;
                    page2Label.Text = "-----";
                    page3Label.Text = "-----";
                    break;

                case "ZX Spectrum 128ke":
                    romNameLabel.Text = ziggyWin.config.Current128keROM;
                    break;

                case "ZX Spectrum 128k":
                    romNameLabel.Text = ziggyWin.config.Current128kROM;
                    break;

                case "ZX Spectrum +3":
                    romNameLabel.Text = ziggyWin.config.CurrentPlus3ROM;
                    break;
            }
        }
    }
}