using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ZeroWin
{
    public partial class Options : Form
    {
        private readonly Form1 zwRef;
        private int currentModelIndex = -1;
        private bool stereoSound = true;
        private bool ayFor48k;
        private int joy1;
        private int joy2;

        #region Accessors

        public bool EnableKey2Joy {
            get => key2joyCheckBox.Checked;
            set => key2joyCheckBox.Checked = value;
        }

        public int Key2JoyStickType {
            get => key2joyComboBox.SelectedIndex;
            set => key2joyComboBox.SelectedIndex = value;
        }

        public int MouseSensitivity {
            get => (11 - mouseTrackBar.Value);
            set => mouseTrackBar.Value = 11 - value;
        }

        public bool EnableKempstonMouse {
            get => mouseCheckBox.Checked;
            set => mouseCheckBox.Checked = value;
        }

        public int Joystick1Choice {
            get => joystickComboBox1.SelectedIndex;
            set => joy1 = value;
        }

        public int Joystick2Choice {
            get => joystick2ComboBox1.SelectedIndex;
            set => joy2 = value;
        }

        public int Joystick1EmulationChoice {
            get => joystickComboBox2.SelectedIndex;
            set => joystickComboBox2.SelectedIndex = value;
        }

        public int Joystick2EmulationChoice {
            get => joystick2ComboBox2.SelectedIndex;
            set => joystick2ComboBox2.SelectedIndex = value;
        }

        public int EmulationSpeed {
            get => emulationSpeedTrackBar.Value;
            set => emulationSpeedTrackBar.Value = value;
        }

        public bool HighCompatibilityMode {
            get => Use128keCheckbox.Checked;
            set => Use128keCheckbox.Checked = value;
        }

        public bool RestoreLastState {
            get => lastStateCheckbox.Checked;
            set => lastStateCheckbox.Checked = value;
        }

        public bool ShowOnScreenLEDS {
            get => onScreenLEDCheckbox.Checked;
            set => onScreenLEDCheckbox.Checked = value;
        }

        public int SpeakerSetup {
            get {
                if (!stereoRadioButton.Checked)
                    return 0;

                if (acbRadioButton.Checked)
                    return 1;

                return 2;
            }
            set {
                if (value == 0) {
                    stereoRadioButton.Checked = false;
                    monoRadioButton.Checked = true;
                }
                else {
                    stereoRadioButton.Checked = true;
                    monoRadioButton.Checked = false;

                    if (value == 1) {
                        acbRadioButton.Checked = true;
                        abcRadioButton.Checked = false;
                    }
                    else {
                        acbRadioButton.Checked = false;
                        abcRadioButton.Checked = true;
                    }
                }
            }
        }

        public bool EnableStereoSound {
            get => stereoSound;
            set {
                stereoSound = value;
                stereoRadioButton.Checked = value;
                monoRadioButton.Checked = !value;
            }
        }

        public bool EnableAYFor48K {
            get => ayFor48k;
            set {
                ayFor48k = value;
                ayFor48kCheckbox.Checked = value;
            }
        }

        public String RomPath {
            get => romPathTextBox.Text;
            set => romPathTextBox.Text = value;
        }

        public String GamePath {
            get => gamePathTextBox.Text;
            set => gamePathTextBox.Text = value;
        }

        public String RomToUse48k { get; set; } = "";

        public String RomToUse128k { get; set; } = "";

        public String RomToUse128ke { get; set; } = "";
        public String RomToUsePlus3 { get; set; } = "";

        public String RomToUsePentagon { get; set; } = "";

        public bool FileAssociateSNA {
            get => snaCheckBox.Checked;
            set => snaCheckBox.Checked = value;
        }

        public bool FileAssociateSZX {
            get => szxCheckBox.Checked;
            set => szxCheckBox.Checked = value;
        }

        public bool FileAssociateZ80 {
            get => z80CheckBox.Checked;
            set => z80CheckBox.Checked = value;
        }

        public bool FileAssociatePZX {
            get => pzxCheckBox.Checked;
            set => pzxCheckBox.Checked = value;
        }

        public bool FileAssociateTZX {
            get => tzxCheckBox.Checked;
            set => tzxCheckBox.Checked = value;
        }

        public bool FileAssociateTAP {
            get => tapCheckBox.Checked;
            set => tapCheckBox.Checked = value;
        }

        public bool FileAssociateDSK {
            get => dskCheckBox.Checked;
            set => dskCheckBox.Checked = value;
        }

        public bool FileAssociateTRD {
            get => trdCheckBox.Checked;
            set => trdCheckBox.Checked = value;
        }

        public bool FileAssociateSCL {
            get => sclCheckBox.Checked;
            set => sclCheckBox.Checked = value;
        }

        public int SpectrumModel {
            get => modelComboBox.SelectedIndex;
            set => modelComboBox.SelectedIndex = value;
        }

        public bool InterlacedMode {
            get => interlaceCheckBox.Checked;
            set => interlaceCheckBox.Checked = value;
        }

        public bool PixelSmoothing {
            get => pixelSmoothingCheckBox.Checked;
            set => pixelSmoothingCheckBox.Checked = value;
        }

        public bool EnableVSync {
            get => vsyncCheckbox.Checked;
            set => vsyncCheckbox.Checked = value;
        }

        public bool UseIssue2Keyboard {
            get => issue2RadioButton.Checked;
            set {
                if (value) {
                    issue2RadioButton.Checked = true;
                    issue3radioButton.Checked = false;
                }
                else {
                    issue2RadioButton.Checked = false;
                    issue3radioButton.Checked = true;
                }
            }
        }

        public bool UseDirectX {
            get => directXRadioButton.Checked;
            set {
                directXRadioButton.Checked = interlaceCheckBox.Enabled = pixelSmoothingCheckBox.Enabled = vsyncCheckbox.Enabled = value;
                gdiRadioButton.Checked = !value;
            }
        }

        public int Palette {
            get => paletteComboBox.SelectedIndex;
            set => paletteComboBox.SelectedIndex = value;
        }

        public int borderSize {
            get => borderSizeComboBox.SelectedIndex;
            set => borderSizeComboBox.SelectedIndex = value;
        }

        public int windowSize {
            get => windowSizeComboBox.SelectedIndex;
            set => windowSizeComboBox.SelectedIndex = value;
        }

        public bool UseLateTimings {
            get => timingCheckBox.Checked;
            set => timingCheckBox.Checked = value;
        }

        public bool PauseOnFocusChange {
            get => pauseCheckBox.Checked;
            set => pauseCheckBox.Checked = value;
        }

        public bool ConfirmOnExit {
            get => exitConfirmCheckBox.Checked;
            set => exitConfirmCheckBox.Checked = value;
        }

        public bool MaintainAspectRatioInFullScreen {
            get => aspectRatioFullscreenCheckBox.Checked;
            set => aspectRatioFullscreenCheckBox.Checked = value;
        }

        public bool DisableTapeTraps {
            get => disableTapeTrapCheckbox.Checked;
            set => disableTapeTrapCheckbox.Checked = value;
        }

        public bool KempstonUsesPort1F {
            get => port1FCheckbox.Checked;
            set => port1FCheckbox.Checked = value;
        }
        #endregion Accessors

        public Options(Form1 parentRef) {
            InitializeComponent();
            // Set the default dialog font on each child control
            foreach (Control c in Controls) {
                c.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, c.Font.Size);
            }
            zwRef = parentRef;
        }

        private void Options_Load(object sender, EventArgs e) {
            Location = new Point(zwRef.Location.X + 20, zwRef.Location.Y + 20);
            if (UseDirectX) {
                interlaceCheckBox.Enabled = true;
                pixelSmoothingCheckBox.Enabled = true;
            }
            else {
                interlaceCheckBox.Enabled = false;
                pixelSmoothingCheckBox.Enabled = false;
            }
            button1.Enabled = Joystick1Choice > 0;
            button2.Enabled = Joystick2Choice > 0;
        }

        private void romBrowseButton_Click(object sender, EventArgs e) {
            openFileDialog1.InitialDirectory = RomPath;
            openFileDialog1.Title = "Choose a ROM";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "All supported files|*.rom;";

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                romTextBox.Text = openFileDialog1.SafeFileName;
                RomPath = Path.GetDirectoryName(openFileDialog1.FileName);

                switch (currentModelIndex) {
                    case 0:
                        RomToUse48k = romTextBox.Text;
                        break;

                    case 1:
                        RomToUse128k = romTextBox.Text;
                        break;

                    case 2:
                        RomToUse128ke = romTextBox.Text;
                        break;

                    case 3:
                        RomToUsePlus3 = romTextBox.Text;
                        break;

                    case 4:
                        RomToUsePentagon = romTextBox.Text;
                        break;
                }
            }
        }

        private void gamePathButton_Click(object sender, EventArgs e) {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                GamePath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void romPathButton_Click(object sender, EventArgs e) {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                String romInUse = "";
                switch (currentModelIndex) {
                    case 0:
                        romInUse = RomToUse48k;
                        break;

                    case 1:
                        romInUse = RomToUse128k;
                        break;

                    case 2:
                        romInUse = RomToUse128ke;
                        break;

                    case 3:
                        romInUse = RomToUsePlus3;
                        break;

                    case 4:
                        romInUse = RomToUsePentagon;
                        break;
                }
                if (!File.Exists(folderBrowserDialog1.SelectedPath + "\\" + romInUse)) {
                    MessageBox.Show("The current ROM couldn't be found in this path.\n\nEnsure this path is correct, or specify a new ROM \nin the Hardware section.",
                             "File Warning", MessageBoxButtons.OK,
                             MessageBoxIcon.Warning);
                }

                RomPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void defaultSettingsButton_Click(object sender, EventArgs e) {
            if (MessageBox.Show("This will cause you to lose all your current settings!\nAre you sure you want to revert to default settings?",
                          "Confirm settings reset", MessageBoxButtons.YesNo,
                          MessageBoxIcon.Question) == DialogResult.Yes) {

                ZeroConfig defCon = new ZeroConfig();
                RomToUse48k = defCon.Current48kROM;
                RomToUse128k = defCon.Current128kROM;
                RomToUse128ke = defCon.Current128keROM;
                RomToUsePlus3 = defCon.CurrentPlus3ROM;
                RomToUsePentagon = defCon.CurrentPentagonROM;
                //Don't revert path to defaults, since there is no default path.
                //Instead try to use application start up path.
                RomPath = Application.StartupPath + "\\roms";
                GamePath = Application.StartupPath + "\\programs";
                string model = defCon.CurrentSpectrumModel;
                switch (model) {
                    case "ZX Spectrum 48k":
                        SpectrumModel = 0;
                        break;

                    case "ZX Spectrum 128k":
                        SpectrumModel = 1;
                        break;

                    case "ZX Spectrum 128ke":
                        SpectrumModel = 2;
                        break;

                    case "ZX Spectrum +3":
                        SpectrumModel = 3;
                        break;

                    case "Pentagon 128k":
                        SpectrumModel = 4;
                        break;
                }
                UseDirectX = defCon.UseDirectX;
                borderSize = defCon.BorderSize;

                string paletteMode = defCon.PaletteMode;
                switch (paletteMode) {
                    case "Grayscale":
                        Palette = 1;
                        break;

                    case "ULA Plus":
                        Palette = 2;
                        break;

                    default:
                        Palette = 0;
                        break;
                }
                PauseOnFocusChange = defCon.PauseOnFocusLost;
                ConfirmOnExit = defCon.ConfirmOnExit;
                UseLateTimings = defCon.UseLateTimings;
                UseIssue2Keyboard = defCon.UseIssue2Keyboard;
                RestoreLastState = defCon.RestoreLastStateOnStart;
                EmulationSpeed = defCon.EmulationSpeed;
                ShowOnScreenLEDS = defCon.ShowOnscreenIndicators;
                Use128keCheckbox.Checked = defCon.HighCompatibilityMode;
                interlaceCheckBox.Checked = defCon.EnableInterlacedOverlay;
            }
        }

        private void modelComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            switch (currentModelIndex) {
                case 0:
                    RomToUse48k = romTextBox.Text;
                    break;

                case 1:
                    RomToUse128k = romTextBox.Text;
                    break;

                case 2:
                    RomToUse128ke = romTextBox.Text;
                    break;

                case 3:
                    RomToUsePlus3 = romTextBox.Text;
                    break;

                case 4:
                    RomToUsePentagon = romTextBox.Text;
                    break;
            }

            switch (modelComboBox.SelectedIndex) {
                case 0:
                    romTextBox.Text = RomToUse48k;
                    break;

                case 1:
                    romTextBox.Text = RomToUse128k;
                    break;

                case 2:
                    romTextBox.Text = RomToUse128ke;
                    break;

                case 3:
                    romTextBox.Text = RomToUsePlus3;
                    break;

                case 4:
                    romTextBox.Text = RomToUsePentagon;
                    break;
            }

            currentModelIndex = modelComboBox.SelectedIndex;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) {
        }

        private void stereoRadioButton_CheckedChanged(object sender, EventArgs e) {
            monoRadioButton.Checked = !stereoRadioButton.Checked;
        }

        private void monoRadioButton_CheckedChanged(object sender, EventArgs e) {
            stereoRadioButton.Checked = !monoRadioButton.Checked;
        }

        private void ayFor48kCheckbox_CheckedChanged(object sender, EventArgs e) {
            EnableAYFor48K = ayFor48kCheckbox.Checked;
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e) {
        }

        private void tabPage5_Enter(object sender, EventArgs e) {
            joystickComboBox1.Items.Clear();
            joystick2ComboBox1.Items.Clear();
            joystickComboBox1.Items.Add("None");
            joystick2ComboBox1.Items.Add("None");
            JoystickController.EnumerateJosticks();
            string[] devNames = JoystickController.GetDeviceNames();
            for (int f = 0; f < devNames.Length; f++) {
                joystickComboBox1.Items.Add(devNames[f]);
                joystick2ComboBox1.Items.Add(devNames[f]);
            }

            if (joystickComboBox1.SelectedIndex < 0)
                joystickComboBox1.SelectedIndex = 0;

            if (joystick2ComboBox1.SelectedIndex < 0)
                joystick2ComboBox1.SelectedIndex = 0;

            if (joystickComboBox2.SelectedIndex < 0)
                joystickComboBox2.SelectedIndex = 0;

            if (joystick2ComboBox2.SelectedIndex < 0)
                joystick2ComboBox2.SelectedIndex = 0;

            if (devNames.Length >= joy1)
                joystickComboBox1.SelectedIndex = joy1;

            if (devNames.Length >= joy2)
                joystick2ComboBox1.SelectedIndex = joy2;
        }

        private void joystick2ComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (joystick2ComboBox1.SelectedIndex == joystickComboBox1.SelectedIndex)
                joystickComboBox1.SelectedIndex = 0;

            button2.Enabled = Joystick2Choice > 0;
        }

        private void joystickComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (joystickComboBox1.SelectedIndex == joystick2ComboBox1.SelectedIndex)
                joystick2ComboBox1.SelectedIndex = 0;

            button1.Enabled = Joystick1Choice > 0;
        }

        private void emulationSpeedTrackBar_Scroll(object sender, EventArgs e) {
            toolTip1.SetToolTip(emulationSpeedTrackBar, "Speed  " + emulationSpeedTrackBar.Value + "%");
        }

        private void lastStateCheckbox_CheckedChanged(object sender, EventArgs e) {
        }

        private void pixelSmoothingCheckBox_CheckedChanged(object sender, EventArgs e) {
            PixelSmoothing = pixelSmoothingCheckBox.Checked;
        }

        private void directXRadioButton_CheckedChanged(object sender, EventArgs e) {
            interlaceCheckBox.Enabled = true;
            pixelSmoothingCheckBox.Enabled = true;
            vsyncCheckbox.Enabled = true;
        }

        private void gdiRadioButton_CheckedChanged(object sender, EventArgs e) {
            interlaceCheckBox.Enabled = false;
            pixelSmoothingCheckBox.Enabled = false;
            vsyncCheckbox.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (zwRef.joystick1.isInitialized)
                zwRef.joystick1.Release();
            zwRef.joystick1 = new JoystickController();
            zwRef.joystick1.InitJoystick(zwRef, Joystick1Choice - 1);
            JoystickRemap jsRemap = new JoystickRemap(zwRef, zwRef.joystick1);
            jsRemap.ShowDialog();
        }
    }
}