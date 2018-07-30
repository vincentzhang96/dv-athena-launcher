using System;

namespace Divinitor.DN.Athena.Lib.Launcher
{
    public struct ProcessInformation
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public uint dwProcessId;
        public uint dwThreadId;
    }
}