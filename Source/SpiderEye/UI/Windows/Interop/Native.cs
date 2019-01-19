using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SpiderEye.UI.Windows.Interop
{
    internal class Native
    {
        private const string NtDll = "ntdll.dll";
        private const string User32Dll = "user32.dll";

        [DllImport(User32Dll, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableMouseInPointer(bool fEnable);

        [DllImport(NtDll, SetLastError = true)]
        public static extern bool RtlGetVersion(ref OsVersionInfo versionInfo);

        public static OsVersionInfo GetOsVersion()
        {
            var info = new OsVersionInfo(Marshal.SizeOf(typeof(OsVersionInfo)));
            RtlGetVersion(ref info);

            CheckLastError();

            return info;
        }

        public static void CheckLastError()
        {
            int error = Marshal.GetLastWin32Error();
            if (error < 0) { throw new Win32Exception(error); }
        }
    }
}
