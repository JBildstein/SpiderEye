using System.Runtime.InteropServices;
using SpiderEye.Tools;

namespace SpiderEye.Linux.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GdkColor
    {
        public readonly double Red;
        public readonly double Green;
        public readonly double Blue;
        public readonly double Alpha;

        public GdkColor(string hex)
        {
            ColorTools.ParseHex(hex, out byte r, out byte g, out byte b);
            Red = r / 255d;
            Green = g / 255d;
            Blue = b / 255d;
            Alpha = 1;
        }

        public GdkColor(byte red, byte green, byte blue)
        {
            Red = red / 255d;
            Green = green / 255d;
            Blue = blue / 255d;
            Alpha = 1;
        }
    }
}
