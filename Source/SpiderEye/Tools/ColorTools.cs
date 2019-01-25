using System;

namespace SpiderEye.Tools
{
    internal static class ColorTools
    {
        public static void ParseHex(string color, out byte r, out byte g, out byte b)
        {
            color = color?.TrimStart('#');
            if (string.IsNullOrWhiteSpace(color) || color.Length != 6)
            {
                color = "FFFFFF";
            }

            r = Convert.ToByte(color.Substring(0, 2), 16);
            g = Convert.ToByte(color.Substring(2, 2), 16);
            b = Convert.ToByte(color.Substring(4, 2), 16);
        }
    }
}
