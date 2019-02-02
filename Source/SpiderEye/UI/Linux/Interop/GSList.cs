using System;

namespace SpiderEye.UI.Linux.Interop
{
    internal unsafe struct GSList
    {
        public readonly IntPtr Data;
        public readonly GSList* Next;
    }
}
