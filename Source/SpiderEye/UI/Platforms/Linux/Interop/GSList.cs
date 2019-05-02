using System;
using System.Runtime.InteropServices;

namespace SpiderEye.UI.Linux.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GSList
    {
        public readonly IntPtr Data;
        public readonly GSList* Next;
    }
}
