using System;
using System.Runtime.InteropServices;
using SpiderEye.UI.Linux.Interop;

namespace SpiderEye.UI.Linux.Native
{
    internal static class GLib
    {
        private const string GLibNativeDll = "libglib-2.0.so";
        private const string GObjectNativeDll = "libgobject-2.0.so";
        private const string GIONativeDll = "libgio-2.0.so";

        [DllImport(GLibNativeDll, EntryPoint = "g_malloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Malloc(UIntPtr size);

        [DllImport(GLibNativeDll, EntryPoint = "g_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Free(IntPtr mem);

        [DllImport(GObjectNativeDll, EntryPoint = "g_signal_connect_data", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConnectSignalData(IntPtr instance, IntPtr signal_name, Delegate handler, IntPtr data, IntPtr destroy_data, int connect_flags);

        [DllImport(GIONativeDll, EntryPoint = "g_memory_input_stream_new_from_data", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateStreamFromData(IntPtr data, long len, IntPtr destroy);

        [DllImport(GObjectNativeDll, EntryPoint = "g_object_unref", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UnrefObject(IntPtr obj);

        [DllImport(GLibNativeDll, EntryPoint = "g_error_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeError(IntPtr error);

        [DllImport(GLibNativeDll, EntryPoint = "g_slist_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeSList(IntPtr slist);

        [DllImport(GObjectNativeDll, EntryPoint = "g_file_error_quark", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetFileErrorQuark();

        [DllImport(GLibNativeDll, EntryPoint = "g_bytes_get_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr GetBytesSize(IntPtr bytes);

        [DllImport(GLibNativeDll, EntryPoint = "g_bytes_unref", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr UnrefBytes(IntPtr bytes);

        [DllImport(GLibNativeDll, EntryPoint = "g_bytes_get_data", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetBytesDataPointer(IntPtr bytes, out UIntPtr size);

        [DllImport(GObjectNativeDll, EntryPoint = "g_object_set", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetProperty(IntPtr obj, IntPtr propertyName, IntPtr value, IntPtr terminator);

        public static void ConnectSignal(IntPtr instance, string signalName, Delegate handler, IntPtr data)
        {
            using (GLibString gname = signalName)
            {
                ConnectSignalData(instance, gname, handler, data, IntPtr.Zero, 0);
            }
        }

        public static void SetProperty(IntPtr obj, string propertyName, IntPtr value)
        {
            using (GLibString gname = propertyName)
            {
                SetProperty(obj, gname, value, IntPtr.Zero);
            }
        }
    }
}
