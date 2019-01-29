using System;
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

        [DllImport(User32Dll, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowExW(
            uint dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport(User32Dll, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        public static OsVersionInfo GetOsVersion()
        {
            var info = new OsVersionInfo(Marshal.SizeOf(typeof(OsVersionInfo)));
            RtlGetVersion(ref info);

            CheckLastError();

            return info;
        }

        public static IntPtr CreateBrowserWindow(IntPtr parentWindow)
        {
            IntPtr ptr = CreateWindowExW(
                0,
                "Static",
                string.Empty,
                1375731712, // Child | Visible | ClipChildren
                0,
                0,
                0,
                0,
                parentWindow,
                IntPtr.Zero,
                Marshal.GetHINSTANCE(typeof(Native).Module),
                IntPtr.Zero);

            if (ptr == IntPtr.Zero) { throw new Win32Exception(); }

            return ptr;
        }

        public static void CheckLastError()
        {
            int error = Marshal.GetLastWin32Error();
            if (error < 0) { throw new Win32Exception(error); }
        }
    }
}
