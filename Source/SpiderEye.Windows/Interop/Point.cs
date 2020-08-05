using System.Runtime.InteropServices;

namespace SpiderEye.Windows.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Point
    {
        public int X;
        public int Y;
    }
}
