using System.Runtime.InteropServices;

namespace SpiderEye.Windows.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonitorInfo
    {
        public int Size;
        public Rect Monitor;
        public Rect WorkArea;
        public uint Flags;

        public static MonitorInfo Default
        {
            get
            {
                MonitorInfo result = default;
                result.Size = Marshal.SizeOf(result);
                return result;
            }
        }
    }
}
