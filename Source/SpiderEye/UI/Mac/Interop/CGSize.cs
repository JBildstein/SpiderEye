using System.Runtime.InteropServices;

namespace SpiderEye.UI.Mac.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CGSize
    {
        public double Width;
        public double Height;

        public CGSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
