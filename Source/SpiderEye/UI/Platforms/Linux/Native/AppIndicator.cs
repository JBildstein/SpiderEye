using System;
using System.Runtime.InteropServices;
using SpiderEye.UI.Linux.Interop;

namespace SpiderEye.UI.Linux.Native
{
    internal static partial class AppIndicator
    {
        private const string AppIndicatorNativeDll = "libappindicator3.so";

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_new", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create(IntPtr id, IntPtr icon_name, AppIndicatorCategory category);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_icon", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIcon(IntPtr self, IntPtr icon_name);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_menu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMenu(IntPtr self, IntPtr menu);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_title", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetTitle(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_title", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetTitle(IntPtr self, IntPtr title);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_status", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetStatus(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_status", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetStatus(IntPtr self, AppIndicatorStatus status);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Dispose(IntPtr gobject);
    }
}
