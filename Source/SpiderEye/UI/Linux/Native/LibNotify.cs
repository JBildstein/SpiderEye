using System;
using System.Runtime.InteropServices;
using SpiderEye.UI.Linux.Interop;

namespace SpiderEye.UI.Linux.Native
{
    internal static class LibNotify
    {
        private const string LibNotifyDll = "libnotify";

        [DllImport(LibNotifyDll, EntryPoint = "notify_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Init(string app_name);

        [DllImport(LibNotifyDll, EntryPoint = "notify_uninit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Close();

        [DllImport(LibNotifyDll, EntryPoint = "notify_is_initted", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsInitiated();

        [DllImport(LibNotifyDll, EntryPoint = "notify_get_app_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern string GetAppName();

        [DllImport(LibNotifyDll, EntryPoint = "notify_set_app_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAppName(string app_name);

        public static class Notification
        {
            [DllImport(LibNotifyDll, EntryPoint = "notify_notification_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Create(string sumary, string body, string icon);

            [DllImport(LibNotifyDll, EntryPoint = "notify_notification_close", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool Close(IntPtr notification, out IntPtr error);

            [DllImport(LibNotifyDll, EntryPoint = "notify_notification_show", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool Show(IntPtr notification, out IntPtr error);

            [DllImport(LibNotifyDll, EntryPoint = "notify_notification_set_timeout", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetTimeout(IntPtr notification, int timeout);

            [DllImport(LibNotifyDll, EntryPoint = "notify_notification_set_image_from_pixbuf", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetImage(IntPtr notification, IntPtr pixbuf);
        }
    }
}
