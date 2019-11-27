using System;

namespace SpiderEye.Mac.Interop
{
    [Flags]
    internal enum NSWindowStyleMask
    {
        Titled = 1 << 0,
        Closable = 1 << 1,
        Miniaturizable = 1 << 2,
        Resizable = 1 << 3,
    }
}
