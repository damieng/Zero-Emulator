using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;

namespace ZeroWin
{
    public class MouseController
    {
        private Device mouse;
        Form1 ziggyWin;

        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public bool MouseLeftButtonDown { get; set; }
        public bool MouseRightButtonDown { get; set; }

        public void AcquireMouse(Form1 zw) {
            ziggyWin = zw;
            mouse = new Device(SystemGuid.Mouse);
            mouse.SetDataFormat(DeviceDataFormat.Mouse);
            mouse.SetCooperativeLevel(ziggyWin, CooperativeLevelFlags.Exclusive | CooperativeLevelFlags.Foreground);
            mouse.Acquire();
        }

        public void UpdateMouse() {
            if (mouse != null) {

                try {
                    MouseState state = mouse.CurrentMouseState;
                    MouseX = state.X;
                    MouseY = state.Y;
                    byte[] buttons = state.GetMouseButtons();
                    MouseLeftButtonDown = buttons[0] > 0;//state.IsPressed(0);
                    MouseRightButtonDown = buttons[1] > 0;//state.IsPressed(1);
                }
                catch (Exception) {
                    ziggyWin.EnableMouse(false);
                }
            }
        }

        public void ReleaseMouse() {
            if (mouse != null) {
                mouse.Unacquire();
                mouse.Dispose();
            }
            mouse = null;
        }
    }

    public class JoystickController
    {
        public Device joystick;
        public JoystickState state;
        public static List<DeviceInstance> joystickList = new List<DeviceInstance>();
        public string name;
        public bool isInitialized;

        public int[] buttonMap = new int[0];

        //Specifies which button will act as the 'Fire' button.
        //No key will be assigned to this button in the buttonmap list above (i.e. it will remain -1).
        public int fireButtonIndex;

        public static void EnumerateJosticks() {
            joystickList.Clear();

            DeviceList deviceList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

            foreach (DeviceInstance di in deviceList) {
                joystickList.Add(di);
            }
        }

        public static string[] GetDeviceNames() {
            return joystickList.Select(j => j.InstanceName).ToArray();
        }

        public bool InitJoystick(Form1 zw, int deviceNum) {
            try {
                joystick = new Device(joystickList[deviceNum].InstanceGuid);
                joystick.SetCooperativeLevel(zw, CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Background);
                joystick.SetDataFormat(DeviceDataFormat.Joystick);
                name = joystickList[deviceNum].ProductName;
            }
            catch (InputException) {
                MessageBox.Show("Couldn't connect to joystick!", "Joystick Problem", MessageBoxButtons.OK);
                return false;
            }
            foreach (DeviceObjectInstance deviceObject in joystick.Objects) {
                if ((deviceObject.ObjectId & (int)DeviceObjectTypeFlags.Axis) != 0)
                    joystick.Properties.SetRange(ParameterHow.ById, deviceObject.ObjectId, new InputRange(-1000, 1000));
            }
            // acquire the device
            try {
                joystick.Acquire();
            }
            catch (InputException de) {
                MessageBox.Show(de.Message, "Joystick Error", MessageBoxButtons.OK);
                return false;
            }

            buttonMap = new int[joystick.Caps.NumberButtons];
            for (int f = 0; f < buttonMap.Length; f++)
                buttonMap[f] = -1;
            fireButtonIndex = 0; //Button 0 on the controller is 'fire' by default
            isInitialized = true;
            return true;
        }

        public void Update() {
            if (!isInitialized || joystick == null)
                return;

            try {
                joystick.Poll();
                state = joystick.CurrentJoystickState;
            }
            catch (InputException) {
                MessageBox.Show("The connection to the joystick has been lost.", "Joystick Problem", MessageBoxButtons.OK);
                isInitialized = false;
            }
        }

        public void Release() {
            if (joystick != null) {
                joystick.Unacquire();
                joystick.Dispose();
            }
            joystick = null;
            isInitialized = false;
        }
    }
}