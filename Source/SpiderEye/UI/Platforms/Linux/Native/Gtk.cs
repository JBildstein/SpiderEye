using System;
using System.Runtime.InteropServices;
using SpiderEye.UI.Linux.Interop;

namespace SpiderEye.UI.Linux.Native
{
    internal static class Gtk
    {
        private const string GtkNativeDll = "libgtk-3.so";

        public static class Widget
        {
            [DllImport(GtkNativeDll, EntryPoint = "gtk_container_add", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ContainerAdd(IntPtr container, IntPtr widget);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_widget_show_all", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ShowAll(IntPtr widget);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_widget_show", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Show(IntPtr widget);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_widget_destroy", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Destroy(IntPtr widget);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_widget_set_sensitive", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetEnabled(IntPtr widget, bool enabled);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_widget_get_sensitive", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool GetEnabled(IntPtr widget);
        }

        public static class Window
        {
            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Create(GtkWindowType type);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_scrolled_window_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateScrolled(IntPtr hadjustment, IntPtr vadjustment);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_get_title", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetTitle(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_title", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetTitle(IntPtr window, IntPtr title);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_close", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Close(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_present", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Present(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_keep_above", CallingConvention = CallingConvention.Cdecl)]
            public static extern void KeepAbove(IntPtr window, bool value);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_default_size", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetDefaultSize(IntPtr window, int width, int height);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_get_size", CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetSize(IntPtr window, out int width, out int height);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_get_resizable", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool GetResizable(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_resizable", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetResizable(IntPtr window, bool resizable);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_type_hint", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetTypeHint(IntPtr window, GdkWindowTypeHint hint);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_position", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPosition(IntPtr window, GtkWindowPosition position);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_fullscreen", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Fullscreen(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_unfullscreen", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Unfullscreen(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_maximize", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Maximize(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_unmaximize", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Unmaximize(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_iconify", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Minimize(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_deiconify", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Unminimize(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_is_maximized", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool IsMaximized(IntPtr window);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_resize", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Resize(IntPtr window, int width, int height);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_icon", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetIcon(IntPtr window, IntPtr icon);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_window_set_icon_list", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetIconList(IntPtr window, IntPtr list);
        }

        public static class Menu
        {
            [DllImport(GtkNativeDll, EntryPoint = "gtk_menu_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Create();

            [DllImport(GtkNativeDll, EntryPoint = "gtk_menu_shell_append", CallingConvention = CallingConvention.Cdecl)]
            public static extern void AddItem(IntPtr menu_shell, IntPtr child);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_separator_menu_item_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateSeparatorItem();

            [DllImport(GtkNativeDll, EntryPoint = "gtk_menu_item_new_with_label", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateLabelItem(IntPtr label);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_menu_item_set_submenu", CallingConvention = CallingConvention.Cdecl)]
            public static extern void AddSubmenu(IntPtr menu_item, IntPtr submenu);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_menu_item_get_label", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetMenuItemLabel(IntPtr menu_item);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_menu_item_set_label", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetMenuItemLabel(IntPtr menu_item, IntPtr label);
        }

        public static class Dialog
        {
            [DllImport(GtkNativeDll, EntryPoint = "gtk_message_dialog_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateMessageDialog(IntPtr parent, GtkDialogFlags flags, GtkMessageType type, GtkButtonsType buttons, IntPtr message_format);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_native_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateNativeFileDialog(IntPtr title, IntPtr parent, GtkFileChooserAction action, IntPtr acceptLabel, IntPtr cancelLabel);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_dialog_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CreateFileDialog(
                IntPtr title,
                IntPtr parent,
                GtkFileChooserAction action,
                IntPtr firstButtonText,
                GtkResponseType firstButtonResponse,
                IntPtr secondButtonText,
                GtkResponseType secondButtonResponse,
                IntPtr terminator);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_dialog_run", CallingConvention = CallingConvention.Cdecl)]
            public static extern GtkResponseType Run(IntPtr dialog);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_native_dialog_run", CallingConvention = CallingConvention.Cdecl)]
            public static extern GtkResponseType RunNative(IntPtr dialog);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_set_do_overwrite_confirmation", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetOverwriteConfirmation(IntPtr chooser, bool enable);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_set_create_folders", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool SetCanCreateFolder(IntPtr dialog, bool enable);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_set_select_multiple", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool SetAllowMultiple(IntPtr dialog, bool enable);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_set_current_name", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetCurrentName(IntPtr chooser, IntPtr name);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_get_filename", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetFileName(IntPtr chooser);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_set_filename", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetFileName(IntPtr dialog, IntPtr fileName);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_set_current_folder", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool SetCurrentFolder(IntPtr dialog, IntPtr folder);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_add_filter", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool AddFileFilter(IntPtr dialog, IntPtr filter);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_file_chooser_get_filenames", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetSelectedFiles(IntPtr dialog);

            public static class FileFilter
            {
                [DllImport(GtkNativeDll, EntryPoint = "gtk_file_filter_new", CallingConvention = CallingConvention.Cdecl)]
                public static extern IntPtr Create();

                [DllImport(GtkNativeDll, EntryPoint = "gtk_file_filter_set_name", CallingConvention = CallingConvention.Cdecl)]
                public static extern void SetName(IntPtr filter, IntPtr name);

                [DllImport(GtkNativeDll, EntryPoint = "gtk_file_filter_add_pattern", CallingConvention = CallingConvention.Cdecl)]
                public static extern void AddPattern(IntPtr filter, IntPtr pattern);
            }
        }

        public static class Css
        {
            [DllImport(GtkNativeDll, EntryPoint = "gtk_css_provider_new", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Create();

            [DllImport(GtkNativeDll, EntryPoint = "gtk_css_provider_load_from_data", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool LoadData(IntPtr provider, IntPtr data, IntPtr size, IntPtr error);
        }

        public static class StyleContext
        {
            [DllImport(GtkNativeDll, EntryPoint = "gtk_widget_get_style_context", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Get(IntPtr widget);

            [DllImport(GtkNativeDll, EntryPoint = "gtk_style_context_add_provider", CallingConvention = CallingConvention.Cdecl)]
            public static extern void AddProvider(IntPtr context, IntPtr provider, GtkStyleProviderPriority priority);
        }

        public static class Version
        {
            [DllImport(GtkNativeDll, EntryPoint = "gtk_get_major_version", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint Major();

            [DllImport(GtkNativeDll, EntryPoint = "gtk_get_minor_version", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint Minor();

            [DllImport(GtkNativeDll, EntryPoint = "gtk_get_micro_version", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint Micro();

            public static bool IsAtLeast(int major, int minor, int micro)
            {
                return Major() >= major
                    && Minor() >= minor
                    && Micro() >= micro;
            }
        }

        [DllImport(GtkNativeDll, EntryPoint = "gtk_init_check", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Init(ref int argc, ref IntPtr argv);

        [DllImport(GtkNativeDll, EntryPoint = "gtk_main", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Main();

        [DllImport(GtkNativeDll, EntryPoint = "gtk_main_iteration", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MainIteration();

        [DllImport(GtkNativeDll, EntryPoint = "gtk_main_quit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Quit();

        [DllImport(GtkNativeDll, EntryPoint = "gtk_main_iteration_do", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MainIteration(bool blocking);
    }
}
