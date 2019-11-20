using System;

namespace SpiderEye.UI.Platforms.Mac.Interop
{
    [Flags]
    internal enum NSEventModifierFlags : ulong
    {
        None = 0,
        Shift = 1 << 17,
        Control = 1 << 18,
        Option = 1 << 19,
        Command = 1 << 20,
        Function = 1 << 23,
    }
}
