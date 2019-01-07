namespace SpiderEye.Linux
{
    internal struct GdkColor
    {
        public readonly double Red;
        public readonly double Green;
        public readonly double Blue;
        public readonly double Alpha;

        public GdkColor(byte red, byte green, byte blue)
        {
            Red = red / 255d;
            Green = green / 255d;
            Blue = blue / 255d;
            Alpha = 1;
        }
    }
}
