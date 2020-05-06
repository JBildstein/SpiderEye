﻿using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SpiderEye.Windows.Interop
{
    internal class WinNative
    {
        private const string NtDll = "ntdll.dll";

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
