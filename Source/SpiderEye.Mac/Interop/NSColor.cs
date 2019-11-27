using System;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac.Interop
{
    internal static class NSColor
    {
        public static IntPtr FromHex(string hex)
        {
            ColorTools.ParseHex(hex, out byte r, out byte g, out byte b);

            return AppKit.Call("NSColor", "colorWithRed:green:blue:alpha:", r / 255d, g / 255d, b / 255d, 1d);
        }
    }
}
