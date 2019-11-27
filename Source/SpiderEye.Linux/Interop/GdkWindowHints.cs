using System;

namespace SpiderEye.Linux.Interop
{
    [Flags]
    internal enum GdkWindowHints
    {
        Pos = 1 << 0,
        MinSize = 1 << 1,
        MaxSize = 1 << 2,
        BaseSize = 1 << 3,
        Aspect = 1 << 4,
        ResizeInc = 1 << 5,
        WinGravity = 1 << 6,
        UserPos = 1 << 7,
        UserSize = 1 << 8,
    }
}
