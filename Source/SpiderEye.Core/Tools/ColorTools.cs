using System;

namespace SpiderEye.Tools
{
    internal static class ColorTools
    {
        public static void ParseHex(string? color, out byte r, out byte g, out byte b)
        {
            color = color?.TrimStart('#');
            if (string.IsNullOrWhiteSpace(color) || color.Length != 6)
            {
                color = "FFFFFF";
            }

            r = Convert.ToByte(color[..2], 16);
            g = Convert.ToByte(color[2..4], 16);
            b = Convert.ToByte(color[4..], 16);
        }

        public static string ToHex(byte r, byte g, byte b)
        {
            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }
}
