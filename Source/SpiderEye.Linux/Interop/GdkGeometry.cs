using System.Runtime.InteropServices;

namespace SpiderEye.Linux.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GdkGeometry
    {
        public readonly int MinWidth;
        public readonly int MinHeight;
        public readonly int MaxWidth;
        public readonly int MaxHeight;
        public readonly int BaseWidth;
        public readonly int BaseHeight;
        public readonly int WidthInc;
        public readonly int HeightInc;
        public readonly double MinAspect;
        public readonly double MaxAspect;
        public readonly GdkGravity WinGravity;

        public GdkGeometry(Size min, Size max)
            : this()
        {
            MinWidth = (int)min.Width;
            MinHeight = (int)min.Height;
            MaxWidth = (int)max.Width;
            MaxHeight = (int)max.Height;
        }
    }
}
