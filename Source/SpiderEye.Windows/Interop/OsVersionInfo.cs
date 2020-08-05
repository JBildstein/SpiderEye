using System.Runtime.InteropServices;

namespace SpiderEye.Windows.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct OsVersionInfo
    {
        public int Size;
        public int MajorVersion;
        public int MinorVersion;
        public int BuildNumber;
        public int PlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string CSDVersion;
        public ushort ServicePackMajor;
        public ushort ServicePackMinor;
        public short SuiteMask;
        public ProductType ProductType;
        private byte reserved;

        public static OsVersionInfo Default
        {
            get
            {
                OsVersionInfo result = default;
                result.Size = Marshal.SizeOf(result);
                return result;
            }
        }
    }
}
