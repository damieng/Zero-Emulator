using System.Windows.Forms;
using Speccy;
using ZeroWin.Properties;

namespace ZeroWin
{
    public class ZeroConfig
    {
        public string ApplicationPath { get; set; } = "";

        public bool HighCompatibilityMode { get; set; }

        public bool EnableInterlacedOverlay { get; set; }

        public MachineModel Model { get; set; } = MachineModel._48k;

        public int Volume { get; set; } = 50;

        public bool MuteSound { get; set; }

        public int StereoSoundOption { get; set; } = 1;

        public int WindowSize { get; set; }

        public int FullScreenWidth { get; set; } = 800;

        public int FullScreenHeight { get; set; } = 600;
        public bool FullScreenFormat16 { get; set; }

        public bool FullScreen { get; set; }

        public string PathRoms { get; set; } = @"\roms\";

        public string PathGames { get; set; } = @"\programs\";

        public string PathGameSaves { get; set; } = @"\saves\";

        public string PathScreenshots { get; set; } = @"\screenshots\";

        public string PathCheats { get; set; } = @"\cheats\";

        public string PathInfos { get; set; } = @"\info\";

        public string Current48kROM { get; set; } = @"48k.rom";

        public string current128kRom = @"128k.rom";

        public string Current128kROM {
            get => current128kRom;
            set => current128kRom = value;
        }

        public string current128keRom = @"128ke.rom";

        public string Current128keROM {
            get => current128keRom;
            set => current128keRom = value;
        }

        public string currentPlus3Rom = @"plus3.rom";

        public string CurrentPlus3ROM {
            get => currentPlus3Rom;
            set => currentPlus3Rom = value;
        }

        public string currentPentagonRom = @"pentagon.rom";

        public string CurrentPentagonROM {
            get => currentPentagonRom;
            set => currentPentagonRom = value;
        }

        public string currentModel = "ZX Spectrum 48k";

        public string CurrentSpectrumModel {
            get => currentModel;
            set => currentModel = value;
        }

        public string PaletteMode { get; set; } = "Normal";

        public int EmulationSpeed { get; set; } = 100;

        public bool UseLateTimings { get; set; }

        public bool UseIssue2Keyboard { get; set; }

        public bool UseDirectX { get; set; } = true;

        public bool AccociateCSWFiles { get; set; }

        public bool AccociatePZXFiles { get; set; }

        public bool AccociateTZXFiles { get; set; }

        public bool AccociateTAPFiles { get; set; }

        public bool AccociateSNAFiles { get; set; }

        public bool AccociateSZXFiles { get; set; }

        public bool AccociateZ80Files { get; set; }

        public bool AccociateDSKFiles { get; set; }

        public bool AccociateTRDFiles { get; set; }

        public bool AccociateSCLFiles { get; set; }

        public bool PauseOnFocusLost { get; set; } = true;

        public bool ConfirmOnExit { get; set; } = true;

        public bool EnableSound { get; set; } = true;

        public bool FullSpeedEmulation { get; set; }

        public int BorderSize { get; set; }

        public bool EnableAYFor48K { get; set; }

        public bool TapeAutoStart { get; set; } = true;

        public bool TapeAutoLoad { get; set; } = true;

        public bool TapeEdgeLoad { get; set; } = true;

        public bool TapeAccelerateLoad { get; set; } = true;

        public bool TapeInstaLoad { get; set; } = true;

        public bool DisableTapeTraps { get; set; }

        public bool EnableKempstonMouse { get; set; }

        public int MouseSensitivity { get; set; } = 3;

        public bool EnableKey2Joy { get; set; }

        public int Key2JoystickType { get; set; }

        public int Joystick1ToEmulate { get; set; }

        public int Joystick2ToEmulate { get; set; }
        public string Joystick1Name { get; set; } = "";
        public string Joystick2Name { get; set; } = "";
        public bool KempstonUsesPort1F {
            get;
            set;
        }

        public bool ShowOnscreenIndicators { get; set; } = true;

        public bool RestoreLastStateOnStart { get; set; }

        public bool EnablePixelSmoothing { get; set; }

        public bool EnableVSync { get; set; }

        public bool MaintainAspectRatioInFullScreen { get; set; } = true;

        public void Load() {
            Current48kROM = Settings.Default.ROM48k;
            current128kRom = Settings.Default.ROM128k;
            current128keRom = Settings.Default.ROM128ke;
            currentPlus3Rom = Settings.Default.ROMPlus3;
            currentPentagonRom = Settings.Default.ROMPentagon;
            PathRoms = Settings.Default.PathROM;
            PathGames = Settings.Default.PathPrograms;
            if (PathGames == "") {
                PathGames = Application.StartupPath + "\\programs";
            }
            PathScreenshots = Settings.Default.PathScreenshots;
            PathGameSaves = Settings.Default.PathSaves;
            PathInfos = Settings.Default.PathInfos;
            PathCheats = Settings.Default.PathCheats;
            currentModel = Settings.Default.Model;
            UseDirectX = Settings.Default.UseDirectX;
            FullScreen = Settings.Default.StartFullscreen;
            MaintainAspectRatioInFullScreen = Settings.Default.MaintainAspectRatioInFullScreen;
            WindowSize = Settings.Default.WindowSize;
            FullScreenWidth = Settings.Default.FullScreenWidth;
            FullScreenHeight = Settings.Default.FullScreenHeight;
            FullScreenFormat16 = Settings.Default.FullScreenFormat16;
            WindowSize = Settings.Default.WindowSize;
            PaletteMode = Settings.Default.Palette;
            BorderSize = Settings.Default.BorderSize;
            EnableInterlacedOverlay = Settings.Default.Interlaced;
            EnablePixelSmoothing = Settings.Default.PixelSmoothing;
            EnableVSync = Settings.Default.EnableVSync;
            Volume = Settings.Default.Volume;
            MuteSound = Settings.Default.Mute;
            EnableAYFor48K = Settings.Default.AySoundFor48k;
            StereoSoundOption = Settings.Default.SpeakerSetup;
            HighCompatibilityMode = Settings.Default.HighCompatabilityMode;
            PauseOnFocusLost = Settings.Default.PauseOnFocusChange;
            ShowOnscreenIndicators = Settings.Default.ShowOnScreenLEDs;
            RestoreLastStateOnStart = Settings.Default.RestoreLastStateOnStart;
            ConfirmOnExit = Settings.Default.ConfirmOnExit;
            UseLateTimings = Settings.Default.TimingModel;
            UseIssue2Keyboard = Settings.Default.Issue2Keyboard;
            EmulationSpeed = Settings.Default.EmulationSpeed;
            AccociateSZXFiles = Settings.Default.FileAssociationSZX;
            AccociateSNAFiles = Settings.Default.FileAssociationSNA;
            AccociateZ80Files = Settings.Default.FileAssociationZ80;
            AccociateTZXFiles = Settings.Default.FileAssociationTZX;
            AccociatePZXFiles = Settings.Default.FileAssociationPZX;
            AccociateTAPFiles = Settings.Default.FileAssociationTAP;
            AccociateDSKFiles = Settings.Default.FileAssociationDSK;
            AccociateTRDFiles = Settings.Default.FileAssociationTRD;
            AccociateSCLFiles = Settings.Default.FileAssociationSCL;
            TapeAutoStart = Settings.Default.TapeAutoStart;
            TapeAutoLoad = Settings.Default.TapeAutoLoad;
            TapeEdgeLoad = Settings.Default.TapeEdgeLoad;
            TapeAccelerateLoad = Settings.Default.TapeAccelerateLoad;
            TapeInstaLoad = Settings.Default.TapeInstaLoad;
            DisableTapeTraps = Settings.Default.DisableTapeTraps;
            EnableKempstonMouse = Settings.Default.KempstonMouse;
            MouseSensitivity = Settings.Default.MouseSensitivity;
            EnableKey2Joy = Settings.Default.Key2Joy;
            Key2JoystickType = Settings.Default.Key2JoystickType;
            Joystick1Name = Settings.Default.joystick1;
            Joystick2Name = Settings.Default.joystick2;
            Joystick1ToEmulate = Settings.Default.joystick1ToEmulate;
            Joystick2ToEmulate = Settings.Default.joystick2ToEmulate;
            KempstonUsesPort1F = Settings.Default.KempstonUsesPort1F;
        }

        public void Save() {
            Settings.Default.ROM48k = Current48kROM;
            Settings.Default.ROM128k = current128kRom;
            Settings.Default.ROM128ke = current128keRom;
            Settings.Default.ROMPlus3 = currentPlus3Rom;
            Settings.Default.ROMPentagon = currentPentagonRom;
            Settings.Default.PathROM = PathRoms;
            Settings.Default.PathPrograms = PathGames;
            Settings.Default.PathScreenshots = PathScreenshots;
            Settings.Default.PathSaves = PathGameSaves;
            Settings.Default.PathInfos = PathInfos;
            Settings.Default.PathCheats = PathCheats;
            Settings.Default.Model = currentModel;
            Settings.Default.UseDirectX = UseDirectX;
            Settings.Default.EnableVSync = EnableVSync;
            Settings.Default.StartFullscreen = FullScreen;
            Settings.Default.MaintainAspectRatioInFullScreen = MaintainAspectRatioInFullScreen;
            Settings.Default.WindowSize = (byte)WindowSize;
            Settings.Default.FullScreenWidth = FullScreenWidth;
            Settings.Default.FullScreenHeight = FullScreenHeight;
            Settings.Default.FullScreenFormat16 = FullScreenFormat16;
            Settings.Default.Palette = PaletteMode;
            Settings.Default.BorderSize = (byte)BorderSize;
            Settings.Default.Interlaced = EnableInterlacedOverlay;
            Settings.Default.PixelSmoothing = EnablePixelSmoothing;
            Settings.Default.Volume = (byte)Volume;
            Settings.Default.Mute = MuteSound;
            Settings.Default.AySoundFor48k = EnableAYFor48K;
            Settings.Default.SpeakerSetup = (byte)StereoSoundOption;
            Settings.Default.HighCompatabilityMode = HighCompatibilityMode;
            Settings.Default.PauseOnFocusChange = PauseOnFocusLost;
            Settings.Default.ShowOnScreenLEDs = ShowOnscreenIndicators;
            Settings.Default.RestoreLastStateOnStart = RestoreLastStateOnStart;
            Settings.Default.ConfirmOnExit = ConfirmOnExit;
            Settings.Default.TimingModel = UseLateTimings;
            Settings.Default.Issue2Keyboard = UseIssue2Keyboard;
            Settings.Default.EmulationSpeed = EmulationSpeed;
            Settings.Default.FileAssociationSZX = AccociateSZXFiles;
            Settings.Default.FileAssociationSNA = AccociateSNAFiles;
            Settings.Default.FileAssociationZ80 = AccociateZ80Files;
            Settings.Default.FileAssociationTZX = AccociateTZXFiles;
            Settings.Default.FileAssociationPZX = AccociatePZXFiles;
            Settings.Default.FileAssociationTAP = AccociateTAPFiles;
            Settings.Default.FileAssociationDSK = AccociateDSKFiles;
            Settings.Default.FileAssociationTRD = AccociateTRDFiles;
            Settings.Default.FileAssociationSCL = AccociateSCLFiles;
            Settings.Default.TapeAutoStart = TapeAutoStart;
            Settings.Default.TapeAutoLoad = TapeAutoLoad;
            Settings.Default.TapeEdgeLoad = TapeEdgeLoad;
            Settings.Default.TapeAccelerateLoad = TapeAccelerateLoad;
            Settings.Default.TapeInstaLoad = TapeInstaLoad;
            Settings.Default.DisableTapeTraps = DisableTapeTraps;
            Settings.Default.KempstonMouse = EnableKempstonMouse;
            Settings.Default.MouseSensitivity = (byte)MouseSensitivity;
            Settings.Default.Key2Joy = EnableKey2Joy;
            Settings.Default.Key2JoystickType = (byte)Key2JoystickType;
            Settings.Default.joystick1 = Joystick1Name;
            Settings.Default.joystick2 = Joystick2Name;
            Settings.Default.joystick1ToEmulate = Joystick1ToEmulate;
            Settings.Default.joystick2ToEmulate = Joystick2ToEmulate;
            Settings.Default.KempstonUsesPort1F = KempstonUsesPort1F;

            Settings.Default.Save();
        }
    }
}