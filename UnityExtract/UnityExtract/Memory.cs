using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Swoopie.MemoryManagement;

namespace Swoopie
{

    static class Memory
    {
        #region Declaration Stuff
        static readonly int PROCESS_WM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        public static Process p;
        public static Api api = null;

        public static IntPtr ImageBase()
        {
            if (!isRunning())
            {
                return (IntPtr)0x0;
            }
            IntPtr baseAddress = p.MainModule.BaseAddress;
            return baseAddress;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, ref UInt32 lpNumberOfBytesWritten);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]

        public static extern short GetAsyncKeyState(int vKey);
        #endregion
        #region Read/Write Memory
        //READ
        public static byte[] ReadBytes(long address, Int32 bufferSize)
        {
            if (!isRunning())
            {
                return null;
            }
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, p.Id);

            int bytesRead = 0;
            byte[] buffer = new byte[bufferSize];

            ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);

            return buffer;
        }
        public static T Read<T>(long address, int len)
        {
            try
            {
                int size = Marshal.SizeOf(typeof(T));
                byte[] buffer = new byte[len];
                int read = 0;
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, p.Id);
                ReadProcessMemory((int)processHandle, address, buffer, len, ref read);
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                T data = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();
                return data;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        public static T ReadOld<T>(long address, Int32 bufferSize)
        {
            if (!isRunning())
            {
                return (T)Convert.ChangeType("0", typeof(T)); ;
            }
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, p.Id);

            int bytesRead = 0;
            byte[] buffer = new byte[bufferSize];

            ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);

            switch (Core.GetGenericType(new Dictionary<int, T>()))
            {
                case "single":
                    return (T)Convert.ChangeType(BitConverter.ToSingle(buffer, 0), typeof(T));

                case "string":
                    return (T)Convert.ChangeType(System.Text.Encoding.UTF8.GetString(buffer), typeof(T));

                case "int32":
                    return (T)Convert.ChangeType(BitConverter.ToInt32(buffer, 0), typeof(T));
                case "int64":
                    return (T)Convert.ChangeType(BitConverter.ToInt64(buffer, 0), typeof(T));
                case "byte":
                    return (T)Convert.ChangeType(buffer[0], typeof(T));
                case "byte[]":
                    return (T)Convert.ChangeType(buffer, typeof(T));
                default:
                    MessageBox.Show("Default" + Environment.NewLine + Core.GetGenericType(new Dictionary<int, T>()));
                    return (T)Convert.ChangeType("0", typeof(T));
            }
        }
        public static T ReadOld<T>(long address, Int32 bufferSize, bool addBase = false)
        {
            if (!isRunning())
            {
                return (T)Convert.ChangeType("0", typeof(T)); ;
            }
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, p.Id);

            int bytesRead = 0;
            byte[] buffer = new byte[bufferSize];


            if (addBase)
            {
                ReadProcessMemory((int)processHandle, Memory.ImageBase().ToInt64() + address, buffer, buffer.Length, ref bytesRead);
            }
            else
            {
                ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);
            }
            switch (Core.GetGenericType(new Dictionary<int, T>()))
            {
                case "single":
                    return (T)Convert.ChangeType(BitConverter.ToSingle(buffer, 0), typeof(T));

                case "string":
                    return (T)Convert.ChangeType(System.Text.Encoding.UTF8.GetString(buffer), typeof(T));

                case "int32":
                    return (T)Convert.ChangeType(BitConverter.ToInt32(buffer, 0), typeof(T));
                case "int64":
                    return (T)Convert.ChangeType(BitConverter.ToInt64(buffer, 0), typeof(T));
                case "byte":
                    return (T)Convert.ChangeType(buffer[0], typeof(T));
                case "byte[]":
                    return (T)Convert.ChangeType(buffer, typeof(T));
                default:
                    MessageBox.Show("Default" + Environment.NewLine + Core.GetGenericType(new Dictionary<int, T>()));
                    return (T)Convert.ChangeType("0", typeof(T));
            }
        }
        public static T Read<T>(long address, bool addBase = false, int customSize = -1)
        {
            try
            {
                int size = customSize == -1 ? Marshal.SizeOf(typeof(T)) : customSize;
                byte[] buffer = new byte[size];
                int read = 0;
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, p.Id);
                if (addBase)
                {
                    ReadProcessMemory((int)processHandle, Memory.ImageBase().ToInt64() + address, buffer, size, ref read);
                }
                else
                {
                    ReadProcessMemory((int)processHandle, address, buffer, size, ref read);
                }

                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                T data = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

                handle.Free();
                return data;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        //WRITE
        public static void WriteNops(long address, int amount)
        {
            byte[] nops = new byte[amount];
            for (int i = 0; i < amount; i++)
            {
                nops[i] = 144;
            }
            WriteBytes(address, nops);
        }
        public static void WriteBytes(long address, dynamic val)
        {
            if (!isRunning())
            {
                return;
            }

            bool success;
            UInt32 nBytesRead = 0;
            byte[] value = null;

            try
            {
                value = BitConverter.GetBytes(val);
            }
            catch
            {
                value = val;
            }
            success = WriteProcessMemory(p.Handle, (IntPtr)address, value, (IntPtr)value.Length, ref nBytesRead);
        }
        public static void WriteInt(long address, int value)
        {
            if (!isRunning())
            {
                return;
            }
            bool success;
            byte[] buffer = BitConverter.GetBytes(value);
            UInt32 nBytesRead = 0;
            success = WriteProcessMemory(p.Handle, (IntPtr)address, buffer, (IntPtr)4, ref nBytesRead);
        }
        public static void WriteFloat(long address, float value)
        {
            if (!isRunning())
            {
                return;
            }
            bool success;
            byte[] buffer = BitConverter.GetBytes(value);
            UInt32 nBytesRead = 0;
            success = WriteProcessMemory(p.Handle, (IntPtr)address, buffer, (IntPtr)4, ref nBytesRead);
        }
        public static void WriteShort(long address, short value)
        {
            if (!isRunning())
            {
                return;
            }
            bool success;
            byte[] buffer = BitConverter.GetBytes(value);
            UInt32 nBytesRead = 0;
            success = WriteProcessMemory(p.Handle, (IntPtr)address, buffer, (IntPtr)2, ref nBytesRead);
        }
        public static void WriteByte(long address, byte value)
        {
            if (!isRunning())
            {
                return;
            }
            bool success;
            byte[] buffer = BitConverter.GetBytes(value);
            UInt32 nBytesRead = 0;
            success = WriteProcessMemory(p.Handle, (IntPtr)address, buffer, (IntPtr)1, ref nBytesRead);
        }
        #endregion
        public static bool isRunning()
        {
            try
            {
                p = Process.GetProcessesByName(EFTCore.procName)[0];
                if (api == null && p != null)
                {
                    api = ApiFactory.Create(new GameProcess(p, false));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
