using System;
using System.Diagnostics;

namespace Swoopie.MemoryManagement
{
    public abstract class AbstractGameProcess : Process
    {
        protected AbstractGameProcess()
        {
        }

        public abstract IntPtr GetBaseAddress();

        public abstract IntPtr GetHandle();

        public abstract bool GetHasExited();

        public abstract int GetId();

        public abstract int GetMainModuleMemorySize();

        public abstract IntPtr GetMainWindowHandle();

        public abstract bool Is32Bit();
    }
}