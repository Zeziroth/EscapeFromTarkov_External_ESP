using System;
using System.Collections.Generic;

namespace Swoopie.MemoryManagement
{
    public interface IProcessMethods
    {
        AbstractGameProcess GetGameProcess();

        List<AbstractGameProcess> GetGameProcesses();

        IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out int lpNumberOfBytesRead);

        IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, ProcessMethods.FreeType dwFreeType);

        bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);
    }
}