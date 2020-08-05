using System;

namespace SpiderEye.Mac.Interop
{
    [Flags]
    internal enum NSWindowStyleMask
    {
        Borderless = 0,
        Titled = 1 << 0,
        Closable = 1 << 1,
        Miniaturizable = 1 << 2,
        Resizable = 1 << 3,
        UnifiedTitleAndToolbar = 1 << 12,
        FullScreen = 1 << 14,
        FullSizeContentView = 1 << 15,
    }
}
