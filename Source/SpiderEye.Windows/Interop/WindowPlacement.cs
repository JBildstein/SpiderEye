using System.Runtime.InteropServices;

namespace SpiderEye.Windows.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowPlacement
    {
        public int Length;
        public int Flags;
        public SW ShowCommand;
        public Point MinPosition;
        public Point MaxPosition;
        public Rect NormalPosition;

        public static WindowPlacement Default
        {
            get
            {
                WindowPlacement result = default;
                result.Length = Marshal.SizeOf(result);
                return result;
            }
        }
    }
}
