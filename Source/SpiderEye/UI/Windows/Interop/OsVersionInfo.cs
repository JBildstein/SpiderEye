using System.Runtime.InteropServices;

namespace SpiderEye.UI.Windows.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct OsVersionInfo
    {
        public int OSVersionInfoSize;
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

        public OsVersionInfo(int structSize)
            : this()
        {
            OSVersionInfoSize = structSize;
        }
    }
}
