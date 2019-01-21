using System;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac.Interop
{
    internal static class NSColor
    {
        public static IntPtr FromHex(string hex)
        {
            hex = hex?.TrimStart('#');
            if (string.IsNullOrWhiteSpace(hex) || hex.Length != 6)
            {
                hex = "FFFFFF";
            }

            double r = Convert.ToByte(hex.Substring(0, 2), 16) / 255d;
            double g = Convert.ToByte(hex.Substring(2, 2), 16) / 255d;
            double b = Convert.ToByte(hex.Substring(4, 2), 16) / 255d;

            return AppKit.Call("NSColor", "colorWithRed:green:blue:alpha:", r, g, b, 1d);
        }
    }
}
