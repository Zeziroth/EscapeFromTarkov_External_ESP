using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Swoopie.MemoryManagement
{
    public class ProcessMethods : IProcessMethods
    {
        public const int ProcessVmOperation = 8;

        public const int ProcessVmRead = 16;

        public const int ProcessVmWrite = 32;

        public ProcessMethods()
        {

        }

        public AbstractGameProcess GetGameProcess()
        {
            if (!Environment.Is64BitOperatingSystem && Process.GetProcessesByName(Settings.GAME_NAME).Length != 0)
            {

            }

            Process[] processesByName = Process.GetProcessesByName(Settings.GAME_NAME);
            Process[] processArray = Process.GetProcessesByName(Settings.GAME_NAME);
            if (processesByName.Length == 0 && processArray.Length == 0)
            {

            }
            if ((int)processesByName.Length > 1 || (int)processArray.Length > 1)
            {

            }
            if ((int)processesByName.Length == 1)
            {
                return new GameProcess(processesByName[0], true);
            }
            if ((int)processArray.Length != 1)
            {

            }
            return new GameProcess(processArray[0], false);
        }

        public List<AbstractGameProcess> GetGameProcesses()
        {
            Process[] processesByName = Process.GetProcessesByName(Settings.GAME_NAME);
            Process[] processArray = Process.GetProcessesByName(Settings.GAME_NAME);
            List<AbstractGameProcess> list = (
                from process in (IEnumerable<Process>)processesByName
                select new GameProcess(process, true)).Cast<AbstractGameProcess>().ToList<AbstractGameProcess>();
            list.AddRange(
                from processdx11 in (IEnumerable<Process>)processArray
                select new GameProcess(processdx11, false));
            return list;
        }

        public IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId)
        {
            return ProcessMethods.NativeMethods.OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId);
        }

        public bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out int lpNumberOfBytesRead)
        {
            return ProcessMethods.NativeMethods.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, out lpNumberOfBytesRead);
        }

        public IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return ProcessMethods.NativeMethods.SendMessage(hWnd, msg, wParam, lParam);
        }

        public bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, ProcessMethods.FreeType dwFreeType)
        {
            return ProcessMethods.NativeMethods.VirtualFreeEx(hProcess, lpAddress, dwSize, dwFreeType);
        }

        public bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten)
        {
            return ProcessMethods.NativeMethods.WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, out lpNumberOfBytesWritten);
        }

        [Flags]
        public enum AllocationType
        {
            Commit = 4096,
            Reserve = 8192,
            Decommit = 16384,
            Release = 32768,
            Reset = 524288,
            TopDown = 1048576,
            WriteWatch = 2097152,
            Physical = 4194304,
            LargePages = 536870912
        }

        [Flags]
        public enum FreeType
        {
            MemDecommit = 16384,
            MemRelease = 32768
        }

        [Flags]
        public enum MemoryProtection
        {
            NoAccess = 1,
            ReadOnly = 2,
            ReadWrite = 4,
            WriteCopy = 8,
            Execute = 16,
            ExecuteRead = 32,
            ExecuteReadWrite = 64,
            ExecuteWriteCopy = 128,
            GuardModifierflag = 256,
            NoCacheModifierflag = 512,
            WriteCombineModifierflag = 1024
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
            internal static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

            [DllImport("Kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
            internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out int lpNumberOfBytesRead);

            [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
            internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = true, SetLastError = true)]
            internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, ProcessMethods.FreeType dwFreeType);

            [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
            internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);
        }
    }
}