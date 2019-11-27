using System;
using System.Runtime.InteropServices;

namespace SpiderEye.Linux.Native
{
    internal static class Gdk
    {
        private const string GdkNativeDll = "libgdk-3.so";

        public static class Pixbuf
        {
            [DllImport(GdkNativeDll, EntryPoint = "gdk_pixbuf_new_from_stream", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr NewFromStream(IntPtr stream, IntPtr cancellable, IntPtr error);
        }
    }
}
