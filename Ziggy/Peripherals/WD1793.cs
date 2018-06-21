using System;
using System.Runtime.InteropServices;

namespace Peripherals
{
    public class WD1793
    {
        [DllImport(@"wd1793.dll")]
        public static extern IntPtr wd1793_Initialise();

        [DllImport(@"wd1793.dll")]
        private static extern void wd1793_ShutDown(IntPtr fdc);

        [DllImport(@"wd1793.dll")]
        private static extern bool wd1793_InsertDisk(IntPtr fdc, byte unit, string filename);

        [DllImport(@"wd1793.dll")]
        private static extern void wd1793_EjectDisks(IntPtr fdc);

        [DllImport(@"wd1793.DLL")]
        private static extern void wd1793_EjectDisk(IntPtr fdc, byte _unit);

        [DllImport(@"wd1793.DLL")]
        private static extern byte wd1793_ReadStatusReg(IntPtr fdc);

        [DllImport(@"wd1793.DLL")]
        private static extern byte wd1793_ReadTrackReg(IntPtr fdc);

        [DllImport(@"wd1793.DLL")]
        private static extern byte wd1793_ReadSectorReg(IntPtr fdc);

        [DllImport(@"wd1793.DLL")]
        private static extern byte wd1793_ReadDataReg(IntPtr fdc);

        [DllImport(@"wd1793.DLL")]
        private static extern byte wd1793_ReadSystemReg(IntPtr fdc);

        [DllImport(@"wd1793.DLL")]
        private static extern void wd1793_WriteTrackReg(IntPtr fdc, byte _data);

        [DllImport(@"wd1793.DLL")]
        private static extern void wd1793_WriteSectorReg(IntPtr fdc, byte _data);

        [DllImport(@"wd1793.DLL")]
        private static extern void wd1793_WriteDataReg(IntPtr fdc, byte _data);

        [DllImport(@"wd1793.DLL")]
        private static extern void wd1793_WriteSystemReg(IntPtr fdc, byte _data);

        [DllImport(@"wd1793.DLL")]
        private static extern void wd1793_WriteCommandReg(IntPtr fdc, byte _data, ushort _pc);

        [DllImport(@"wd1793.DLL")]
        private static extern bool wd1793_DiskInserted(IntPtr fdc, byte _unit);

        //[DllImport(@"wd1793.DLL")]
        //private static extern void wd1793_SCL2TRD(IntPtr fdc, byte _unit);

        protected IntPtr fdc = IntPtr.Zero;

        public void DiskInsert(string filename, byte _unit) {
            wd1793_InsertDisk(fdc, _unit, filename);
        }

        public void DiskEject(byte _unit) {
            if (fdc != IntPtr.Zero)
                wd1793_EjectDisk(fdc, _unit);
        }

        public byte ReadStatusReg() {
            return wd1793_ReadStatusReg(fdc);
        }

        public byte ReadSectorReg() {
            return wd1793_ReadSectorReg(fdc);
        }

        public byte ReadDataReg() {
            return wd1793_ReadDataReg(fdc);
        }

        public byte ReadTrackReg() {
            return wd1793_ReadTrackReg(fdc);
        }

        public byte ReadSystemReg() {
            return wd1793_ReadSystemReg(fdc);
        }

        public void WriteCommandReg(byte _data, ushort _pc) {
            wd1793_WriteCommandReg(fdc, _data, _pc);
        }

        public void WriteSectorReg(byte _data) {
            wd1793_WriteSectorReg(fdc, _data);
        }

        public void WriteTrackReg(byte _data) {
            wd1793_WriteTrackReg(fdc, _data);
        }

        public void WriteDataReg(byte _data) {
            wd1793_WriteDataReg(fdc, _data);
        }

        public void WriteSystemReg(byte _data) {
            wd1793_WriteSystemReg(fdc, _data);
        }

        public void DiskInitialise() {
            if (fdc != IntPtr.Zero)
                wd1793_ShutDown(fdc);

            fdc = wd1793_Initialise();
        }

        public void DiskShutdown() {
            if (fdc != IntPtr.Zero)
                wd1793_ShutDown(fdc);

            //OnDiskEvent(new DiskEventArgs(0));
        }
    }
}