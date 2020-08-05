using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SpiderEye.Windows.Interop
{
    internal class Native
    {
        private const string NtDll = "ntdll.dll";
        private const string User32 = "user32.dll";

        public static OsVersionInfo GetOsVersion()
        {
            var info = OsVersionInfo.Default;
            RtlGetVersion(ref info);

            CheckLastError();

            return info;
        }

        public static bool SetWindowState(Form form, SW state)
        {
            bool wasVisible = ShowWindow(form.Handle, state);
            CheckLastError();

            return wasVisible;
        }

        public static WS GetWindowStyle(Form form)
        {
            int style = GetWindowLong(form.Handle, GWL.STYLE);
            CheckLastError();

            return unchecked((WS)style);
        }

        public static WindowPlacement GetWindowPlacement(Form form)
        {
            var placement = WindowPlacement.Default;
            GetWindowPlacement(form.Handle, ref placement);
            CheckLastError();

            return placement;
        }

        public static void SetWindowPlacement(Form form, WindowPlacement placement)
        {
            SetWindowPlacement(form.Handle, ref placement);
            CheckLastError();
        }

        public static MonitorInfo GetMonitorInfo(Form form, Monitor monitor)
        {
            IntPtr monitorPtr = MonitorFromWindow(form.Handle, monitor);
            CheckLastError();

            var info = MonitorInfo.Default;
            GetMonitorInfo(monitorPtr, ref info);
            CheckLastError();

            return info;
        }

        public static void SetWindowStyle(Form form, WS style)
        {
            SetWindowLong(form.Handle, GWL.STYLE, (int)style);
            CheckLastError();
        }

        public static void SetWindowPos(Form form, int x, int y, int cx, int cy, SWP flags)
        {
            SetWindowPos(form.Handle, IntPtr.Zero, x, y, cx, cy, flags);
            CheckLastError();
        }

        private static void CheckLastError()
        {
            int error = Marshal.GetLastWin32Error();
            if (error < 0) { throw new Win32Exception(error); }
        }

        [DllImport(NtDll, SetLastError = true)]
        private static extern bool RtlGetVersion(ref OsVersionInfo versionInfo);

        [DllImport(User32, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, SW msg);

        [DllImport(User32, SetLastError = true)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        [DllImport(User32, SetLastError = true)]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [DllImport(User32, SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport(User32, SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport(User32, SetLastError = true)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

        [DllImport(User32, SetLastError = true)]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, Monitor dwFlags);

        [DllImport(User32, SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);
    }
}
