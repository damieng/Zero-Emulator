using System.Runtime.InteropServices;

namespace Peripherals
{
    public class UDP765
    {
        [DllImport(@"fdc765.DLL")]
        private static extern System.IntPtr u765_Initialise();

        [DllImport(@"fdc765.DLL")]
        private static extern void u765_Shutdown(System.IntPtr fdc);

        [DllImport(@"fdc765.DLL")]
        private static extern void u765_ResetDevice(System.IntPtr fdc);

        [DllImport(@"fdc765.DLL")]
        private static extern void u765_InsertDisk(System.IntPtr fdc, string filename, byte unit);

        [DllImport(@"fdc765.DLL")]
        private static extern void u765_EjectDisk(System.IntPtr fdc, byte unit);

        [DllImport(@"fdc765.DLL")]
        private static extern void u765_SetMotorState(System.IntPtr fdc, byte state);

        [DllImport(@"fdc765.DLL")]
        private static extern byte u765_StatusPortRead(System.IntPtr fdc);

        [DllImport(@"fdc765.DLL")]
        private static extern byte u765_DataPortRead(System.IntPtr fdc);

        [DllImport(@"fdc765.DLL")]
        private static extern void u765_DataPortWrite(System.IntPtr fdc, byte data);

        [DllImport(@"fdc765.DLL")]
        private static extern int u765_DiskInserted(System.IntPtr fdc, byte unit);

        protected System.IntPtr fdc = System.IntPtr.Zero;

        public bool DiskWriteProtect {
            get;
            set;
        }

        public byte DiskReadByte() {
            if (fdc != System.IntPtr.Zero)
                return u765_DataPortRead(fdc);

            return 0;
        }

        public void DiskWriteByte(byte _data) {
            if ((fdc != System.IntPtr.Zero) && (!DiskWriteProtect))
                u765_DataPortWrite(fdc, _data);
        }

        public byte DiskStatusRead() {
            if (fdc != System.IntPtr.Zero)
                return u765_StatusPortRead(fdc);

            return 0;
        }

        public void DiskMotorState(byte _state) {
            if (fdc != System.IntPtr.Zero)
                u765_SetMotorState(fdc, _state);

            //OnDiskEvent(new DiskEventArgs((_state & 0x08) >> 3));
        }

        public void DiskInsert(string filename, byte _unit) {
            u765_InsertDisk(fdc, filename, _unit);
        }

        public void DiskEject(byte _unit) {
            if (fdc != System.IntPtr.Zero)
                u765_EjectDisk(fdc, _unit);
        }

        public void DiskReset() {
            if (fdc != System.IntPtr.Zero)
                u765_ResetDevice(fdc);

            //OnDiskEvent(new DiskEventArgs(0));
        }

        public void DiskInitialise() {
            if (fdc != System.IntPtr.Zero)
                u765_Shutdown(fdc);

            fdc = u765_Initialise();
        }

        public void DiskShutdown() {
            if (fdc != System.IntPtr.Zero)
                u765_Shutdown(fdc);

            //OnDiskEvent(new DiskEventArgs(0));
        }
    }
}