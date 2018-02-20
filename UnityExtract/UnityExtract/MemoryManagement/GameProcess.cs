using System;
using System.Diagnostics;

namespace Swoopie.MemoryManagement
{
    public class GameProcess : AbstractGameProcess
    {
        private readonly Process process;

        private readonly bool is32Bit;

        public string Identifier
        {
            get
            {
                return string.Format("{0} | PID: {1}", this.process.ProcessName, this.process.Id);
            }
        }

        public GameProcess(Process process, bool is32Bit = true)
        {
            this.process = process;
            this.is32Bit = is32Bit;
        }

        public override IntPtr GetBaseAddress()
        {
            return this.process.MainModule.BaseAddress;
        }

        public override IntPtr GetHandle()
        {
            return this.process.Handle;
        }

        public override bool GetHasExited()
        {
            return this.process.HasExited;
        }

        public override int GetId()
        {
            return this.process.Id;
        }

        public override int GetMainModuleMemorySize()
        {
            return this.process.MainModule.ModuleMemorySize;
        }

        public override IntPtr GetMainWindowHandle()
        {
            return this.process.MainWindowHandle;
        }

        public override bool Is32Bit()
        {
            return this.is32Bit;
        }

        public new void Refresh()
        {
            this.process.Refresh();
        }
    }
}