using System;
using System.Runtime.InteropServices;
using SpiderEye.UI.Linux.Interop;

namespace SpiderEye.UI.Linux.Native
{
    internal static class Gtk
    {
        private const string GtkNativeDll = "libgtk-3.so";

        public static class Window
        {
            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Create(GtkWindowType type);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_widget_destroy", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Destroy(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_scrolled_window_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateScrolled(IntPtr hadjustment, IntPtr vadjustment);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_get_title", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetTitle(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_title", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetTitle(IntPtr window, IntPtr title);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_widget_show_all", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ShowAll(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_close", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Close(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_present", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Present(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_keep_above", CallingConvention = CallingConvention.Cdecl)]
            public static extern void KeepAbove(IntPtr window, bool value);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_default_size", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetDefaultSize(IntPtr window, int width, int height);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_resizable", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetResizable(IntPtr window, bool resizable);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_position", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPosition(IntPtr window, GtkWindowPosition position);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_fullscreen", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Fullscreen(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_unfullscreen", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Unfullscreen(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_container_add", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ContainerAdd(IntPtr container, IntPtr widget);
        }

        [DllImport(GtkNativeDll, EntryPoint = "gtk_init_check", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Init(ref int argc, ref IntPtr argv);

        [DllImport(GtkNativeDll, EntryPoint = "gtk_main_iteration", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MainIteration();

        [DllImport(GtkNativeDll, EntryPoint = "gtk_main_iteration_do", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MainIteration(bool blocking);
    }
}
